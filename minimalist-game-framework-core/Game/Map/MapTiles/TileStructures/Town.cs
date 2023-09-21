using System;
using System.Collections.Generic;

public class Town : TileStructure
{

    public Town(Tuple<int, int> position) : base(position, "TileStructures/town.png") {}

    public override int getFactionID()
    {
        return -1;
    }

    public override void setFactionID(int factionID)
    {

    }

    public override List<string> save()
    {
        List<string> l = new List<string>();

        Utilities.write(ref l, base.location);

        return l;
    }
    public override int load(ref List<string> l, int i)
    {
        Utilities.read(ref base.location, l[i], l[i + 1]);
        i += 2;

        return i;
    }
}