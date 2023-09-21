/*

using System;
using System.Collections.Generic;

public class CityManager
{
    private static List<City> cities = new List<City>(); 
    public static City selectedCity;
    public static int cityVision = 3;

    static CityManager(){}
    
    // turns a house into a fort for the given faction
    public static void incorporateTown(Town town, int factionID)
    {
        City city = new City(town.location, factionID);
        cities.Add(city);
        MapManager.setStructure(town.location, city);
        FactionManager.moveTo(factionID, town.location, cityVision);

        Engine.PlaySound(soundStorage.newCity);
    }

    // shows how many cookies a faction produces per turn
    public static void updateProduction()
    {
        foreach (City c in cities)
        {
            if (c != null) c.addProduction();
        }
    }
    
    // Manages the logic for selecting a unit and bringing up the relevant UI and such
    public static void selectCity(City city)
    {
        // Deselects the existing city
        deselect();
        selectedCity = city;
        MenuLoader.cityMenu(city);
        // bring up a menu with city UI 
    }

    // Deploys a unit on the given tile 
    public static bool buyUnit(UnitDex dex, Units unitT, City city)
    {
        // If there isn't already a unit on the tile
        if (MapManager.occupiedBy(city.location) == -1)
        {
            // If the faction can afford the unit
            if (FactionManager.getCookies(city.getFactionID()) >= dex.getUnitAttributes(unitT).cookieCost)
            {
                int unitID = UnitManager.createUnit(city.getFactionID(), unitT, city.location);
                UnitManager.setMobility(unitID, 0);
                FactionManager.changeCookies(city.getFactionID(), -dex.getUnitAttributes(unitT).cookieCost);
                Engine.PlaySound(soundStorage.newUnit);
                return true;
            }
        }
        return false;
    }

    public static void deselect()
    {
        if (selectedCity != null && MenuManager.getMenuAmt() > 1)
            MenuManager.closeMenu();
        selectedCity = null;
    }

    public static void drawCityBorders(int scale)
    {
        foreach(City c in cities)
            c.drawBorder(scale);
    }

    public static List<string> save()
    {
        List<string> l = new List<string>();

        int c = cities.Count;
        Utilities.write(ref l, c);
        for (int i = 0; i < c; i++)
        {
            Utilities.write(ref l, cities[i].location);
            List<string> cl = cities[i].save();
            foreach (string str in cl) l.Add(str);
        }
        Utilities.write(ref l, cityVision);

        return l;
    }
    public static int load(ref List<string> l, int i)
    {
        int c = 0;
        Utilities.read(ref c, l[i++]);
        for (int j = 0; j < c; j++)
        {
            Tuple<int, int> location = new Tuple<int, int>(0, 0);
            Utilities.read(ref location, l[i], l[i + 1]);
            i += 2;
            City city = new City(location);
            i = city.load(ref l, i);
        }
        Utilities.read(ref cityVision, l[i++]);

        return i;
    }
}

*/