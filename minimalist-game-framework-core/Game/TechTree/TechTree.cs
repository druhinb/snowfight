using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

public class TechTree
{
    int id;
    public Dictionary<string, TreeItem> techTreeItems =
        new Dictionary<string, TreeItem>();

    public int factionID;

    public TechTree(int techTreeID, int factionID) 
    {
        this.id = techTreeID;
        this.factionID = factionID;
    }

    // checks if the item is unlockable, if it is unlockable unlock it and removes itself from the required items of other tree items
    public void unlockTreeItems(string s)
    {
        TreeItem t = techTreeItems[s];
        HashSet<string> unlocked = t.getUnlockedItems();

        double cookies = FactionManager.getCookies(factionID);
        if (t.unlockable() && cookies >= t.getCost())
        {
            t.unlockItem();
            upgrade(s);
            Engine.PlaySound(soundStorage.techUnlocked);

            foreach (string str in unlocked)
            {
                techTreeItems[str].removeReq(s);
            }

            FactionManager.changeCookies(factionID, -t.getCost());
            MenuManager.closeMenu();
            MenuLoader.techTree();
        }
    }

    public List<TreeItem> getTreeItems()
    {
        List<TreeItem> l = new List<TreeItem>();

        foreach (TreeItem t in techTreeItems.Values) l.Add(t);

        return l;
    }

    //f.factionUnits.getUnitAttributes(Units.ADULT).attackPoints =  (int)(f.factionUnits.getUnitAttributes(Units.ADULT).attackPoints * upg.Item2);
    //f.factionUnits.getUnitAttributes(Units.TEEN).attackPoints =  (int)(f.factionUnits.getUnitAttributes(Units.TEEN).attackPoints * upg.Item2);
    //f.factionUnits.getUnitAttributes(Units.KID).attackPoints =  (int)(f.factionUnits.getUnitAttributes(Units.KID).attackPoints * upg.Item2);

    public void upgrade(string t) 
    {
        Tuple<int, double>[] upgradeList = techTreeItems[t].getUpgrades();
        foreach (Tuple<int, double> upg in upgradeList) 
        {
            // goes through and modifies all the faction's stats
            // also, updates the stats of all units immediately
            if (upg.Item1 == 1)
            {
                foreach (UnitAttributes u in FactionManager.factions[factionID].factionUnits.unitProperties.Values)
                {
                    u.attackPoints = (int)(u.attackPoints * upg.Item2);
                }
                foreach (Unit u in UnitManager.getUnits().Values)
                {
                    if (u.getFactionID() == factionID) { u.attributes.attackPoints = (int)(u.attributes.attackPoints * upg.Item2); }
                }
            }
            else if (upg.Item1 == 2)
            {
                foreach (UnitAttributes u in FactionManager.factions[factionID].factionUnits.unitProperties.Values)
                {
                    u.healthPoints = (int)(u.healthPoints * upg.Item2);
                }
                foreach (Unit u in UnitManager.getUnits().Values)
                {
                    if (u.getFactionID() == factionID) { u.attributes.healthPoints = (int)(u.attributes.healthPoints * upg.Item2); }
                }
            }
            else if (upg.Item1 == 3)
            {
                foreach (UnitAttributes u in FactionManager.factions[factionID].factionUnits.unitProperties.Values)
                {
                    u.defencePoints = (int)(u.defencePoints * upg.Item2);
                }
                foreach (Unit u in UnitManager.getUnits().Values)
                {
                    if (u.getFactionID() == factionID) { u.attributes.defencePoints = (int)(u.attributes.defencePoints * upg.Item2); }
                }
            }
            // custom non-stat upgrades
            else if (upg.Item1 == 4)
            {
                if (upg.Item2 == 1) { FactionManager.factions[factionID].customTechs.Add("spawnAttack"); }
                else if (upg.Item2 == 2) { FactionManager.factions[factionID].customTechs.Add("fortVision"); }
            }
        }
    }
    public void addTreeItem(string s, TreeItem t)
    {
        techTreeItems.Add(s, t);
    }
    public void makeTreeItems()
    {
        techTreeItems.Clear();

        addTreeItem("Start", new TreeItem("Start", new HashSet<string> { "Thick Jacket", "Blubber", "Bigger Snowballs" }, new HashSet<string> { }, new string[] { "Begin the journey of technological advancements!" }, 0, new Tuple<int, double>[] { }, new Vector2(Game.Resolution.X / 2 - 75, 100), id));

        // defense
        addTreeItem("Thick Jacket", new TreeItem("Thick Jacket", new HashSet<string> { "Thicker Jacket", "Waterproofing" }, new HashSet<string> { "Start" }, new string[] { "A thick jacket protects your units from the cold.", "+15% Armor" }, 20, new Tuple<int, double>[] { new Tuple<int, double>(3, 1.15) }, new Vector2(Game.Resolution.X / 2 - 375, 200), id));
        addTreeItem("Thicker Jacket", new TreeItem("Thicker Jacket", new HashSet<string> { "Thickest Jacket" }, new HashSet<string> { "Thick Jacket" }, new string[] { "A thicker jacket protects your units from the cold a bit better.", "+15% Armor", "+10% Health" }, 30, new Tuple<int, double>[] { new Tuple<int, double>(3, 1.15), new Tuple<int, double>(2, 1.1) }, new Vector2(Game.Resolution.X / 2 - 485, 300), id));
        addTreeItem("Waterproofing", new TreeItem("Waterproofing", new HashSet<string> { "Thickest Jacket" }, new HashSet<string> { "Thick Jacket" }, new string[] { "With advanced waterproofing technology, your scouts can burrow for a long time.", "+ You don't lose vision at forts you used to control if they are captured." }, 15, new Tuple<int, double>[] { new Tuple<int, double>(4, 2) }, new Vector2(Game.Resolution.X / 2 - 265, 300), id));
        addTreeItem("Thickest Jacket", new TreeItem("Thickest Jacket", new HashSet<string> { }, new HashSet<string> { "Thicker Jacket", "Waterproofing" }, new string[] { "The thickest and most waterproof jacket, the ultimate defense in a snowball fight.", "+35% Armor", "+5% Health" }, 55, new Tuple<int, double>[] { new Tuple<int, double>(3, 1.35), new Tuple<int, double>(2, 1.05) }, new Vector2(Game.Resolution.X / 2 - 375, 400), id));

        //health
        addTreeItem("Blubber", new TreeItem("Blubber", new HashSet<string> { "Land Blubber" }, new HashSet<string> { "Start" }, new string[] { "A fancy word for fat.", "+25% Health" }, 35, new Tuple<int, double>[] { new Tuple<int, double>(2, 1.25) }, new Vector2(Game.Resolution.X / 2 - 75, 200), id));
        addTreeItem("Land Blubber", new TreeItem("Land Blubber", new HashSet<string> { "Bubba" }, new HashSet<string> { "Blubber" }, new string[] { "Your units are getting a bit wider.", "+25% Health" }, 45, new Tuple<int, double>[] { new Tuple<int, double>(2, 1.25) }, new Vector2(Game.Resolution.X / 2 - 75, 300), id));
        addTreeItem("Bubba", new TreeItem("Bubba", new HashSet<string> { }, new HashSet<string> { "Land Blubber" }, new string[] { "Your units harness the power of the legendary Bubba.", "+50% Health" }, 100, new Tuple<int, double>[] { new Tuple<int, double>(2, 1.5) }, new Vector2(Game.Resolution.X / 2 - 75, 400), id));

        //attack
        addTreeItem("Bigger Snowballs", new TreeItem("Bigger Snowballs", new HashSet<string> { "Snowball Stockpile", "Snowball Packing" }, new HashSet<string> { "Start" }, new string[] { "More mass, more damage.", "+20% Attack"}, 30, new Tuple<int, double>[] { new Tuple<int, double>(1, 1.20) }, new Vector2(Game.Resolution.X / 2 + 225, 200), id));
        addTreeItem("Snowball Stockpile", new TreeItem("Snowball Stockpile", new HashSet<string> { "Perfect Snowball" }, new HashSet<string> { "Bigger Snowballs" }, new string[] { "You start leaving piles of snowballs in your forts.", "+ Units can attack the turn they are recruited." }, 45, new Tuple<int, double>[] { new Tuple<int, double>(4, 1) }, new Vector2(Game.Resolution.X / 2 + 115, 300), id));
        addTreeItem("Snowball Packing", new TreeItem("Snowball Packing", new HashSet<string> { "Perfect Snowball" }, new HashSet<string> { "Bigger Snowballs" }, new string[] { "Pack the snowballs more densely, resulting in more damage.", "+20% Attack" }, 40, new Tuple<int, double>[] { new Tuple<int, double>(1, 1.20) }, new Vector2(Game.Resolution.X / 2 + 335, 300), id));
        addTreeItem("Perfect Snowball", new TreeItem("Perfect Snowball", new HashSet<string> { }, new HashSet<string> { "Snowball Packing", "Snowball Stockpile" }, new string[] { "Combining all your research, you get the perfect snowball.", "+50% Attack" }, 120, new Tuple<int, double>[] { new Tuple<int, double>(1, 1.50) }, new Vector2(Game.Resolution.X / 2 + 225, 400), id));
    }

    public List<string> save()
    {
        List<string> l = new List<string>();

        Utilities.write(ref l, id);
        Utilities.write(ref l, factionID);

        int c = techTreeItems.Count;
        Utilities.write(ref l, c);
        foreach (KeyValuePair<string, TreeItem> tt in techTreeItems)
        {
            Utilities.write(ref l, tt.Key);
            List<string> til = tt.Value.save();
            foreach (string str in til) l.Add(str);
        }

        return l;
    }
    public int load(ref List<string> l , int i)
    {
        Utilities.read(ref id, l[i++]);
        Utilities.read(ref factionID, l[i++]);

        int c = 0;
        Utilities.read(ref c, l[i++]);
        techTreeItems = new Dictionary<string, TreeItem>();
        for (int j = 0; j < c; j++)
        {
            string tts = "";
            TreeItem ti = new TreeItem("", new HashSet<string>(),
                new HashSet<string>(), new string[0], 0, new Tuple<int, double>[0],
                Vector2.Zero, -1);

            Utilities.read(ref tts, l[i++]);
            i = ti.load(ref l, i);

            techTreeItems.Add(tts, ti);
        }

        return i;
    }
}
