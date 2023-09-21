using System;
using System.Collections.Generic;

public static class TechTreeManager
{
	static IDGenerator homelandSecurity = new IDGenerator();
	static Dictionary<int, TechTree> techTrees = new Dictionary<int, TechTree>();

	public static void reset()
	{
		homelandSecurity = new IDGenerator();
		techTrees = new Dictionary<int, TechTree>();
	}

	public static void makeTreeItems(int techTreeID) { techTrees[techTreeID].makeTreeItems(); }
	public static void unlockTreeItems(int techTreeID, string name) { techTrees[techTreeID].unlockTreeItems(name); }
	public static List<TreeItem> getTreeItems(int techTreeID) { return techTrees[techTreeID].getTreeItems(); }

	public static int generateTechTree(int factionID)
	{
		int id = homelandSecurity.genID();
		techTrees[id] = new TechTree(id, factionID);
		return id;
	}
	public static List<string> save()
	{
		List<string> l = new List<string>();

		int c = techTrees.Count;
		Utilities.write(ref l, c);
		foreach (KeyValuePair<int, TechTree> tt in techTrees)
		{
			Utilities.write(ref l, tt.Key);
			List<string> ttl = tt.Value.save();
			foreach (string str in ttl) l.Add(str);
		}

		List<string> hsl = homelandSecurity.save();
		foreach (string str in hsl) l.Add(str);

		return l;
	}
	public static int load(ref List<string> l, int i)
	{
		int c = 0;
		Utilities.read(ref c, l[i++]);
		techTrees = new Dictionary<int, TechTree>();
		for (int j = 0; j < c; j++)
		{
			int tti = 0;
			TechTree tt = new TechTree(-1, -1);

			Utilities.read(ref tti, l[i++]);
			i = tt.load(ref l, i);

			techTrees.Add(tti, tt);
		}

		i = homelandSecurity.load(ref l, i);

		return i;
	}
}