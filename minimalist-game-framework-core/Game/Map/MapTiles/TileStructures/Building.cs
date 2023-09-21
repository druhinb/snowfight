using System;
using System.Collections.Generic;

public class Building : TileStructure
{
    public Building(Tuple<int, int> position, string texturePath) : base(position, texturePath) { }

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
        Utilities.write(ref l, base.texturePath);

        return l;
    }
    public override int load(ref List<string> l, int i)
    {
        Utilities.read(ref base.location, l[i], l[i + 1]);
        i += 2;
        Utilities.read(ref base.texturePath, l[i++]);
        base.renderTex = Engine.LoadTexture(base.texturePath);

        return i;
    }
}