using System;
using System.Collections.Generic;

public static class TileStructureManager
{
	static IDGenerator homelandSecurity = new IDGenerator();
	static Dictionary<int, TileStructure> structures = new Dictionary<int, TileStructure>();

    public static readonly int cityVision = 3;
    private static HashSet<int> cities = new HashSet<int>();
    public static int selectedCity = -1;

    public static void reset()
    {
        homelandSecurity = new IDGenerator();
        structures = new Dictionary<int, TileStructure>();
        cities = new HashSet<int>();
        selectedCity = -1;
    }

    public static int getFactionID(int structureID)
        { return (structureID == -1 ? -1 : structures[structureID].getFactionID()); }
    public static Type getStructureType(int structureID)
        { return (structureID == -1 ? null : structures[structureID].GetType()); }

    public static int createTown(Tuple<int, int> position)
    {
        int id = homelandSecurity.genID();
        MapManager.setStructure(position, id);
        structures[id] = new Town(position);
        return id;
    }

    public static int createBuilding(Tuple<int, int> position, string texturePath)
    {
        int id = homelandSecurity.genID();
        MapManager.setStructure(position, id);
        structures[id] = new Building(position, texturePath);
        return id;
    }

    public static void render(int structureID, Vector2 pos, int scale)
        { structures[structureID].render(pos, scale); }

    // turns a eligible square into a fort for the given faction
    public static void incorporateCity(int townID, int factionID)
    {
        Engine.PlaySound(soundStorage.newCity);
        Tuple<int, int> location = structures[townID].location;

        MapManager.setStructure(location, townID);
        cities.Add(townID);
        structures[townID] = new City(location, factionID);
        FactionManager.changeCookiesPerTurn(factionID, ((City)(structures[townID])).productivity);

        FactionManager.moveTo(factionID, location, cityVision);
    }
    // turns city back into town
    public static void separateCity(int cityID, int factionID)
    {
        Engine.PlaySound(soundStorage.capturedCity);
        Tuple<int, int> location = structures[cityID].location;

        if (!FactionManager.factions[structures[cityID].getFactionID()].customTechs.Contains("fortVision"))
            FactionManager.moveFrom(structures[cityID].getFactionID(), structures[cityID].location, cityVision);

        ((City)(structures[cityID])).killCity();
        MapManager.setStructure(location, cityID);
        cities.Remove(cityID);
        structures[cityID] = new Town(location);
    }

    // Manages the logic for selecting a unit and bringing up the relevant UI and such
    public static void selectCity(int cityID)
    {
        // Deselects the existing city
        deselect();
        selectedCity = cityID;
        MenuLoader.cityMenu(cityID);
        // bring up a menu with city UI 
    }

    // Deploys a unit on the given tile 
    public static bool buyUnit(Units unitT)
    {
        if (selectedCity == -1) return false;

        UnitDex dex = FactionManager.getFactionUnits(structures[selectedCity].getFactionID());
        Tuple<int, int> location = structures[selectedCity].location;
        int factionID = structures[selectedCity].getFactionID();

        // If there isn't already a unit on the tile
        if (MapManager.occupiedBy(location) == -1)
        {
            // If the faction can afford the unit
            if (FactionManager.getCookies(factionID) >= dex.getUnitAttributes(unitT).cookieCost)
            {
                Engine.PlaySound(soundStorage.newUnit);
                int unitID = UnitManager.createUnit(factionID, unitT, location);
                UnitManager.setMobility(unitID, 0);
                FactionManager.changeCookies(factionID, -dex.getUnitAttributes(unitT).cookieCost);

                return true;
            }
        }
        return false;
    }

    public static void deselect()
    {
        if (selectedCity != -1 && MenuManager.getMenuAmt() > 1)
            MenuManager.closeMenu();
        selectedCity = -1;
    }

    public static void drawCityBorders(int scale)
    {
        foreach (int c in cities)
            ((City)(structures[c])).drawBorder(scale);
    }

    public static bool occupy(int unitID, int id)
    {
        int unitFaction = UnitManager.getFactionID(unitID);
        if (id == -1) return false;

        if (structures[id] is Town)
        {
            // turns a house into a fort
            incorporateCity(id, unitFaction);
        }
        else if (structures[id] is City && (structures[id].getFactionID() != unitFaction))
        {
            // converts a fort of one faction to another faction
            separateCity(id, structures[id].getFactionID());
            Engine.PlaySound(soundStorage.capturedCity);
            structures[id].setFactionID(unitFaction);
            incorporateCity(id, unitFaction);

            // converts the faction ids of all controlled tiles to the other faction for border drawing
            foreach (Tuple<int, int> coords in ((City)(structures[id])).controlledTilesIDs)
            {
                MapManager.setFactionID(coords, unitFaction);
            }
        }

        return true;
    }

    private static int getID(TileStructure ts)
    {
        switch (ts.GetType().ToString())
        {
            case ("Building"):
                return 0;
            case ("City"):
                return 1;
            case ("Town"):
                return 2;
        }
        return -1;
    }
    private static TileStructure makeTS(int id)
    {
        switch(id)
        {
            case (0):
                return new Building(new Tuple<int, int>(0, 0), "art.png");
            case (1):
                return new City(new Tuple<int, int>(0, 0));
            case (2):
                return new Town(new Tuple<int, int>(0, 0));
        }
        return null;
    }

    static List<string> l = new List<string>();
    public static List<string> save()
    {
        l.Clear();

        List<string> hsl = homelandSecurity.save();
        foreach (string str in hsl) l.Add(str);

        int c = structures.Count;
        Utilities.write(ref l, c);
        foreach (KeyValuePair<int, TileStructure> s in structures)
        {
            Utilities.write(ref l, s.Key);
            int id = getID(s.Value);
            Utilities.write(ref l, id);
            List<string> sl = s.Value.save();
            foreach (string str in sl) l.Add(str);
        }

        c = cities.Count;
        Utilities.write(ref l, c);
        foreach (int city in cities)
        {
            Utilities.write(ref l, city);
        }

        return l;
    }
    public static int load(ref List<string> l, int i)
    {
        i = homelandSecurity.load(ref l, i);

        int c = 0;
        Utilities.read(ref c, l[i++]);
        structures = new Dictionary<int, TileStructure>();
        for (int j = 0; j < c; j++)
        {
            int si = 0;
            Utilities.read(ref si, l[i++]);
            int id = 0;
            Utilities.read(ref id, l[i++]);
            TileStructure ts = makeTS(id);
            i = ts.load(ref l, i);

            structures.Add(si, ts);
        }

        Utilities.read(ref c, l[i++]);
        cities = new HashSet<int>();
        for (int j = 0; j < c; j++)
        {
            int city = 0;
            Utilities.read(ref city, l[i++]);
            cities.Add(city);
        }

        return i;
    }
}
