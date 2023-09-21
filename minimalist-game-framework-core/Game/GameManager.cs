using System;
using System.IO;
using System.Collections.Generic;

public static class GameManager
{
    /* Rendering related things */
    private static Vector2 mapDisplayOffset = new Vector2(200, 100);
    public static int defaultScale = 64;
    public static DynamicCamera cam;

    /*Misc.*/
    public static int turn = 1;
    public static int currentFaction = 0;
    public static int sceneNum = 0;
    public static int winner = -1;

    public static void reset()
    {
        turn = 1;
        currentFaction = 0;
        sceneNum = 0;
        winner = -1;

        SceneManager.reset();
        TechTreeManager.reset();
        TileStructureManager.reset();
        UnitManager.reset();
    }

    public static void initialize(float acceleration, float resistance, int mapHalfSize, Tuple<int, int> halfMapDisplay)
    {
        cam = new DynamicCamera(Vector2.Zero, acceleration, resistance, new Tuple<int, int>(-mapHalfSize, -mapHalfSize),
            new Tuple<int, int>(mapHalfSize, mapHalfSize), defaultScale, halfMapDisplay);
    }
    
    public static int getCurrentFaction()
    {
        return currentFaction;
    }

    public static void tick()
    {
        SceneManager.runScene(sceneNum);
        MenuManager.menuLoop();
    }

    public static void incrementTurn()
    {
        FactionManager.setLastCameraState(currentFaction, new Tuple<Vector2, int>(cam.getPosition(), cam.getScale()));

        MapManager.deselect(); TileStructureManager.deselect(); UnitManager.deselect();

        // find a playable faction
        do {
            currentFaction = (currentFaction + 1) % FactionManager.getFactionCount();
            if (currentFaction == 0)
            {
                turn++;
                UnitManager.resetMovementPoints();
            }
        } while (FactionManager.factions[getCurrentFaction()].lost);

        bool lostThisTurn = FactionManager.updateCondition(currentFaction);
        if (!lostThisTurn) FactionManager.changeCookies(currentFaction, FactionManager.getCookiesPerTurn(currentFaction));

        cam.setScale(FactionManager.getLastCameraState(currentFaction).Item2);
        cam.setPosition(FactionManager.getLastCameraState(currentFaction).Item1);

        MenuManager.antiScreenPeek(FactionManager.getFactionName(currentFaction), turn, FactionManager.getFactionColor(currentFaction), lostThisTurn);
        
        // check if the current player has won
        int playersPlaying = 0;
        for (int i = 0; i < FactionManager.factions.Length; i++)
        {
            if (!FactionManager.factions[i].lost) { playersPlaying++; }
        }
        if (playersPlaying <= 1 && !FactionManager.factions[GameManager.currentFaction].lost)
        {
            winner = currentFaction;
            GameManager.sceneNum = 2;
        }
    }

    static List<string> s = new List<string>();
    public static List<string> save()
    {
        s.Clear();

        Utilities.write(ref s, turn);
        Utilities.write(ref s, currentFaction);
        Utilities.write(ref s, sceneNum);

        List<string> camSave = cam.save();
        foreach (string str in camSave) s.Add(str);

        return s;
    }

    public static int load(ref List<string> l, int i)
    {
        Utilities.read(ref turn, l[i++]);
        Utilities.read(ref currentFaction, l[i++]);
        Utilities.read(ref sceneNum, l[i++]);

        i = cam.load(ref l, i);

        return i;
    }
}