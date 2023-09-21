using System;
using System.Collections.Generic;

public class Unit
{
	int factionID;
	Units unitType;
	public UnitAttributes attributes;
	int mobility;
	Tuple<int, int> position;
	public bool canAttack = false;

	public Unit(int factionID, Units unitType, UnitAttributes attributes, Tuple<int, int> position)
	{
		this.factionID = factionID;
		this.unitType = unitType;
		this.attributes = new UnitAttributes(attributes.attackPoints, attributes.healthPoints, attributes.mobility, attributes.defencePoints,
		attributes.attackRange, attributes.cookieCost, attributes.passiveEffects, attributes.texturePath);
		this.mobility = attributes.mobility;
		this.position = position;
        if (FactionManager.factions[factionID].customTechs.Contains("spawnAttack"))
		{
			canAttack = true;
		}

    }

	// Get methods
	public int getFactionID() { return factionID; }
	public Units getUnitType() { return unitType; }
	public UnitAttributes getAttributes() { return attributes; }

	public int getMobility() 
	{ 
		return mobility;
	}

	public int getVision(Tuple<int, int> indices) 
	{ 
		return (int) (BiomeManager.getBiomeAttributes(MapManager.getBiome(indices)).visionModifier * Math.Max(attributes.mobility, attributes.attackRange));
	}

	public int getDefence(Tuple<int, int> indices) 
	{ 
		return (int) (BiomeManager.getBiomeAttributes(MapManager.getBiome(indices)).defenceModifier * attributes.defencePoints);
	}

	public int getAttack(Tuple<int, int> indices) 
	{ 
		return (int) (BiomeManager.getBiomeAttributes(MapManager.getBiome(indices)).attackModifier * attributes.attackPoints);
	}

	public Tuple<int, int> getPosition() { return position; }
	
	public void moveTo(Tuple<int, int> indices) 
	{
		FactionManager.moveFrom(this.factionID, this.position, getVision(this.position));
		position = indices; mobility = 0;
		FactionManager.moveTo(this.factionID, indices, getVision(indices));
	}
	public void setMobility(int newMobility) { mobility = newMobility; }
	public void resetMobility() { mobility = attributes.mobility; }
	public void attack() { canAttack = false; }
	public void resetAttack() { canAttack = true; }

	public bool takeDamage(int damage)
	{
		this.attributes.healthPoints -= (int) 
		(damage - (this.getDefence(this.position)));
		
		if (this.attributes.healthPoints <= 0)
			return true;
		return false;
	}

	// renders the unit, its health bar, and its move and attack indicators
	public void render(Vector2 pos, int scale)
	{
		Engine.DrawTexture(attributes.texture, pos, size: new Vector2(scale, scale), scaleMode: TextureScaleMode.Nearest);

		// health bar
		Engine.DrawTexture(MenuManager.hpBg, pos + new Vector2(0, (float)(scale * (15.0 / 24.0))), size: new Vector2((float)(scale * (20.0/24.0)), (float)(scale * (9.0/24.0))), scaleMode: TextureScaleMode.Nearest);
		Engine.DrawTexture(MenuManager.hpBar, 
			pos + new Vector2((float)(scale * (2.0 / 24.0)), 
			(float)(scale * (16.5 / 24.0))), 
			FactionManager.getFactionColor(factionID), 
			new Vector2((float)(scale * (((attributes.healthPoints * 1.0) / 
			(FactionManager.getFactionUnits(factionID).getUnitAttributes(unitType).healthPoints * 1.0) * 16.0) / 24.0)), 
			(float)(scale * (6.0 / 24.0))), scaleMode: TextureScaleMode.Nearest);

		// tactics indicators (only rendered on player's turn)
		if (factionID == GameManager.currentFaction)
		{
			if (mobility != 0)
			{
				Engine.DrawTexture(MenuManager.moveNote, pos, size: new Vector2(scale / 4, scale / 4), scaleMode: TextureScaleMode.Linear);
			}
			if (canAttack == true)
			{
				Engine.DrawTexture(MenuManager.attackNote, pos + new Vector2(scale * 3 / 4, 0), size: new Vector2(scale / 4, scale / 4), scaleMode: TextureScaleMode.Linear);
			}
		}
    }

	public List<string> save()
	{
		List<string> l = new List<string>();

		Utilities.write(ref l, factionID);
        Utilities.write(ref l, unitType);
		Utilities.write(ref l, mobility);
		Utilities.write(ref l, position);
		Utilities.write(ref l, canAttack);

		List<string> al = attributes.save();
		foreach (string str in al) l.Add(str);

		return l;
    }
	public int load(ref List<string> l, int i)
	{
		Utilities.read(ref factionID, l[i++]);
		Utilities.read(ref unitType, l[i++]);
		Utilities.read(ref mobility, l[i++]);
		Utilities.read(ref position, l[i], l[i + 1]);
		i += 2;
		Utilities.read(ref canAttack, l[i++]);

		i = attributes.load(ref l, i);

		return i;
	}
}