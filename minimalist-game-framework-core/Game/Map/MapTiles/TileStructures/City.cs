using System;
using System.Collections.Generic;

public class City : TileStructure
{
    public List<Tuple<int, int>> controlledTilesIDs;
    int faction;
    public double productivity;
    Tuple<int, int> position;

    public City(Tuple<int, int> originalTileIndices) : base(originalTileIndices, "tileStructures/city.png") { }

    public City(Tuple<int, int> originalTileIndices, int factionID) : base(originalTileIndices, "tileStructures/city.png")
    {
        this.faction = factionID;
        position = originalTileIndices;
        productivity = 1;
        //Generates and initializes a 5x5 "control zone" for the city 
        controlledTilesIDs = new List<Tuple<int, int>>();
        for(int i = originalTileIndices.Item1 - 2; i <= originalTileIndices.Item1 + 2; i++)
        {
            for (int j = originalTileIndices.Item2 - 2; j <= originalTileIndices.Item2 + 2; j++)
            {
                Tuple<int, int> tileIndices = new Tuple<int, int>(i, j);
                if ((TileStructureManager.getStructureType(MapManager.getStructureID(tileIndices)) != typeof(Town) ||
                    tileIndices.Equals(position)) && MapManager.getFactionID(tileIndices) == -1)
                {
                    controlledTilesIDs.Add(tileIndices);
                    MapManager.setFactionID(tileIndices, factionID);
                    FactionManager.changeCookiesPerTurn(faction, MapManager.getProductivity(tileIndices));
                }
            } 
        }
    }

    public void killCity()
    {
        FactionManager.changeCookiesPerTurn(faction, -productivity);
        foreach (Tuple<int, int> tile in controlledTilesIDs)
        {
            FactionManager.changeCookiesPerTurn(faction, -MapManager.getProductivity(tile));
            MapManager.setFactionID(tile, -1);
        }
    }

    //Get the faction this city is associated with 
    public override int getFactionID()
    {
        return faction;
    }

    public override void setFactionID(int factionID)
    {
        this.faction= factionID;
    }

    public override void render(Vector2 pos, int scale)
    {
        Engine.DrawTexture(renderTex, pos, size: new Vector2(scale, scale), scaleMode: TextureScaleMode.Nearest);
        Engine.DrawTexture(TileStructure.cityOverlay, pos, FactionManager.getFactionColor(faction), size: new Vector2(scale, scale), scaleMode: TextureScaleMode.Nearest);
    }

    public void drawBorder(int scale)
    {
        List<Tuple<int, int>> temp;
        Vector2 pos;
        Color color;

        foreach (Tuple<int, int> t in this.controlledTilesIDs)
        {
            pos = MapManager.getScreenCoordinates(GameManager.cam.getPosition(), t);
            temp = MapManager.getFourSurrounding(t);
            color = FactionManager.getFactionColor(this.faction);

            if (temp.Count > 0 && checkBorder(temp[0])) Engine.DrawRectSolid(new Bounds2(pos, new Vector2(scale / 10, scale)), color);
            if (temp.Count > 1 && checkBorder(temp[1])) Engine.DrawRectSolid(new Bounds2(pos + new Vector2(scale * 9 / 10, 0), new Vector2(scale / 10, scale)), color);
            if (temp.Count > 2 && checkBorder(temp[2])) Engine.DrawRectSolid(new Bounds2(pos, new Vector2(scale, scale / 10)), color);
            if (temp.Count > 3 && checkBorder(temp[3])) Engine.DrawRectSolid(new Bounds2(pos + new Vector2(0, scale * 9 / 10), new Vector2(scale, scale / 10)), color);
        }
    }

    // checks tiles to draw a border if necessary
    private bool checkBorder(Tuple<int, int> t)
    {
        if (MapManager.getFactionID(t) == this.faction)
            return false;
        return true;
    }

    public override List<string> save()
    {
        List<string> l = new List<string>();

        Utilities.write(ref l, base.location);

        int c = controlledTilesIDs.Count;
        Utilities.write(ref l, c);
        for (int i = 0; i < c; i++)
        {
            Utilities.write(ref l, controlledTilesIDs[i]);
        }

        Utilities.write(ref l, faction);
        Utilities.write(ref l, productivity);
        Utilities.write(ref l, position);

        return l;
    }
    public override int load(ref List<string> l, int i)
    {
        Utilities.read(ref base.location, l[i], l[i + 1]);
        i += 2;

        int c = 0;
        Utilities.read(ref c, l[i++]);
        controlledTilesIDs = new List<Tuple<int, int>>();
        for (int j = 0; j < c; j++)
        {
            Tuple<int, int> id = new Tuple<int, int>(0, 0);
            Utilities.read(ref id, l[i], l[i + 1]);
            i += 2;
            controlledTilesIDs.Add(id);
        }

        Utilities.read(ref faction, l[i++]);
        Utilities.read(ref productivity, l[i++]);
        Utilities.read(ref position, l[i], l[i + 1]);
        i += 2;

        return i;
    }
}