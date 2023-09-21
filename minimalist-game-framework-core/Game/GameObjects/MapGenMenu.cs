using System;
using System.Collections.Generic;

class MapGenMenu
{
    // saved vars for generating a game
    private static double seed = 0.1;
    public static int mapHalfSize = 20;
    private static double houseDensity = 0.6;
    private static int playerAmt = 2;
    private static double abundance = 0.6;

    // objects
    private static Image seedStr;
    private static Image mapHalfSizeStr;

    private static Image abundanceStr;
    private static Image houseDensityStr;
    private static Image playerAmtStr;

    private static Texture infoTex = Engine.LoadTexture("menu/infoIcon.png");

    private static Tuple<int, int> halfMapDisplay = new Tuple<int, int>(540, 310);
    private static Vector2 mapDisplayOffset = new Vector2(200, 100);

    public static void menu()
    {
        // components
        List<Image> images = new List<Image>();
        images.Add(new Image(new Vector2(300, 115), Vector2.Zero, null, "--- Map Settings ---"));

        images.Add(new Image(new Vector2(300, 150), Vector2.Zero, null, "- Seed -"));
        seedStr = (new Image(new Vector2(300, 175), Vector2.Zero, null, ((seed + 0.1) * 5).ToString()));
        images.Add(seedStr);
        images.Add(new Image(new Vector2(140, 170), new Vector2(30, 30), infoTex, mouseOverTextIn: new String[] { "Each number generates a unique map." }));

        images.Add(new Image(new Vector2(300, 200), Vector2.Zero, null, "- Map Size -"));
            mapHalfSizeStr = (new Image(new Vector2(300, 225), Vector2.Zero, null, (mapHalfSize * 2).ToString() + "x" + (mapHalfSize * 2).ToString()));
        images.Add(mapHalfSizeStr);
        images.Add(new Image(new Vector2(140, 220), new Vector2(30, 30), infoTex, mouseOverTextIn: new String[] { "Map size, in tiles." }));

        images.Add(new Image(new Vector2(300, 265), Vector2.Zero, null, "--- Faction Settings ---"));

        images.Add(new Image(new Vector2(300, 300), Vector2.Zero, null, "- Feature Abundance -"));
        abundanceStr = (new Image(new Vector2(300, 325), Vector2.Zero, null, abundance.ToString()));
        images.Add(abundanceStr);
        images.Add(new Image(new Vector2(140, 320), new Vector2(30, 30), infoTex, mouseOverTextIn: new String[] { "How many resources are placed on the map.", "More resources lead to larger and stronger armies." }));

        images.Add(new Image(new Vector2(300, 350), Vector2.Zero, null, "- House Density -"));
        houseDensityStr = (new Image(new Vector2(300, 375), Vector2.Zero, null, houseDensity.ToString()));
        images.Add(houseDensityStr);
        images.Add(new Image(new Vector2(140, 370), new Vector2(30, 30), infoTex, mouseOverTextIn: new String[] { "How many houses are placed on the map.", "More houses lead to less power differences and more map mobility." }));

        images.Add(new Image(new Vector2(300, 400), Vector2.Zero, null, "- Player Amount -"));
        playerAmtStr = (new Image(new Vector2(300, 425), Vector2.Zero, null, playerAmt.ToString()));
        images.Add(playerAmtStr);
        images.Add(new Image(new Vector2(140, 420), new Vector2(30, 30), infoTex, mouseOverTextIn: new String[] { "How many players are playing against each other." }));


        List<Button> buttons = new List<Button>();
        string[] startConfirmText = { "Start a new game with selected options." };
        buttons.Add(new Button(new Vector2(225, 520), new Vector2(150, 30), gameStart, "Start New Match", startConfirmText));

        buttons.Add(new Button(new Vector2(200, 170), new Vector2(30, 30), removeSeed, "-"));
        buttons.Add(new Button(new Vector2(370, 170), new Vector2(30, 30), addSeed, "+"));
        buttons.Add(new Button(new Vector2(430, 170), new Vector2(30, 30), rngSeed, "?"));
        buttons.Add(new Button(new Vector2(470, 170), new Vector2(30, 30), resetSeed, "1"));

        buttons.Add(new Button(new Vector2(200, 220), new Vector2(30, 30), removeSize, "-"));
        buttons.Add(new Button(new Vector2(370, 220), new Vector2(30, 30), addSize, "+"));

        buttons.Add(new Button(new Vector2(200, 320), new Vector2(30, 30), removeAbun, "-"));
        buttons.Add(new Button(new Vector2(370, 320), new Vector2(30, 30), addAbun, "+"));

        buttons.Add(new Button(new Vector2(200, 370), new Vector2(30, 30), removeDens, "-"));
        buttons.Add(new Button(new Vector2(370, 370), new Vector2(30, 30), addDens, "+"));

        buttons.Add(new Button(new Vector2(200, 420), new Vector2(30, 30), removePlayer, "-"));
        buttons.Add(new Button(new Vector2(370, 420), new Vector2(30, 30), addPlayer, "+"));


        // menus
        MenuManager.makeMenu(new Menu(new Vector2(50, 50), new Vector2(500, 550), "Game Setup", buttons, images));

        rngSeed();
    }

    // calls the game start method
    public static void gameStart()
    {
        GameManager.initialize(50, (float)(0.02), mapHalfSize, halfMapDisplay);
        MapManager.createMap(new Tuple<int, int>(2 * mapHalfSize, 2 * mapHalfSize), "perlin", false, seed,
            halfMapDisplay, mapDisplayOffset, houseDensity, abundance);

        FactionManager.initialize(playerAmt, new Tuple<int, int>(mapHalfSize, mapHalfSize));
        MenuLoader.gameStart();
        GameManager.sceneNum = 1;
    }

    /* world generation buttons */
    private static void addSeed()
    {
        seed = Utilities.round(seed + 0.2);
        if (seed >= 1999.9) seed = 1999.9;
        seedStr.setString(Math.Round(((seed + 0.1) * 5), 0).ToString());
    }

    private static void removeSeed()
    {
        seed = Utilities.round(seed - 0.2);
        if (seed <= 0.1) seed = 0.1;
        seedStr.setString(Math.Round(((seed + 0.1) * 5), 0).ToString());
    }

    private static void rngSeed()
    {
        Random random = new Random();
        int dispSeed = random.Next(10000);
        seed = (dispSeed / 5) - 0.1;
        seedStr.setString(Math.Round(((seed + 0.1) * 5), 0).ToString());
    }

    private static void resetSeed()
    {
        seed = 0.1;
        seedStr.setString(Math.Round(((seed + 0.1) * 5), 0).ToString());
    }

    private static void addSize()
    {
        mapHalfSize += 5;
        if (mapHalfSize > 50) mapHalfSize = 50;
        mapHalfSizeStr.setString((mapHalfSize * 2).ToString() + "x" + (mapHalfSize * 2).ToString());
    }
    private static void removeSize()
    {
        mapHalfSize -= 5;
        if (mapHalfSize < 15) mapHalfSize = 15;
        mapHalfSizeStr.setString((mapHalfSize * 2).ToString() + "x" + (mapHalfSize * 2).ToString());
    }

    private static void addAbun()
    {
        abundance = Utilities.round(abundance + 0.1);
        if (abundance > 1) abundance = 1; 
        abundanceStr.setString(abundance.ToString());
    }
    private static void removeAbun()
    {
        abundance = Utilities.round(abundance - 0.1);
        if (abundance < 0.1) abundance = 0.1;
        abundanceStr.setString(abundance.ToString());
    }

    private static void addDens()
    {
        houseDensity = Utilities.round(houseDensity + 0.1);
        if (houseDensity > 1) houseDensity = 1;
        houseDensityStr.setString(houseDensity.ToString());
    }
    private static void removeDens()
    {
        houseDensity = Utilities.round(houseDensity - 0.1);
        if (houseDensity < 0.1) houseDensity = 0.1;
        houseDensityStr.setString(houseDensity.ToString());
    }

    private static void addPlayer()
    {
        playerAmt += 1;
        if (playerAmt > 6) { playerAmt = 6; } 
        playerAmtStr.setString(playerAmt.ToString());
    }
    private static void removePlayer()
    { 
        playerAmt -= 1;
        if (playerAmt < 2) { playerAmt = 2; }
        playerAmtStr.setString(playerAmt.ToString());
    }
}
