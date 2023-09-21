using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;

public class UnitAttributes
{

    //What's the interval of generation for this type of tile?
    public int attackPoints;
    public int healthPoints;
    public int mobility;
    public int defencePoints;
    public int attackRange;
    public int cookieCost;

    public Action passiveEffects;

    //What's this unit's texture?
    public Texture texture;
    public String texturePath;

    public UnitAttributes(int attackPoints, int healthPoints, int mobility, int defencePoints,
        int attackRange, int cookieCost, Action pE, String texturePath)
    {
        this.attackPoints = attackPoints;
        this.healthPoints = healthPoints;
        this.mobility = mobility;
        this.defencePoints = defencePoints;
        this.attackRange = attackRange;
        this.passiveEffects = pE;
        this.cookieCost = cookieCost;
        this.texturePath = texturePath;

        this.texture = Engine.LoadTexture(texturePath);
    }

    public List<string> save()
    {
        List<string> l = new List<string>();

        Utilities.write(ref l, attackPoints);
        Utilities.write(ref l, healthPoints);
        Utilities.write(ref l, mobility);
        Utilities.write(ref l, defencePoints);
        Utilities.write(ref l, attackRange);
        Utilities.write(ref l, cookieCost);
        Utilities.write(ref l, texturePath);

        return l;
    }
    public int load(ref List<string> l, int i)
    {
        Utilities.read(ref attackPoints, l[i++]);
        Utilities.read(ref healthPoints, l[i++]);
        Utilities.read(ref mobility, l[i++]);
        Utilities.read(ref defencePoints, l[i++]);
        Utilities.read(ref attackRange, l[i++]);
        Utilities.read(ref cookieCost, l[i++]);
        Utilities.read(ref texturePath, l[i++]);
        texture = Engine.LoadTexture(texturePath);

        return i;
    }
}