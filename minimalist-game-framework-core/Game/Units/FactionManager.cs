using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class FactionManager
{
	public static Color[] color = { Color.Red, Color.Green, Color.Blue, Color.Purple, Color.Pink, Color.Orange };
	public static Faction[] factions; // store the factions

	static HashSet<String> factionNames; // store the names of the factions

	/// <summary>
	/// Initializes all factions.
	/// </summary>
	/// <param name="numberOfFactions">Number of factions to initialize.</param>
	/// <param name="halfMapDimensions">Half the dimensions of the map in tiles. (halfWidth, halfHeight)</param>
	public static void initialize(int numberOfFactions, Tuple<int, int> halfMapDimensions)
	{
		int mapWidth = 2 * halfMapDimensions.Item1;
		int mapHeight = 2 * halfMapDimensions.Item2;

        factions = new Faction[numberOfFactions];
		factionNames = new HashSet<String>();

		int interval = mapWidth >= mapHeight ? mapWidth / numberOfFactions : mapHeight / numberOfFactions;

		for (int i = 0; i < numberOfFactions; i++)
		{
			factions[i] = new Faction(mapWidth, mapHeight, "Player " + (i + 1).ToString(), TechTreeManager.generateTechTree(i));
            factionNames.Add("Player " + (i + 1).ToString());

			Tuple<int, int> initialLocation = new Tuple<int, int>(interval * i + interval / 2, interval * i + interval / 2);
			factions[i].lastCameraState = new Tuple<Vector2, int>
				(MapManager.getCoordinates(initialLocation), GameManager.defaultScale); 

			int unit = UnitManager.createUnit(i, Units.TEEN, initialLocation);
		}	
	}

	public static void testingMethod(int factionID)
	{
		factions[factionID].testingMethod();
	}

	/// <summary>
	/// Change the name of a faction.
	/// </summary>
	/// <param name="ID">Which faction to change.</param>
	/// <param name="newName">Name to assign to the faction.</param>
	/// <returns>If the name change was successful.</returns>
	public static bool changeName(int ID, String newName)
	{
		if (factionNames.Contains(newName)) return false;
		else
		{
			String oldName = factions[ID].name;
			factionNames.Remove(oldName);
			factionNames.Add(newName);
			factions[ID].name = newName;

			return true;
		}
	}

	public static bool inVision(int factionID, Tuple<int, int> tileIndices) { return factions[factionID].inVision(tileIndices); }
    public static bool explored(int factionID, Tuple<int, int> tileIndices) { return factions[factionID].explored(tileIndices); }

    public static void resetEfficiencies()
	{
		foreach (Faction f in factions)
		{
			f.cookiesPerTurn = 0.0;
		}
	}

	// checks if the current faction is defeated to remove them from the game
	public static bool updateCondition(int ID)
	{
		return factions[ID].isDefeated();
	}
	
	// get and set methods
	public static String getFactionName(int ID) { return factions[ID].name; }
	public static Color getFactionColor(int ID) { return color[ID]; }

	
	public static int getFactionCount() { return factions.Length;}

	public static double getCookies(int ID) { return Math.Round(factions[ID].cookies, 1); }
	public static double getCookiesPerTurn(int ID) { return Math.Round(factions[ID].cookiesPerTurn, 1); }
	public static Tuple<Vector2, int> getLastCameraState(int ID) { return factions[ID].lastCameraState; }
	public static UnitAttributes getUnitAttributes(int ID, Units unitT) { return factions[ID].factionUnits.getUnitAttributes(unitT); }
	public static UnitDex getFactionUnits(int ID) { return factions[ID].factionUnits; }

    public static void setCookies(int ID, double cookies) { factions[ID].cookies = cookies; }
    public static void changeCookies(int ID, double dCookies) { factions[ID].cookies += dCookies; }
	public static void setCookiesPerTurn(int ID, double cookiesPerTurn) { factions[ID].cookiesPerTurn = cookiesPerTurn; }
    public static void changeCookiesPerTurn(int ID, double dCookiesPerTurn) { factions[ID].cookiesPerTurn += dCookiesPerTurn; }
    public static void setLastCameraState(int ID, Tuple<Vector2, int> lastCameraState) { factions[ID].lastCameraState = lastCameraState; }

    public static void moveTo(int ID, Tuple<int, int> indices, int vision) { factions[ID].moveTo(indices, vision); }
    public static void moveFrom(int ID, Tuple<int, int> indices, int vision) { factions[ID].moveFrom(indices, vision); }

    static List<string> l = new List<string>();
    public static List<string> save()
	{
		l.Clear();

		int c = factions.Length;
		Utilities.write(ref l, c);
		for (int i = 0; i < c; i++)
		{
			List<string> fs = factions[i].save();
			foreach (string str in fs) l.Add(str);
		}

		c = factionNames.Count;
		Utilities.write(ref l, c);
		foreach (string factionName in factionNames)
		{
            Utilities.write(ref l, factionName);
        }

		return l;
	}
	public static int load(ref List<string> l, int i)
	{
		int c = 0;
		Utilities.read(ref c, l[i++]);
		factions = new Faction[c];
		for (int j = 0; j < c; j++)
		{
            Faction f = new Faction(0, 0, "", -1);
			i = f.load(ref l, i);
			factions[j] = f;
		}

		Utilities.read(ref c, l[i++]);
		factionNames = new HashSet<string>();
		for (int j = 0; j < c; j++)
		{
			string factionName = "";
			Utilities.read(ref factionName, l[i++]);
			factionNames.Add(factionName);
		}

		return i;
	}
}