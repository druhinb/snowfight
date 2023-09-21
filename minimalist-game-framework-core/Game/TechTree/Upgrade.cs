using System;

public class Upgrade {
    private int cost;

    public Upgrade(int cost)
    {
        this.cost = cost;
    }
}

public class Modifier : Upgrade
{
    public int attackPoints;
    public int healthPoints;
    public int mobility;
    public int defencePoints;
    public int attackRange;
    public int cookieCost;

    public Modifier(int cost) : base(cost) { }
}

public class AddMod : Modifier
{
    public AddMod(int attackPoints, int healthPoints, int mobility, int defencePoints,
        int attackRange, int cookieCost, int cost) : base(cost)
    {
        attackPoints = 0;
    }
}

// public class MultiplierMod : Modifier { }

public class EffectUpgrade : Upgrade
{
    Action passiveEffects;

    public EffectUpgrade(Action passiveEffects, int cost) : base(cost)
    {
        this.passiveEffects = passiveEffects;
    }
}