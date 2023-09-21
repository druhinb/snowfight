using System.IO;
using System.Collections.Generic;

public static class SaveManager
{
    public static bool check_for_file(string fileName) { return File.Exists(fileName + ".txt"); }

    static List<string> save = new List<string>();
    public static void save_file(string fileName)
    {
        //File.OpenWrite(fileName + ".txt");
        save.Clear();

        List<string> cms = TileStructureManager.save();
        List<string> mms = MapManager.save();
        List<string> ttms = TechTreeManager.save();
        List<string> fms = FactionManager.save();
        List<string> ums = UnitManager.save();
        List<string> gms = GameManager.save();

        foreach (string str in cms) save.Add(str);
        foreach (string str in mms) save.Add(str);
        foreach (string str in ttms) save.Add(str);
        foreach (string str in fms) save.Add(str);
        foreach (string str in ums) save.Add(str);
        foreach (string str in gms) save.Add(str);

        string[] sa = new string[save.Count];
        for (int i = 0; i < save.Count; i++)
        {
            sa[i] = save[i];
        }

        File.WriteAllLines(fileName + ".txt", sa);
    }
    public static void load_file(string fileName)
    {
        List<string> save = new List<string>();

        string[] sa = new string[0];
        sa = File.ReadAllLines(fileName + ".txt");
        for (int j = 0; j < sa.Length; j++) save.Add(sa[j]);

        int i = 0;
        i = TileStructureManager.load(ref save, i);
        i = MapManager.load(ref save, i);
        i = TechTreeManager.load(ref save, i);
        i = FactionManager.load(ref save, i);
        i = UnitManager.load(ref save, i);
        i = GameManager.load(ref save, i);
    }
}