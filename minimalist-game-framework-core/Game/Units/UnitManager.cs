using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;


public struct TilesRange
{
    public HashSet<Tuple<int, int>> moveToAbleTiles;
    public HashSet<Tuple<int, int>> attackableTiles;

    public TilesRange(HashSet<Tuple<int, int>> moveToAbleTiles, HashSet<Tuple<int, int>> attackableTiles)
    {
        this.moveToAbleTiles = moveToAbleTiles;
        this.attackableTiles = attackableTiles;
    }
}

public static class UnitManager
{
    private static IDGenerator homelandSecurity = new IDGenerator();

    //The biomes the unit cannot move to 
    private static HashSet<Biomes> unmoveableBiomes = new HashSet<Biomes>();
    private static Dictionary<int, Unit> units = new Dictionary<int, Unit>();

    //The unit that's selected 
    public static int selectedUnitID = -1;
    public static TilesRange selUnitRange;

    private static Texture moveTex = Engine.LoadTexture("movement.png");
    private static Texture attackTex = Engine.LoadTexture("attack.png");

    //Initialize all the unit properties
    static UnitManager() { }

    public static void reset()
    {
        homelandSecurity = new IDGenerator();
        unmoveableBiomes = new HashSet<Biomes>();
        units = new Dictionary<int, Unit>();
        selectedUnitID = -1;
        selUnitRange = new TilesRange();
    }

    // get methods
    public static Dictionary<int, Unit> getUnits() { return units; }
    public static int getFactionID(int unitID) { return units[unitID].getFactionID(); }
    public static Units getUnitType(int unitID) { return units[unitID].getUnitType(); }
    public static UnitAttributes getAttributes(int unitID) { return units[unitID].getAttributes(); }
    public static int getVision(int unitID, Tuple<int, int> indices) { return units[unitID].getVision(indices); }
    public static int getDefence(int unitID, Tuple<int, int> indices) { return units[unitID].getDefence(indices); }
    public static int getAttack(int unitID, Tuple<int, int> indices) { return units[unitID].getAttack(indices); }
    public static int getMobility(int unitID) { return units[unitID].getMobility(); }
    public static Tuple<int, int> getPosition(int unitID) { return units[unitID].getPosition(); }

    public static void setMobility(int unitID, int mobility) { units[unitID].setMobility(mobility); }

    //Manages the logic for selecting a unit and bringing up the relevant UI and such
    public static void selectUnit(int unitID)
    {
        //Deselects the existing unit
        deselect();
        selectedUnitID = unitID;
        renderUnitRange(unitID);
    }

    public static void renderUnit(int unitID, Vector2 pos, int scale) { units[unitID].render(pos, scale); }

    // Manages the movement of a unit to a tile 
    public static void moveTo(int unitID, Tuple<int, int> tileIndices)
    {
        Engine.PlaySound(soundStorage.returnRandomSound(soundStorage.footsteps));
        MapManager.deoccupy(units[unitID].getPosition(), unitID);
        units[unitID].moveTo(tileIndices);
        MapManager.occupy(tileIndices, unitID);
        units[unitID].setMobility(0);

        deselect();
    }

    public static bool takeDamage(int unitID, int damage) { return units[unitID].takeDamage(damage); }

    // Manages the attacking of a unit in a given tile 
    public static void attack(int unitID, Tuple<int, int> tileIndices)
    {
        //Eliminates all potential attackable tiles 
        selUnitRange.attackableTiles.Clear();

        //If the unit is ranged
        if (getAttributes(unitID).attackRange > 0)
        {
            //Attack without moving
            if (takeDamage(MapManager.occupiedBy(tileIndices), getAttack(unitID, getPosition(unitID))))
            {
                destroyUnit(MapManager.occupiedBy(tileIndices), tileIndices);
            }

            Engine.PlaySound(soundStorage.returnRandomSound(soundStorage.snowballs));
        }

        MapManager.setHighlight(tileIndices, Color.Transparent);
        setMobility(unitID, 0);
        units[unitID].attack();
        deselect();
    }

    /// <summary>
    /// BFS for pathing.
    /// </summary>
    /// <param name="uLocation">Unit location.</param>
    /// <param name="factionID">Name of current faction.</param>
    /// <param name="range">Path range.</param>
    /// <param name="validThrough">Function to determine which tiles can be gone through.</param>
    /// <param name="validTo">Function to determine which tiles can be gone to.</param>
    /// <returns></returns>
    private static HashSet<Tuple<int, int>> BFS(Tuple<int, int> uLocation, int factionID, int range,
        Func<Tuple<int, int>, int, bool> validThrough, Func<Tuple<int, int>, int, bool> validTo)
    {
        HashSet<Tuple<int, int>> validTiles = new HashSet<Tuple<int, int>>();
        HashSet<Tuple<int, int>> visited = new HashSet<Tuple<int, int>>();
        Queue<Tuple<Tuple<int, int>, int>> tiles = new Queue<Tuple<Tuple<int, int>, int>>();

        tiles.Enqueue(new Tuple<Tuple<int, int>, int>(uLocation, range));
        while (tiles.Count > 0)
        {
            // Dequeue.
            Tuple<Tuple<int, int>, int> current = tiles.Dequeue();
            Tuple<int, int> currentTile = current.Item1;
            int currentRange = current.Item2;

            // If already visited or cannot move to, continue.
            if (visited.Contains(currentTile) || !validThrough(currentTile, factionID)) continue;

            // Visit and add to tile list.
            visited.Add(currentTile);
            if (validTo(currentTile, factionID)) validTiles.Add(currentTile);

            // If cannot move anymore, continue.
            if (currentRange == 0) continue;

            // Find neighbors and add to queue.
            List<Tuple<int, int>> neighbors = MapManager.neighboringTiles(currentTile);
            foreach (Tuple<int, int> tile in neighbors)
            {
                tiles.Enqueue(new Tuple<Tuple<int, int>, int>(tile, currentRange - 1));
            }
        }

        return validTiles;
    }

    // helpers for bfs for unit movement
    private static bool validMoveThrough(Tuple<int, int> tile, int factionID)
    {
        return MapManager.occupiedBy(tile) == -1 || (MapManager.occupiedBy(tile) != -1 &&
        UnitManager.getFactionID(MapManager.occupiedBy(tile)) == factionID) && moveableBiome(tile, factionID);
    }
    private static bool validMoveTo(Tuple<int, int> tile, int factionID)
    { return MapManager.occupiedBy(tile) == -1; }
    private static bool validAttackThrough(Tuple<int, int> tile, int factionID) { return true; }
    private static bool validAttack(Tuple<int, int> tile, int factionID)
    { return MapManager.occupiedBy(tile) != -1 && UnitManager.getFactionID(MapManager.occupiedBy(tile)) != factionID; }

    private static TilesRange findTiles(Tuple<int, int> uLocation, int factionID, int mobility, int attackRange)
    {
        TilesRange tr = new TilesRange(BFS(uLocation, factionID, mobility, validMoveThrough, validMoveTo),
            BFS(uLocation, factionID, attackRange, validAttackThrough, validAttack));
        return tr;

    }

    // takes an List<MapTile> of biomes that cannot be moved to, checks if tile is moveable to
    private static bool moveableBiome(Tuple<int, int> t, int factionID)
    { return !unmoveableBiomes.Contains(MapManager.getBiome(t)); }

    public static void deselect()
    {
        selectedUnitID = -1;

        if (!selUnitRange.Equals(default(TilesRange)))
        {
            foreach (Tuple<int, int> m in selUnitRange.moveToAbleTiles) MapManager.setHighlight(m, Color.Transparent);
            foreach (Tuple<int, int> m in selUnitRange.attackableTiles) MapManager.setHighlight(m, Color.Transparent);
        }
    }

    private static void renderUnitRange(int unitID)
    {
        selUnitRange = findTiles(getPosition(unitID), getFactionID(unitID),
            getMobility(unitID), units[unitID].canAttack ? getAttributes(unitID).attackRange : 0);

        // highlights different contexts with different colors
        foreach (Tuple<int, int> m in selUnitRange.moveToAbleTiles) MapManager.setHighlight(m, Color.Blue);
        foreach (Tuple<int, int> m in selUnitRange.attackableTiles) MapManager.setHighlight(m, Color.Red);
    }

    // handles unit production in forts and initial map spawn
    public static int createUnit(int factionID, Units unitT, Tuple<int, int> mapIndices)
    {
        int unitID = homelandSecurity.genID();
        units[unitID] = new Unit(factionID, unitT,
            FactionManager.getFactionUnits(factionID).getUnitAttributes(unitT), mapIndices);

        MapManager.occupy(mapIndices, unitID);
        FactionManager.moveTo(factionID, mapIndices, units[unitID].getVision(mapIndices));

        return unitID;
    }

    public static void destroyUnit(int unitID, Tuple<int, int> mapIndices)
    {
        Engine.PlaySound(soundStorage.unitKilled);
        MapManager.deoccupy(mapIndices, unitID);
        FactionManager.moveFrom(getFactionID(unitID), mapIndices, units[unitID].getVision(mapIndices));
        units.Remove(unitID);
        //homelandSecurity.recycleID(unitID);
    }

    public static void resetMovementPoints()
    {
        foreach (Unit u in units.Values)
        {
            u.resetMobility();
            u.resetAttack();
        }
    }

    static List<string> l = new List<string>();
    public static List<string> save()
    {
        l.Clear();

        int c = unmoveableBiomes.Count;
        Utilities.write(ref l, c);
        foreach (Biomes b in unmoveableBiomes)
        {
            Utilities.write(ref l, b);
        }

        c = units.Count;
        Utilities.write(ref l, c);
        foreach (KeyValuePair<int, Unit> u in units)
        {
            Utilities.write(ref l, u.Key);
            List<string> ul = u.Value.save();
            foreach (string str in ul) l.Add(str);
        }

        List<string> hsl = homelandSecurity.save();
        foreach (string str in hsl) l.Add(str);

        return l;
    }
    public static int load(ref List<string> l, int i)
    {
        int c = 0;
        Utilities.read(ref c, l[i++]);
        unmoveableBiomes = new HashSet<Biomes>();
        for (int j = 0; j < c; j++)
        {
            Biomes b = new Biomes();
            Utilities.read(ref b, l[i++]);
            unmoveableBiomes.Add(b);
        }

        Utilities.read(ref c, l[i++]);
        units = new Dictionary<int, Unit>();
        for (int j = 0; j < c; j++)
        {
            int ui = 0;
            Utilities.read(ref ui, l[i++]);
            Unit u = new Unit(0, new Units(), new UnitAttributes(0, 0, 0, 0, 0, 0, null, "art.png"), new Tuple<int, int>(0, 0));
            i = u.load(ref l, i);
            units.Add(ui, u);
        }

        i = homelandSecurity.load(ref l, i);

        return i;
    }
}