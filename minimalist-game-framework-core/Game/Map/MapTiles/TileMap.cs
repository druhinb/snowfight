using System;
using System.Collections.Generic;

public class TileMap
{
	MapTile[,] tiles;

	public TileMap(int width, int height)
	{
		tiles = new MapTile[height, width];
	}

	// passes various methods that concern tiles to the actual tile objects
    public Biomes getBiome(Tuple<int, int> tileIndices)
		{ return tiles[tileIndices.Item2, tileIndices.Item1].getBiome(); }
    public int getStructureID(Tuple<int, int> tileIndices)
		{ return tiles[tileIndices.Item2, tileIndices.Item1].getStructureID(); }
    public Features getFeature(Tuple<int, int> tileIndices)
		{ return tiles[tileIndices.Item2, tileIndices.Item1].getFeature(); }
    public bool isImproved(Tuple<int, int> tileIndices)
		{ return tiles[tileIndices.Item2, tileIndices.Item1].isImproved; }
    public int getFactionID(Tuple<int, int> tileIndices)
		{ return tiles[tileIndices.Item2, tileIndices.Item1].getFactionID(); }
	public void setFactionID(Tuple<int, int> tileIndices, int factionID)
		{ tiles[tileIndices.Item2, tileIndices.Item1].setFactionID(factionID); }
    public double getProductivity(Tuple<int, int> tileIndices)
		{ return tiles[tileIndices.Item2, tileIndices.Item1].getProductivity(); }

    public bool isOccupied(Tuple<int, int> tileIndices)
		{ return tiles[tileIndices.Item2, tileIndices.Item1].isOccupied(); }

    public int occupiedBy(Tuple<int, int> tileIndices)
		{ return tiles[tileIndices.Item2, tileIndices.Item1].occupiedBy(); }

    public void setHighlight(Tuple<int, int> tileIndices, Color color)
		{ tiles[tileIndices.Item2, tileIndices.Item1].setHighlight(color); }

	public void setOverlay(Tuple<int, int> tileIndices, Texture texture) 
		{ tiles[tileIndices.Item1, tileIndices.Item2].setOverlay(texture);}
    public void setStructure(Tuple<int, int> tileIndices, int structureID)
		{ tiles[tileIndices.Item2, tileIndices.Item1].setStructure(structureID); }
    public void setFeature(Tuple<int, int> tileIndices, Features feature)
		{ tiles[tileIndices.Item2, tileIndices.Item1].setFeature(feature); }

    public bool occupy(Tuple<int, int> tileIndices, int unitID)
		{ return tiles[tileIndices.Item2, tileIndices.Item1].occupy(unitID); }

    public bool deoccupy(Tuple<int, int> tileIndices, int unitID)
		{ return tiles[tileIndices.Item2, tileIndices.Item1].deoccupy(unitID); }

    public void improve(Tuple<int, int> tileIndices)
		{ tiles[tileIndices.Item2, tileIndices.Item1].improve(); }

	public void renderObfuscated(Tuple<int, int> tileIndices, Vector2 pos, int scale)
		{ tiles[tileIndices.Item2, tileIndices.Item1].renderObfuscated(pos, scale); }
    public void renderTile(Tuple<int, int> tileIndices, Vector2 pos, int scale)
		{ tiles[tileIndices.Item2, tileIndices.Item1].renderTile(pos, scale); }
    public void renderAll(Tuple<int, int> tileIndices, Vector2 pos, int scale)
		{ tiles[tileIndices.Item2, tileIndices.Item1].renderAll(pos, scale); }

	/*
    public void name(Tuple<int ,int> tileIndices)
		{ return tiles[tileIndices.Item2, tileIndices.Item1]; }
	*/

    // Set specific tile
    public void setTile(MapTile tile) { tiles[tile.y, tile.x] = tile; }

	// Get entire map
	public MapTile[,] getMap() { return tiles; }

	public List<string> save()
	{
		List<string> l = new List<string>();

		int height = tiles.GetLength(0);
		int width = tiles.GetLength(1);

        Utilities.write(ref l, height);
		Utilities.write(ref l, width);

		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				List<string> tl = tiles[i, j].save();
				foreach (string str in tl) l.Add(str);
			}
		}

		return l;
	}
	public int load(ref List<string> l, int i)
	{
		int height = 0, width = 0;

		Utilities.read(ref height, l[i++]);
		Utilities.read(ref width, l[i++]);

		tiles = new MapTile[height, width];
		for (int j = 0; j < height; j++)
		{
			for (int k = 0; k < width; k++)
			{
				tiles[j, k] = new MapTile(new Tuple<int, int>(0, 0), Biomes.PLAINS);
				i = tiles[j, k].load(ref l, i);
			}
		}

		return i;
	}
}
