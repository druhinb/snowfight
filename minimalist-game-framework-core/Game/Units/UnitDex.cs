using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Units
{
    KID = 0,
    TEEN = 1,
    ADULT = 2,
}

public class UnitDex
{
    //Lookup dict for per-unit properties 
    public Dictionary<Units, UnitAttributes> unitProperties =
        new Dictionary<Units, UnitAttributes>();

    public UnitDex() { defineUnits(); }

    public void updateUnitProperties(Units unit, UnitAttributes attributes)
    {
        //Add key-value pair to biome-attribute dictionary
        unitProperties.Add(unit, attributes);
    }

    //Defines unit properties 
    private void defineUnits()
    {
        updateUnitProperties(Units.KID, new UnitAttributes(120, 220, 2, 18, 2, 8, null, "units/kid.png"));
        updateUnitProperties(Units.TEEN, new UnitAttributes(160, 360, 3, 22, 3, 25, null, "units/teen.png"));
        updateUnitProperties(Units.ADULT, new UnitAttributes(240, 450, 3, 35, 3, 55, null, "units/adult.png"));
    }

    public UnitAttributes getUnitAttributes(Units unit)
    {
        unitProperties.TryGetValue(unit, out UnitAttributes u);

        return u;
    }

    public List<string> save()
    {
        List<string> l = new List<string>();

        int c = unitProperties.Count;
        Utilities.write(ref l, c);
        foreach (KeyValuePair<Units, UnitAttributes> k in unitProperties)
        {
            Utilities.write(ref l, k.Key);
            List<string> ul = k.Value.save();
            foreach (string str in ul) l.Add(str);
        }

        return l;
    }
    public int load(ref List<string> l, int i)
    {
        int c = 0;
        Utilities.read(ref c, l[i++]);
        unitProperties = new Dictionary<Units, UnitAttributes>();
        for (int j = 0; j < c; j++)
        {
            Units u = new Units();
            UnitAttributes ua = new UnitAttributes(0, 0, 0, 0, 0, 0, null, "art.png");

            Utilities.read(ref u, l[i++]);
            i = ua.load(ref l, i);

            unitProperties.Add(u, ua);
        }

        return i;
    }
}

