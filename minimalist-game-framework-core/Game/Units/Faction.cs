using System;
using System.Collections;
using System.Collections.Generic;

public class Faction
{
	private int mapWidth, mapHeight;

	private bool[,] mapExplored; // tiles not under fog of war
	private int[,] mapVision; // tiles that are fully in vision (updated)

	public string name;
	public UnitDex factionUnits;
	public double cookies;
	public double cookiesPerTurn;
	public Tuple<Vector2, int> lastCameraState;
	public bool lost;

    public int techTreeID;
    public List<string> customTechs = new List<string>();

    public Faction(int mapWidth, int mapHeight, String name, int techTreeID)
	{
		this.mapWidth = mapWidth;
		this.mapHeight = mapHeight;
		this.name = name;
		mapExplored = new bool[mapWidth, mapHeight];
		mapVision = new int[mapWidth, mapHeight];
		for (int i = 0; i < mapWidth; i++)
		{
			for (int j = 0; j < mapHeight; j++)
			{
				mapExplored[j, i] = false;
				mapVision[j, i] = 0;
			}
		}

		cookies = 35;
		factionUnits = new UnitDex();
		lost = false;
		this.techTreeID = techTreeID;
		if (techTreeID != -1) TechTreeManager.makeTreeItems(techTreeID); 
	}

	public void testingMethod()
	{
		for (int i = 0; i < mapWidth; i++)
		{
			for (int j = 0; j < mapHeight; j++)
			{
				mapExplored[j, i] = true;
				mapVision[j, i]++;
			}
		}
	}

	// remove a unit or building from a tile
	public void moveFrom(Tuple<int, int> indices, int vision)
	{
		int mapX = indices.Item1;
		int mapY = indices.Item2;

		for (int i = mapX - vision; i <= mapX + vision; i++)
		{
			if (i < 0 || i >= mapWidth) continue;
			for (int j = mapY - vision; j <= mapY + vision; j++)
			{
				if (j < 0 || j >= mapHeight) continue;
				mapVision[j, i]--;
			}
		}
	}

	/// <summary>
	/// Move a unit or building to a tile.
	/// </summary>
	/// <param name="indices">Map indices.</param>
	/// <param name="vision">Vision value.</param>
	public void moveTo(Tuple<int, int> indices, int vision)
	{
		bool explored = false;
		int mapX = indices.Item1, mapY = indices.Item2;
		
        for (int i = mapX - vision; i <= mapX + vision; i++)
        {
			if (i < 0 || i >= mapWidth) continue;
			for (int j = mapY - vision; j <= mapY + vision; j++)
			{
				if (j < 0 || j >= mapHeight) continue;
				if (!mapExplored[j, i])
				{
					mapExplored[j, i] = true; explored = true;
 				}
				mapExplored[j, i] = true;
				mapVision[j, i]++;
			}
		}

		if(explored) Engine.PlaySound(soundStorage.explored);
	}

	// has a tile been explored
	public bool explored(Tuple<int, int> indices)
	{
		return mapExplored[indices.Item2, indices.Item1];
	}

    // is a tile in vision
    public bool inVision(Tuple<int, int> indices)
    {
        return mapVision[indices.Item2, indices.Item1] > 0;
	}

	// if the faction is completely destroyed
	public bool isDefeated()
	{
		bool canLose = true;
		for (int i = 0; i < mapVision.GetLength(0); i++)
		{
			for (int j = 0; j < mapVision.GetLength(1); j++)
			{
				if (mapVision[i, j] > 0) { canLose = false; };
			}
		}
		// if the faction has no vision, they lose the game and get to spectate
		lost = canLose;
		if (lost)
		{
			for (int i = 0; i < mapVision.GetLength(0); i++)
			{
				for (int j = 0; j < mapVision.GetLength(1); j++)
				{
					mapVision[i, j] = 1;
				}
			}
		}

		return (canLose);
    }

	public List<string> save()
	{
		List<string> l = new List<string>();

		Utilities.write(ref l, mapWidth);
		Utilities.write(ref l, mapHeight);

		for (int i = 0; i < mapWidth; i++)
		{
			for (int j = 0; j < mapHeight; j++)
			{
				Utilities.write(ref l, mapExplored[i, j]);
				Utilities.write(ref l, mapVision[i, j]);
			}
		}
		Utilities.write(ref l, name);
        Utilities.write(ref l, cookies);
        Utilities.write(ref l, cookiesPerTurn);
		Utilities.write(ref l, lastCameraState);
		Utilities.write(ref l, lost);
		Utilities.write(ref l, techTreeID);

		int c = customTechs.Count;
		Utilities.write(ref l, c);
		foreach (string str in customTechs)
			Utilities.write(ref l, str);

		List<string> fl = factionUnits.save();
		foreach (string str in fl) l.Add(str);

		return l;
    }
	public int load(ref List<string> l, int i)
	{
		Utilities.read(ref mapWidth, l[i++]);
        Utilities.read(ref mapHeight, l[i++]);

		mapExplored = new bool[mapWidth, mapHeight];
		mapVision = new int[mapWidth, mapHeight];
		for (int j = 0; j < mapWidth; j++)
		{
			for (int k = 0; k < mapHeight; k++)
			{
                Utilities.read(ref mapExplored[j, k], l[i++]);
				Utilities.read(ref mapVision[j, k], l[i++]);
            }
		}
        Utilities.read(ref name, l[i++]);
        Utilities.read(ref cookies, l[i++]);
        Utilities.read(ref cookiesPerTurn, l[i++]);
		Utilities.read(ref lastCameraState, l[i], l[i + 1], l[i + 2]);
		i += 3;
		Utilities.read(ref lost, l[i++]);
		Utilities.read(ref techTreeID, l[i++]);

		int c = 0;
		Utilities.read(ref c, l[i++]);
		customTechs = new List<string>();
		for (int j = 0; j < c; j++)
		{
			string s = "";
			Utilities.read(ref s, l[i++]);
			customTechs.Add(s);
		}

		i = factionUnits.load(ref l, i);

		return i;
    }
}