using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TreeItem
{
    private int parentID;
    private int cost;
    private string[] desc;
    private bool unlock = false;
    private HashSet<String> reqs = new HashSet<String>();
    private HashSet<String> unlocked = new HashSet<String>();
    private string name;
    private Tuple<int, double>[] upgrades;
    private Vector2 pos;

    public TreeItem(string name, HashSet<String> unlocked, HashSet<String> reqs, string[] desc, int cost, Tuple<int, double>[] upgrades, Vector2 pos, int parentID)
    {
        this.unlocked = unlocked;
        this.reqs = reqs;
        // add the cookie cost to description
        this.desc = new string[desc.Length + 1];
        for (int i = 0; i < this.desc.Length - 1; i++)
        {
            this.desc[i] = desc[i];
        }
        this.desc[this.desc.Length - 1] = ("Price: " + cost.ToString() + " Cookies");

        this.cost = cost;
        this.name = name;
        this.upgrades = upgrades;
        this.pos = pos;
        this.parentID = parentID;
    }

    public bool unlockable() { return reqs.Count == 0 && !unlock; }

    public bool unlockItem()
    {
        if (unlockable())
        {
            unlock = true;
            return true;

        }
        return false;
    }

    // sends an unlock request to the tech tree
    // workaround to actions requiring a specific amount of inputs and not fitting into the button class
    public void unlockSelf()
    {
        TechTreeManager.unlockTreeItems(parentID, name);
    }

    public string[] getDesc()
    {
        return this.desc;
    }

    public string getName()
    {
        return this.name;
    }

    public int calcTex(double cookies)
    {
        if (unlock) { return 0; }
        else if (cookies >= cost && unlockable()) { return 1; }
        else if (unlockable()) { return 2; }
        else { return 3; }
    }

    public HashSet<string> getUnlockedItems()
    {
        return this.unlocked;
    }

    public void removeReq(string s)
    {
        reqs.Remove(s);
    }

    public int getCost()
    {
        return this.cost;
    }

    public Tuple<int, double>[] getUpgrades() 
    {
        return this.upgrades;
    }

    public Vector2 getPos()
    {
        return pos;
    }

    public List<string> save()
    {
        List<string> l = new List<string>();

        Utilities.write(ref l, parentID);
        Utilities.write(ref l, cost);
        Utilities.write(ref l, unlock);
        Utilities.write(ref l, name);
        Utilities.write(ref l, pos);

        int c = desc.Length;
        Utilities.write(ref l, c);
        for (int i = 0; i < c; i++)
            Utilities.write(ref l, desc[i]);

        c = reqs.Count;
        Utilities.write(ref l, c);
        foreach (string str in reqs)
            Utilities.write(ref l, str);

        c = unlocked.Count;
        Utilities.write(ref l, c);
        foreach (string str in unlocked)
            Utilities.write(ref l, str);

        c = upgrades.Length;
        Utilities.write(ref l, c);
        for (int i = 0; i < c; i++)
            Utilities.write(ref l, upgrades[i]);

        return l;
    }
    public int load(ref List<string> l, int i)
    {
        Utilities.read(ref parentID, l[i++]);
        Utilities.read(ref cost, l[i++]);
        Utilities.read(ref unlock, l[i++]);
        Utilities.read(ref name, l[i++]);
        Utilities.read(ref pos, l[i], l[i + 1]);
        i += 2;

        int c = 0;
        Utilities.read(ref c, l[i++]);
        desc = new string[c];
        for (int j = 0; j < c; j++)
            Utilities.read(ref desc[j], l[i++]);

        Utilities.read(ref c, l[i++]);
        reqs = new HashSet<string>();
        for (int j = 0; j < c; j++)
        {
            string s = "";
            Utilities.read(ref s, l[i++]);
            reqs.Add(s);
        }

        Utilities.read(ref c, l[i++]);
        unlocked = new HashSet<string>();
        for (int j = 0; j < c; j++)
        {
            string s = "";
            Utilities.read(ref s, l[i++]);
            unlocked.Add(s);
        }

        Utilities.read(ref c, l[i++]);
        upgrades = new Tuple<int, double>[c];
        for (int j = 0; j < c; j++)
        {
            Utilities.read(ref upgrades[j], l[i], l[i + 1]);
            i += 2;
        }

        return i;
    }
}

