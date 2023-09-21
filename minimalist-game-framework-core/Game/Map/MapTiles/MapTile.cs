using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapTile
{
    public int x, y;
    Biomes biome;
    Texture tileTex;
    Color highlightColor;
    int structureID = -1;
    Features feature;
    public bool isImproved;
    int factionID = -1;
    Texture overlayTexture;

    double productivity;

    HashSet<int> occupied;
    private static Texture fog = Engine.LoadTexture("tile/fog.png");

    public MapTile(Tuple<int, int> pos, Biomes biome)
    {
        this.x = pos.Item1;
        this.y = pos.Item2;
        this.biome = biome;
        this.tileTex = BiomeManager.getBiomeAttributes(biome).texture;
        productivity = 0;

        occupied = new HashSet<int>();
    }

    public Biomes getBiome() { return biome; }
    public int getStructureID() { return structureID; }
    public Features getFeature() { return feature; }
    public int getFactionID() { return factionID; }
    public void setFactionID(int factionID) { this.factionID = factionID; }
    public double getProductivity() { return productivity; }

    public bool isOccupied() { return occupiedBy() != -1; }

    /// <summary>
    /// Set the highlight colour for the tile
    /// </summary>
    /// <param name="color">the color for the highlight</param>
    public void setHighlight(Color color) { highlightColor = color; }
    public void setOverlay(Texture texture) {overlayTexture = texture;}
    public bool setStructure(int structureID)
    {
        this.structureID = structureID;
        return true;
    }

    // sets a bonus feature that can be developed
    // forests convert plains to forests for tile attribute reasons
    public void setFeature(Features feature)
    {
        this.feature = feature;
        this.productivity = FeatureManager.getFeatureAttributes(feature).unimprovedProductivity;
    }

    /// <summary>
    /// Occupy this tile with a unit if possible
    /// </summary>
    /// <param name="unitID">The unit trying to occupy</param>
    /// <returns> whether the occupation was successful </returns>
    public bool occupy(int unitID)
    {
        if (isOccupied()) return false;
        else
        {
            occupied.Add(unitID);
            int unitFaction = UnitManager.getFactionID(unitID);

            TileStructureManager.occupy(unitID, structureID);

            return true;
        }
    }

    /// <summary>
    /// Removes an occupier from the current tile
    /// </summary>
    /// <param name="unitID">The ID of the unit to be removed</param>
    /// <returns> whether the deoccupation was successful </returns>s
    public bool deoccupy(int unitID)
    {
        return occupied.Remove(unitID);
    }

    /// <summary>
    /// Get the unit occupying the tile
    /// </summary>
    /// <return> the occupier unit</return>
    public int occupiedBy()
    {
        if (occupied.Count > 0)
        {
            return occupied.Single();
        }
        else
        {
            return -1;
        }
    }

    // changes the improvement status of the tile
    // most other handling happens in tilemap
    public void improve()
    {
        isImproved = true;
        this.productivity = FeatureManager.getFeatureAttributes(this.feature).improvedProductivity;
        Engine.PlaySound(soundStorage.build);
    }

    /// <summary>
    /// Render the texture for this tile
    /// </summary>
    /// <param name="pos">the position to render at</param>
    /// <param name="scale">the size to render the tile at</param>
    public void renderObfuscated(Vector2 pos, int scale)
    {
        Engine.DrawTexture(fog, pos, size: new Vector2(scale, scale), scaleMode: TextureScaleMode.Nearest);
    }

    /// <summary>
    /// Render the texture for this tile
    /// </summary>
    /// <param name="pos">the position to render at</param>
    /// <param name="scale">the size to render the tile at</param>
    public void renderTile(Vector2 pos, int scale)
    {
        Engine.DrawTexture(tileTex, pos, size: new Vector2(scale, scale), scaleMode: TextureScaleMode.Nearest);
        if (structureID != -1)
            TileStructureManager.render(structureID, pos, scale);

        if (factionID != -1) drawBorder(scale);
        Engine.DrawRectSolid(new Bounds2(pos, new Vector2(scale, scale)), new Color(0, 0, 0, 200));
    }

    /// <summary>
    /// Render the tile AND all units / buildings on it
    /// </summary>
    /// <param name="pos">the position to render at</param>
    /// <param name="scale">the size to render the tile at</param>
    public void renderAll(Vector2 pos, int scale)
    {
        Engine.DrawTexture(tileTex, pos, size: new Vector2(scale, scale), scaleMode: TextureScaleMode.Nearest);

        if (feature != default(Features))
            if (!isImproved)
                Engine.DrawTexture(FeatureManager.getFeatureAttributes(feature).texture, pos,
                size: new Vector2(scale, scale), scaleMode: TextureScaleMode.Nearest);

        if (factionID != -1) drawBorder(scale);
        Engine.DrawRectEmpty(new Bounds2(pos, new Vector2(scale, scale)), highlightColor.WithAlpha(0.2f));
        Engine.DrawRectSolid(new Bounds2(pos, new Vector2(scale, scale)), highlightColor.WithAlpha(0.25f));
        if(overlayTexture != null)
            Engine.DrawTexture(overlayTexture, pos, size: new Vector2(scale, scale), color: new Color(255, 255, 255, 175), scaleMode: TextureScaleMode.Nearest);

        if (structureID != -1)
            TileStructureManager.render(structureID, pos, scale);
        if (occupied.Count > 0)
        {
            foreach (int unitID in occupied)
            {
                UnitManager.renderUnit(unitID, pos, scale);
            }
        }
    }

    // draws a border for the given tile based on how it's controlled
    public void drawBorder(int scale)
    {
        List<Tuple<int, int>> temp;
        Vector2 pos;
        Color color;

        pos = MapManager.getScreenCoordinates(GameManager.cam.getPosition(), new Tuple<int, int>(x, y));
        temp = MapManager.getFourSurrounding(new Tuple<int, int>(x, y));
        color = FactionManager.getFactionColor(this.factionID);

        if (temp.Count > 0 && MapManager.getFactionID(temp[0]) != this.factionID) Engine.DrawRectSolid(new Bounds2(pos, new Vector2(scale / 10, scale)), color);
        if (temp.Count > 1 && MapManager.getFactionID(temp[1]) != this.factionID) Engine.DrawRectSolid(new Bounds2(pos + new Vector2(scale * 9 / 10, 0), new Vector2(scale / 10, scale)), color);
        if (temp.Count > 2 && MapManager.getFactionID(temp[2]) != this.factionID) Engine.DrawRectSolid(new Bounds2(pos, new Vector2(scale, scale / 10)), color);
        if (temp.Count > 3 && MapManager.getFactionID(temp[3]) != this.factionID) Engine.DrawRectSolid(new Bounds2(pos + new Vector2(0, scale * 9 / 10), new Vector2(scale, scale / 10)), color);
    }

    public List<string> save()
    {
        List<string> l = new List<string>();

        Utilities.write(ref l, x);
        Utilities.write(ref l, y);
        Utilities.write(ref l, biome);
        Utilities.write(ref l, feature);
        Utilities.write(ref l, isImproved);
        Utilities.write(ref l, factionID);
        Utilities.write(ref l, productivity);
        Utilities.write(ref l, structureID);

        int c = occupied.Count;
        Utilities.write(ref l, c);
        foreach (int mt in occupied)
        {
            Utilities.write(ref l, mt);
        }

        return l;
    }
    public int load(ref List<string> l, int i)
    {
        Utilities.read(ref x, l[i++]);
        Utilities.read(ref y, l[i++]);
        Utilities.read(ref biome, l[i++]);
        this.tileTex = BiomeManager.getBiomeAttributes(biome).texture;
        Utilities.read(ref feature, l[i++]);
        Utilities.read(ref isImproved, l[i++]);
        Utilities.read(ref factionID, l[i++]);
        Utilities.read(ref productivity, l[i++]);
        Utilities.read(ref structureID, l[i++]);

        int c = 0;
        Utilities.read(ref c, l[i++]);
        occupied = new HashSet<int>();
        for (int j = 0; j < c; j++)
        {
            int o = 0;
            Utilities.read(ref o, l[i++]);
            occupied.Add(o);
        }

        return i;
    }
}