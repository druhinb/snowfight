using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class IDGenerator
{
	HashSet<int> ids;
	int p = 0;

	public IDGenerator()
	{
		ids = new HashSet<int>();
	}

	public int genID()
	{
		if (ids.Count > 0)
		{
			int id = ids.Single();
			ids.Remove(id);
			return id;
		}
		else return ++p;
	}

	public void recycleID(int id)
	{
		ids.Add(id);
		while (ids.Contains(p - 1)) ids.Remove(--p);
	}

	public List<string> save()
	{
		List<string> l = new List<string>();

		Utilities.write(ref l, p);

		int c = ids.Count;
		Utilities.write(ref l, c);
		foreach (int id in ids)
		{
			Utilities.write(ref l, id);
		}

		return l;
	}
	public int load(ref List<string> l, int i)
	{
		Utilities.read(ref p, l[i++]);

		int c = 0;
		Utilities.read(ref c, l[i++]);
		ids = new HashSet<int>();
		for (int j = 0; j < c; j++)
		{
			int id = 0;
			Utilities.read(ref id, l[i++]);
			ids.Add(id);
		}

		return i;
	}
}

