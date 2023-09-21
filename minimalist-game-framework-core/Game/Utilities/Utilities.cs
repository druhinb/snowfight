using System;
using System.Collections.Generic;

static class Utilities{
    public static Vector2 mousePos = Engine.MousePosition;
    static Utilities(){}

    public static Vector2 getRealPos(DynamicCamera c, float posX, float posY)
    {
        return new Vector2(posX, posY) - c.getPosition();
    }

    public static float round(float num)
    {
        int inum = (int)(num);
        if (Math.Abs(num - inum) < 0.000001) return inum;
        inum++;
        if (Math.Abs(num - inum) < 0.000001) return inum;
        return num;
    }

    public static double round(double num)
    {
        int inum = (int)(num);
        if (Math.Abs(num - inum) < 0.000001) return inum;
        inum++;
        if (Math.Abs(num - inum) < 0.000001) return inum;
        return num;
    }

    public static void write(ref List<string> l, Bounds2 b)
    {
        write(ref l, b.Position);
        write(ref l, b.Size);
    }
    public static void read(ref Bounds2 b, string x1, string y1, string x2, string y2)
    {
        read(ref b.Position, x1, y1);
        read(ref b.Size, x2, y2);
    }

    public static void write(ref List<string> l, Biomes biome) { write(ref l, (int)(biome)); }
    public static void read(ref Biomes biome, string s)
    {
        int i = 0;
        read(ref i, s);
        biome = (Biomes)(i);
    }
    public static void write(ref List<string> l, Features feature) { write(ref l, (int)(feature)); }
    public static void read(ref Features feature, string s)
    {
        int i = 0;
        read(ref i, s);
        feature = (Features)(i);
    }
    public static void write(ref List<string> l, Units unitType) { write(ref l, (int)(unitType)); }
    public static void read(ref Units unitType, string s)
    {
        int i = 0;
        read(ref i, s);
        unitType = (Units)(i);
    }

    public static void write(ref List<string> l, Tuple<Vector2, int> t)
    {
        write(ref l, t.Item1);
        write(ref l, t.Item2);
    }
    public static void read(ref Tuple<Vector2, int> t, string x, string y, string z)
    {
        Vector2 v = Vector2.Zero;
        int i = 0;
        read(ref v, x, y);
        read(ref i, z);
        t = new Tuple<Vector2, int>(v, i);
    }

    public static void write(ref List<string> l, Tuple<int, double> t)
    {
        write(ref l, t.Item1);
        write(ref l, t.Item2);
    }
    public static void read(ref Tuple<int, double> t, string i, string d)
        { t = new Tuple<int, double>(int.Parse(i), double.Parse(d)); }

    public static void write(ref List<string> l, Vector2 v)
    {
        l.Add(v.X.ToString());
        l.Add(v.Y.ToString());
    }
    public static void read(ref Vector2 v, string x, string y)
        { v = new Vector2(float.Parse(x), float.Parse(y)); }

    public static void write(ref List<string> l, float f) { l.Add(f.ToString()); }
    public static void read(ref float f, string s) { f = float.Parse(s); }
    public static void write(ref List<string> l, double f) { l.Add(f.ToString()); }
    public static void read(ref double f, string s) { f = double.Parse(s); }
    public static void write(ref List<string> l, int i) { l.Add(i.ToString()); }
    public static void read(ref int i, string s) { i = int.Parse(s); }
    public static void write(ref List<string> l, bool b) { l.Add((b ? 1 : 0).ToString()); }
    public static void read(ref bool b, string s) { b = s == "1";  }
    public static void write(ref List<string> l, string s) { l.Add(s); }
    public static void read(ref string rs, string s) { rs = s; }
    // not sure that read bool works

    public static void write(ref List<string> l, Tuple<int, int> t)
    {
        l.Add(t.Item1.ToString());
        l.Add(t.Item2.ToString());
    }
    public static void read(ref Tuple<int, int> t, string i1, string i2)
        { t = new Tuple<int, int>(int.Parse(i1), int.Parse(i2)); }
}