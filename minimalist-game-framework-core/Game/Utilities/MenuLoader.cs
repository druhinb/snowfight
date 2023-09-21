/*
* Kirill Obraztsov
* 
* A system for loading menus 
* 
* 11/22/2022
* 
*/
using System;
using System.Collections.Generic;
using System.Threading;

class MenuLoader
{
    private static Texture closeTex = Engine.LoadTexture("menu/testTex.png");
    private static Texture menuBgTex = Engine.LoadTexture("menu/menuTex.png");
    private static Texture infoTex = Engine.LoadTexture("menu/infoIcon.png");
    private static Texture bgTex = Engine.LoadTexture("menu/bg.png");
    private static Texture titleTex = Engine.LoadTexture("menu/title.png");
    private static Texture cookieTex = Engine.LoadTexture("menu/cookie.png");

    private static Texture mapTex = Engine.LoadTexture("menu/map.png");
    private static Texture techTex = Engine.LoadTexture("menu/tech.png");
    private static Texture victoryTex = Engine.LoadTexture("menu/victory.png");

    private static Texture hillTex = Engine.LoadTexture("tile/hilltile.png");
    private static Texture lakeTex = Engine.LoadTexture("tile/icetile.png");
    private static Texture forestTex = Engine.LoadTexture("tile/feature/forest.png");

    private static Texture dotTex = Engine.LoadTexture("dot.png");
    private static Texture techBg = Engine.LoadTexture("menu/techBg.png");
    
    private static Texture techHasI = Engine.LoadTexture("menu/techHas.png");
    private static Texture techBudgetedI = Engine.LoadTexture("menu/techBudgeted.png");
    private static Texture techUnlockedI = Engine.LoadTexture("menu/techUnlocked.png");
    private static Texture techLockedI = Engine.LoadTexture("menu/techLocked.png");

    private static ResizableTexture techHas = Engine.LoadResizableTexture("menu/techHas.png", 4, 4, 4, 4);
    private static ResizableTexture techBudgeted = Engine.LoadResizableTexture("menu/techBudgeted.png", 4, 4, 4, 4);
    private static ResizableTexture techUnlocked = Engine.LoadResizableTexture("menu/techUnlocked.png", 4, 4, 4, 4);
    private static ResizableTexture techLocked = Engine.LoadResizableTexture("menu/techLocked.png", 4, 4, 4, 4);
    private static ResizableTexture[] techTexes = { techHas, techBudgeted, techUnlocked, techLocked };

    // toolbar components
    private static List<Image> toolbarImages = new List<Image>();
    private static Menu toolbar;

    // sounds
    public static int volume = 50;
    public static Boolean muted = false;
    public static Texture soundOn = Engine.LoadTexture("menu/soundOn.png");
    public static Texture soundOff = Engine.LoadTexture("menu/soundOff.png");
    public static Image volImage = new Image(new Vector2(Game.Resolution.X - 940, 55), Vector2.Zero, textIn: volume.ToString());
    public static Image muteImage = new Image(new Vector2(Game.Resolution.X - 880, 50), new Vector2(30, 30), soundOn);

    // main menu
    public static void mainMenu()
    {
        // components
        string[] motGenerate = { "Adjust generation settings,", "and then start a new match." };
        string[] motLoad = { "Load the last saved game." };
        string[] motOptions = { "Adjust window options." };
        string[] motInfo = { "Instructions are viewable in-game in the quick reference sidebar." };
        string[] motCredits = { "View the people who brought you the game,", "and other credits for used work." };
        string[] motExit = { "Leave the game." };

        List<Button> buttons = new List<Button>();
        buttons.Add(new Button(new Vector2(Game.Resolution.X / 2 - 100, Game.Resolution.Y / 2 + 80), new Vector2(200, 30), openGeneration, "Generate New Game", motGenerate));
        buttons.Add(new Button(new Vector2(Game.Resolution.X / 2 - 100, Game.Resolution.Y / 2 + 130), new Vector2(200, 30), load, "Load Game From File", motLoad));
        buttons.Add(new Button(new Vector2(Game.Resolution.X / 2 - 100, Game.Resolution.Y / 2 + 180), new Vector2(200, 30), openCredits, "Credits", motCredits));
        buttons.Add(new Button(new Vector2(Game.Resolution.X / 2 - 100, Game.Resolution.Y / 2 + 230), new Vector2(200, 30), exit, "Exit Game", motExit));

        buttons.Add(new Button(new Vector2(Game.Resolution.X - 880, 50), new Vector2(30, 30), mute, " ", isInvisIn: true));
        buttons.Add(new Button(new Vector2(Game.Resolution.X - 990, 50), new Vector2(30, 30), removeSound, "-"));
        buttons.Add(new Button(new Vector2(Game.Resolution.X - 920, 50), new Vector2(30, 30), addSound, "+"));



        List<Image> images = new List<Image>();
        images.Add(new Image(new Vector2(Game.Resolution.X / 2 - 300, Game.Resolution.Y / 2 - 225), new Vector2(600, 225), titleTex, ""));
        images.Add(volImage);
        images.Add(muteImage);

        // menus
        MenuManager.makeMenu(new Menu(Vector2.Zero, Game.Resolution, "Main Menu", buttons, images), true);
    }

    // actions used by buttons
    private static void actNull()
    {

    }

    // close special menus
    private static void closeSpec()
    {
        MapManager.deselect(); UnitManager.deselect(); TileStructureManager.deselect();
    }
    
    //close normal menus
    private static void forceClose()
    {
        MenuManager.closeAllMenus();
    }
    // credits screen
    private static void openCredits()
    {
        // components
        List<Image> images = new List<Image>();
        images.Add(new Image(new Vector2(250, 100), Vector2.Zero, null, "--- Credits ---"));
        images.Add(new Image(new Vector2(250, 130), Vector2.Zero, null, "Developer - Druhin Bhowal"));
        images.Add(new Image(new Vector2(250, 150), Vector2.Zero, null, "Developer - Kirill Obraztsov"));
        images.Add(new Image(new Vector2(250, 170), Vector2.Zero, null, "Developer - Hongrui Fan"));
        images.Add(new Image(new Vector2(250, 190), Vector2.Zero, null, "Developer - Chen Ziang"));

        images.Add(new Image(new Vector2(250, 250), Vector2.Zero, null, "Background Music - Guild of Ambience"));

        // menus
        MenuManager.makeMenu(new Menu(new Vector2(50, 50), new Vector2(400, 300), "Credits", null, images));

    }
    private static void openGeneration()
    {
        MapGenMenu.menu();
    }

    public static void gameStart()
    {
        // close all the open menus
        forceClose();

        // components
        List<Button> buttons = new List<Button>();
        buttons.Add(new Button(new Vector2(Game.Resolution.X - 200, 50), new Vector2(150, 30), endTurn, "End Turn"));
        buttons.Add(new Button(new Vector2(Game.Resolution.X - 400, 50), new Vector2(150, 30), techTree, "Tech Tree"));
        buttons.Add(new Button(new Vector2(Game.Resolution.X - 600, 50), new Vector2(150, 30), saveConfirm, "Save and Quit"));
        buttons.Add(new Button(new Vector2(Game.Resolution.X - 800, 50), new Vector2(150, 30), concedeConfirm, "Concede"));
        buttons.Add(new Button(new Vector2(Game.Resolution.X - 880, 50), new Vector2(30, 30), mute, " ", isInvisIn: true));

        buttons.Add(new Button(new Vector2(Game.Resolution.X - 990, 50), new Vector2(30, 30), removeSound, "-"));
        buttons.Add(new Button(new Vector2(Game.Resolution.X - 920, 50), new Vector2(30, 30), addSound, "+"));

        buttons.Add(new Button(new Vector2(0, Game.Resolution.Y - 200), new Vector2(200, 200), minimapClick, isInvisIn: true));

        // menus
        toolbar = new Menu(Vector2.Zero, new Vector2(Game.Resolution.X, 100), "Toolbar", buttons, toolbarImages);
        MenuManager.makeMenu(toolbar, true);

        MenuManager.antiScreenPeek(FactionManager.getFactionName(GameManager.currentFaction), GameManager.turn, FactionManager.getFactionColor(GameManager.currentFaction), false);
        
        Engine.DrawRectSolid(
                new Bounds2(new Vector2(0, Game.Resolution.Y - 200), 
                size: new Vector2(200, 200)),
                new Color(0, 0, 0));
    }

    // updates the toolbar 
    public static void updateToolbar(int factionID)
    {
        // only during gameplay
        if (GameManager.sceneNum == 1)
        {
            // hover-over explanations
            string[] cookieExp = { "The total amount of cookies you have for spending." };

            string[] moveExp = { "Units with this icon can move this turn.", "", "[Left Click] on a unit to select it.", 
                                 "[Right Click] on a highlighted square to move the unit.", "",
                                 "Moving any distance will prevent the unit from moving again this turn.", "Units can attack after moving."};
            string[] attackExp = { "Units with this icon can attack this turn.", "", "[Left Click] on a unit to select it.", 
                                   "[Right Click] on a highlighted square to attack it.", "", "Attacking prevents the unit from moving.",
                                   "Units can attack every turn, even if they already moved this turn."};
            string[] playerExp = { "You are " + FactionManager.getFactionName(GameManager.getCurrentFaction()) + "." };

            string[] townExp = { "An uncontested house on the map.", "Walk over this building with any unit to turn it into a faction fort."};
            string[] fortExp = { "Snow forts belong to a faction, and are colored to indicate this.", "Walking over an enemy fort will convert it into your fort.",
                                 "Produces one cookie every turn.", "Clicking on a fort allows you to spend cookies to recruit units at that fort.", 
                                 "The fort's tile must be empty to recruit a unit."};
            string[] landExp = { "Forts control land around themselves, drawing boundaries on the map.", 
                                 "Resources within these borders produce cookies, and can be improved to produce even more cookies.",
                                 "This land also provides map vision, allowing you to spot enemies before they apporach.", 
                                 "However, land boundaries are also visible through fog of war, allowing you to get a general feel of where your opponents are."};

            string[] mapExp = { "Map Controls:", "", "To look around the map, use right click, arrow keys, or WASD.", "Zoom in and out with + and - keys."};
            string[] techExp = { "Tech Trees:", "", "Tech trees allow you to purchase new technologies for your entire faction.", 
                                 "To purchase each technology, you need to have all pre-requisites and pay the cookie cost.",
                                 "Some technologies provide stat upgrades, while others will unlock passive abilities." };
            string[] victoryExp = { "Victory:", "", "When units recieve enough cold buildup from attacks or the environment, they will leave the field.",
                                    "This allows members of your faction to approach and take over opposing forts.",
                                    "When all opposing units and forts have been eliminated, you win the game!"};

            string[] hillExp = { "When your units are on a hill tile, they get the following effects:", "+40% Armor", "+50% Vision Range"};
            string[] lakeExp = { "When your units are on an ice lake tile, they get the following effects:", "-50% Armor"};
            string[] forestExp = { "When your units are on a forest or sawmill tile, they get the following effects:", "-25% Attack", "+100% Armor", "-50% Vision Range" };

            // resets the toolbar to update images on it
            toolbarImages.Clear();
            toolbarImages.Add(new Image(new Vector2(0, 100), new Vector2(200, Game.Resolution.Y - 300), texResIn: MenuManager.menuTex));
            toolbarImages.Add(new Image(new Vector2(50, 44), new Vector2(36, 36), cookieTex, mouseOverTextIn: cookieExp));
            toolbarImages.Add(new Image(new Vector2(150, 53), Vector2.Zero,
                textIn: Math.Round(FactionManager.getCookies(factionID), 1).ToString() + " + " +
                        Math.Round(FactionManager.getCookiesPerTurn(factionID), 1)));

            if (muted) { toolbarImages.Add(new Image(new Vector2(Game.Resolution.X - 880, 50), new Vector2(30, 30), soundOff)); }
            else { toolbarImages.Add(new Image(new Vector2(Game.Resolution.X - 880, 50), new Vector2(30, 30), soundOn)); }

            toolbarImages.Add(new Image(new Vector2(Game.Resolution.X - 940, 55), Vector2.Zero, textIn: volume.ToString()));
            //toolbarImages.Add(volImage);

            // quick reference sidebar menu
            toolbarImages.Add(new Image(new Vector2(100, 150), Vector2.Zero, textIn: "- Quick Reference -"));
            toolbarImages.Add(new Image(new Vector2(100, 170), Vector2.Zero, textIn: "(Hover over icons"));
            toolbarImages.Add(new Image(new Vector2(103, 187), Vector2.Zero, textIn: "for explanations)"));

            toolbarImages.Add(new Image(new Vector2(100, 215), Vector2.Zero, textIn: "Map Controls"));

            toolbarImages.Add(new Image(new Vector2(36, 235), new Vector2(32, 32), mapTex, mouseOverTextIn: mapExp));
            toolbarImages.Add(new Image(new Vector2(84, 235), new Vector2(32, 32), techTex, mouseOverTextIn: techExp));
            toolbarImages.Add(new Image(new Vector2(132, 235), new Vector2(32, 32), victoryTex, mouseOverTextIn: victoryExp));

            toolbarImages.Add(new Image(new Vector2(36, 275), new Vector2(32, 32), MenuManager.moveNote, mouseOverTextIn: moveExp));
            toolbarImages.Add(new Image(new Vector2(84, 275), new Vector2(32, 32), MenuManager.attackNote, mouseOverTextIn: attackExp));
            toolbarImages.Add(new Image(new Vector2(132, 275), new Vector2(32, 32), MenuManager.hpBar, mouseOverTextIn: playerExp, factionOverwrite: true));

            toolbarImages.Add(new Image(new Vector2(100, 320), Vector2.Zero, textIn: "Structure Explanations"));

            toolbarImages.Add(new Image(new Vector2(36, 340), new Vector2(32, 32), TileStructure.town, mouseOverTextIn: townExp));
            toolbarImages.Add(new Image(new Vector2(84, 340), new Vector2(32, 32), TileStructure.city, mouseOverTextIn: fortExp));
            toolbarImages.Add(new Image(new Vector2(132, 340), new Vector2(32, 32), TileStructure.land, mouseOverTextIn: landExp));

            toolbarImages.Add(new Image(new Vector2(100, 385), Vector2.Zero, textIn: "Tile Modifiers"));

            toolbarImages.Add(new Image(new Vector2(36, 405), new Vector2(32, 32), hillTex, mouseOverTextIn: hillExp));
            toolbarImages.Add(new Image(new Vector2(84, 405), new Vector2(32, 32), lakeTex, mouseOverTextIn: lakeExp));
            toolbarImages.Add(new Image(new Vector2(132, 405), new Vector2(32, 32), forestTex, mouseOverTextIn: forestExp));

            toolbar.resetImages();
            toolbar.setImages(toolbarImages);          
        }

        
        
    }

    private static void mute()
    {
        if(!muted)
        {
            SDL2.SDL_mixer.Mix_Volume(-1, volume);
            SDL2.SDL_mixer.Mix_PauseMusic();
        }
        else
        {
            SDL2.SDL_mixer.Mix_Volume(-1, 0);
            SDL2.SDL_mixer.Mix_ResumeMusic();
        }
        muted = !muted;
    }

    private static void addSound()
    {
        volume += 10;
        if (volume > 100) volume = 100;

        SDL2.SDL_mixer.Mix_Volume(-1, volume);
        SDL2.SDL_mixer.Mix_VolumeMusic(volume);
    }
    private static void removeSound()
    {
        volume = volume -= 10;
        if (volume < 0) volume = 0;

        SDL2.SDL_mixer.Mix_Volume(-1, volume);
        SDL2.SDL_mixer.Mix_VolumeMusic(volume);
    }

    private static void saveConfirm()
    {
        MenuManager.blocking = true;

        // components
        List<Image> images = new List<Image>();
        List<Button> buttons = new List<Button>();
        images.Add(new Image(new Vector2(800, 100), Vector2.Zero, null, "Save the game?"));
        images.Add(new Image(new Vector2(800, 150), Vector2.Zero, null, "This will override your previous save files."));
        images.Add(new Image(new Vector2(800, 180), Vector2.Zero, null, "Quicksaving will make a save and let you keep playing,"));
        images.Add(new Image(new Vector2(800, 210), Vector2.Zero, null, "while Save and Quit will save and let you exit the game."));

        buttons.Add(new Button(new Vector2(630, 280), new Vector2(150, 30), MenuManager.closeBlocking, "Cancel"));
        buttons.Add(new Button(new Vector2(820, 280), new Vector2(150, 30), quickSave, "Quicksave"));
        buttons.Add(new Button(new Vector2(630, 340), new Vector2(150, 30), quit, "Quit w/o Saving"));
        buttons.Add(new Button(new Vector2(820, 340), new Vector2(150, 30), saveAndQuit, "Save and Quit"));

        // menus
        MenuManager.makeMenu(new Menu(new Vector2(550, 50), new Vector2(500, 400), "Confirm Save", buttons, images), true);
    }

    private static void quickSave()
    {
        save();
        MenuManager.closeBlocking();
    }
    private static void saveAndQuit()
    {
        save();
        GameManager.reset();
    }
    private static void quit()
    {
        MenuManager.closeBlocking();
        GameManager.reset();
    }

    private static void concedeConfirm()
    {
        MenuManager.blocking = true;

        // components
        List<Image> images = new List<Image>();
        List<Button> buttons = new List<Button>();
        images.Add(new Image(new Vector2(350, 100), Vector2.Zero, null, "Are you sure you wish to concede?"));
        images.Add(new Image(new Vector2(350, 150), Vector2.Zero, null, "Your houses and units will be deleted,"));
        images.Add(new Image(new Vector2(350, 180), Vector2.Zero, null, "and you will be unable to play or spectate."));

        buttons.Add(new Button(new Vector2(220, 250), new Vector2(60, 30), MenuManager.closeBlocking, "No"));
        buttons.Add(new Button(new Vector2(420, 250), new Vector2(60, 30), concedeExecute, "Yes"));

        // menus
        MenuManager.makeMenu(new Menu(new Vector2(150, 50), new Vector2(400, 300), "Confirm Concede Action", buttons, images), true);
    }

    private static void concedeExecute()
    {
        MenuManager.closeBlocking();
        FactionManager.factions[GameManager.getCurrentFaction()].lost = true;

        List<int> IDs = new List<int>();
        foreach (int i in UnitManager.getUnits().Keys)
        {
            IDs.Add(i);
        }
        foreach (int i in IDs) { 
            if (UnitManager.getUnits()[i].getFactionID() == GameManager.getCurrentFaction())
            {
                UnitManager.destroyUnit(i, UnitManager.getUnits()[i].getPosition());
            }
        }

        endTurn();
    }

    public static void renderMinimap()
    {
        float xScale = (float)(200) / MapManager.width;
        float yScale = (float)(200) / MapManager.height;
        Tuple<int, int> indices = new Tuple<int, int>(0,0);

        Engine.DrawRectSolid(
            new Bounds2(new Vector2(0, Game.Resolution.Y - 200), new Vector2(200, 200)), Color.Black);

        for (int i = 0; i < MapManager.width; i++)
        {
            for (int j = 0; j < MapManager.height; j++)
            {
                indices = new Tuple<int, int> (i, j);
                
                if (FactionManager.inVision(GameManager.currentFaction, indices))
                {
                    Engine.DrawRectSolid(
                        new Bounds2(new Vector2(i * (xScale), Game.Resolution.Y - 200 + (j * (yScale))), 
                        size: new Vector2((int)(xScale) + 1, (int)(yScale) + 1)),
                    BiomeManager.getBiomeAttributes(MapManager.getBiome(indices)).minimapColor);

                    if (TileStructureManager.getStructureType(MapManager.getStructureID(indices)) == typeof(City))
                    {
                        Engine.DrawTexture(dotTex, 
                            new Vector2(i * (xScale), Game.Resolution.Y - 200 + (j * (yScale))),
                            Color.Black, size: new Vector2((int)(xScale) + 1, (int)(yScale) + 1), scaleMode: TextureScaleMode.Linear);
                    }
                    
                    if (MapManager.occupiedBy(indices) != -1)
                    {   
                        Engine.DrawTexture(dotTex, 
                            new Vector2(i * (xScale), Game.Resolution.Y - 200 + (j * (yScale))),
                            FactionManager.getFactionColor(UnitManager.getFactionID(MapManager.occupiedBy(indices))), 
                            size: new Vector2((int)(xScale) + 1, (int)(yScale) + 1), scaleMode: TextureScaleMode.Linear);
                    }
                    
                    if (MapManager.getFactionID(indices) != -1)
                    {
                        Color temp = FactionManager.getFactionColor(MapManager.getFactionID(indices));
                        temp.A = 100;

                        Engine.DrawRectSolid(
                            new Bounds2(new Vector2(i * (xScale), Game.Resolution.Y - 200 + (j * (yScale))), 
                            size: new Vector2((int)(xScale) + 1, (int)(yScale) + 1)), temp);
                    }
                }
                else if (FactionManager.explored(GameManager.currentFaction, indices))
                {
                    Engine.DrawRectSolid(
                        new Bounds2(new Vector2(i * (xScale), Game.Resolution.Y - 200 + (j * (yScale))), 
                        size: new Vector2((int)(xScale) + 1, (int)(yScale) + 1)),
                    BiomeManager.getBiomeAttributes(MapManager.getBiome(indices)).minimapColor);
                    
                    Engine.DrawRectSolid(
                        new Bounds2(new Vector2(i * (xScale), Game.Resolution.Y - 200 + (j * (yScale))), 
                        size: new Vector2((int)(xScale) + 1, (int)(yScale) + 1)),
                    new Color(0, 0, 0, 200));
                }
                else
                {
                    Engine.DrawRectSolid(
                        new Bounds2(new Vector2(i * (xScale), Game.Resolution.Y - 200 + (j * (yScale))), 
                        size: new Vector2((int)(xScale) + 1, (int)(yScale) + 1)), Color.Black);
                }
            }
        }

    }

    private static void minimapClick()
    {
       int x =  (int) Engine.MousePosition.X / (200 /MapManager.width);
       int y =  (int) (Engine.MousePosition.Y - (Game.Resolution.Y - 200)) / (200 / MapManager.height);
       
       GameManager.cam.setPosition(MapManager.getCoordinates(new Tuple<int, int>(x, y)));
    }

    public static void endScreen()
    {
        MenuManager.closeAllMenus();

        List<Button> buttons = new List<Button>();
        List<Image> images = new List<Image>();

        images.Add(new Image(new Vector2(60, 92), new Vector2(140, 30), MenuManager.hpBar, factionOverwrite: true));
        images.Add(new Image(new Vector2(275, 100), Vector2.Zero, textIn: "Winner: " + FactionManager.getFactionName(GameManager.winner)));

        for (int i = 0; i < FactionManager.getFactionCount(); i++)
        {
            images.Add(new Image(new Vector2(200, i * 50 + 150), Vector2.Zero, 
            textIn: FactionManager.getFactionName(i) + " ended the game with " + 
            FactionManager.getCookies(i).ToString() + " cookies." ));
        }

        buttons.Add(new Button(new Vector2(30, Game.Resolution.Y - 100), new Vector2(200, 30), openCredits, "Credits"));
        buttons.Add(new Button(new Vector2(250, Game.Resolution.Y - 100), new Vector2(200, 30), GameManager.reset, "Play Again"));
        buttons.Add(new Button(new Vector2(470, Game.Resolution.Y - 100), new Vector2(200, 30), exit, "Exit Game"));

        MenuManager.makeMenu(new Menu(Vector2.Zero, Game.Resolution, "End Screen", buttons, images), true);
    }

    // needs more work, currently creates a broken gamestate on load
    private static void playAgain()
    {
        //GameManager.sceneNum = 0;
        //MenuManager.closeAllMenus();
        //SceneManager.initialize();
    }

    private static void exit()
    {
        Environment.Exit(1);
    }

    // ends the current player's turn
    public static void endTurn()
    {
        Engine.PlaySound(soundStorage.endTurn);
        GameManager.incrementTurn();
    }

    // opens tech tree
    public static void techTree()
    {
        MenuManager.blocking = true;
        List<Button> buttons = new List<Button>();
        List<Image> images = new List<Image>();

        images.Add(new Image(new Vector2(10, 32), new Vector2(1260, 678), techBg));

        string[] cookieExp = { "The total amount of cookies you have for spending." };
        images.Add(new Image(new Vector2(50, 44), new Vector2(36, 36), cookieTex, mouseOverTextIn: cookieExp));
        images.Add(new Image(new Vector2(150, 53), Vector2.Zero,
            textIn: Math.Round(FactionManager.getCookies(GameManager.currentFaction), 1).ToString() + " + " + FactionManager.getCookiesPerTurn(GameManager.currentFaction)));

        images.Add(new Image(new Vector2(Game.Resolution.X / 2 - 16 - 450, 500), new Vector2(32, 32), techHasI));
        images.Add(new Image(new Vector2(Game.Resolution.X / 2 - 16 - 150, 500), new Vector2(32, 32), techBudgetedI));
        images.Add(new Image(new Vector2(Game.Resolution.X / 2 - 16 + 150, 500), new Vector2(32, 32), techUnlockedI));
        images.Add(new Image(new Vector2(Game.Resolution.X / 2 - 16 + 450, 500), new Vector2(32, 32), techLockedI));

        images.Add(new Image(new Vector2(Game.Resolution.X / 2 - 16 - 450, 560), new Vector2(32, 32), textIn: "Techs that you have unlocked"));
        images.Add(new Image(new Vector2(Game.Resolution.X / 2 - 16 - 150, 560), new Vector2(32, 32), textIn: "Techs that you can afford"));
        images.Add(new Image(new Vector2(Game.Resolution.X / 2 - 16 + 150, 560), new Vector2(32, 32), textIn: "Techs that you cannot afford"));
        images.Add(new Image(new Vector2(Game.Resolution.X / 2 - 16 + 450, 560), new Vector2(32, 32), textIn: "Techs that you are missing requirements for"));

        foreach (TreeItem t in TechTreeManager.getTreeItems(FactionManager.factions[GameManager.getCurrentFaction()].techTreeID))
        {
            int texID = t.calcTex(FactionManager.getCookies(GameManager.getCurrentFaction()));
            ResizableTexture tex = techTexes[texID];
            buttons.Add(new Button(t.getPos(), new Vector2(150, 30), t.unlockSelf, t.getName(), t.getDesc(), tex, MenuManager.buttonHoverTex));
        }

        buttons.Add(new Button(new Vector2(Game.Resolution.X - 400, 50), new Vector2(150, 30), MenuManager.closeBlocking, "Exit Tech Tree"));

        MenuManager.makeMenu(new Menu(Vector2.Zero, Game.Resolution, "Tech Tree", buttons, images), true);
    }

    public static void save()
    {
        SaveManager.save_file("save_file");
    }

    public static void load()
    {
        if (!SaveManager.check_for_file("save_file")) return;

        MapGenMenu.gameStart();
        SceneManager.init_game();
        SaveManager.load_file("save_file");
    }

    public static void buyUnit(Units u)
    {
        TileStructureManager.buyUnit(u);
    }

    public static void improveTile(Features feature)
    {
        if (FactionManager.getCookies(GameManager.currentFaction) >= FeatureManager.getFeatureAttributes(feature).improvementCost)
        {
            FactionManager.changeCookies(GameManager.currentFaction, -FeatureManager.getFeatureAttributes(feature).improvementCost);
            FactionManager.changeCookiesPerTurn(GameManager.currentFaction, FeatureManager.getFeatureAttributes(feature).improvedProductivity -
                FeatureManager.getFeatureAttributes(feature).unimprovedProductivity);
            MapManager.setStructure(MapManager.selectedTile, TileStructureManager.createBuilding(MapManager.selectedTile,
                FeatureManager.getFeatureAttributes(feature).improvedTexturePath));
            MapManager.improve(MapManager.selectedTile);

            MenuManager.closeMenu(); 
            inspectTile(MapManager.selectedTile);
        }
    }

    public static void cityMenu(int cityID)
    {
        List<Action> methods = new List<Action>{deployKid, deployTeen, deployAdult};
        List<Button> buttons = new List<Button>();
        List<Image> images = new List<Image>();
        UnitAttributes temp;

        int i = 0;
        
        foreach (Units u in FactionManager.getFactionUnits(TileStructureManager.getFactionID(cityID)).unitProperties.Keys)
        {
            temp = FactionManager.getFactionUnits(TileStructureManager.getFactionID(cityID)).getUnitAttributes(u);
            buttons.Add(new Button(new Vector2((i / 5) * 95 + 50, 150 + (((Game.Resolution.Y - 100) / 5) * ((i % 5)))),
                                   new Vector2(100, 30), 
                                   methods[i], u.ToString(), 
                                   new String[]{"Cost: " + temp.cookieCost.ToString() + " cookies", "", temp.attackPoints.ToString() + " Attack",
                                                temp.healthPoints.ToString() + " Health", temp.defencePoints.ToString() + " Armor",
                                                temp.mobility.ToString() + " Movement", temp.attackRange.ToString() + " Attack Range"}));
            i++;
        }

        images.Add(new Image(new Vector2(200, 0), new Vector2(Game.Resolution.X - 200, 100), texResIn: MenuManager.menuTex));
        buttons.Add(new Button(new Vector2(350, 50), new Vector2(150, 30), closeSpec, "Close Menu"));

        MenuManager.makeMenu(new Menu(new Vector2(0, 100), new Vector2(200, Game.Resolution.Y - 100), "Deploy Units", buttons, images), true);
    }   



    // opens a submenu for the current tile
    public static void inspectTile(Tuple<int, int> tileIndices)
    {   
        List<Image> images = new List<Image>();
        List<Button> buttons = new List<Button>();

        images.Add(new Image(new Vector2(100, 175), Vector2.Zero, infoTex, "Tile Type: " + MapManager.getBiome(tileIndices).ToString()));
        images.Add(new Image(new Vector2(100, 195), Vector2.Zero, infoTex, "Tile Productivity: " + MapManager.getProductivity(tileIndices)));
        images.Add(new Image(new Vector2(100, 215), Vector2.Zero, infoTex, "Improved: " + (MapManager.isImproved(tileIndices) ? "yes":"no")));

        if (!MapManager.isImproved(tileIndices))
        {
            Features feature = MapManager.getFeature(tileIndices);

            if(feature != default(Features))
            {
                Engine.PlaySound(FeatureManager.getFeatureAttributes(feature).sound);
            }

            if (feature == Features.WHEAT)
                buttons.Add(new Button(new Vector2(57, 245), new Vector2(85, 85), constructFarm, "Farm", 
                new String[]{ "Construct improvement on this tile:", FeatureManager.getFeatureAttributes(feature).improvementCost.ToString() + " cookies",
                              "When improved: " + FeatureManager.getFeatureAttributes(feature).improvedProductivity.ToString() + " cookies per turn"}));
            else if (feature == Features.COCOA)
                buttons.Add(new Button(new Vector2(57, 245), new Vector2(85, 85), constructPlantation, "Plantation",
                new String[]{ "Construct improvement on this tile:", FeatureManager.getFeatureAttributes(feature).improvementCost.ToString() + " cookies",
                              "When improved: " + FeatureManager.getFeatureAttributes(feature).improvedProductivity.ToString() + " cookies per turn"}));
            else if (feature == Features.COWS)
                buttons.Add(new Button(new Vector2(57, 245), new Vector2(85, 85), constructRanch, "Ranch", 
                new String[]{ "Construct improvement on this tile:", FeatureManager.getFeatureAttributes(feature).improvementCost.ToString() + " cookies",
                              "When improved: " + FeatureManager.getFeatureAttributes(feature).improvedProductivity.ToString() + " cookies per turn"}));
            else if (feature == Features.WOODS)
                buttons.Add(new Button(new Vector2(57, 245), new Vector2(85, 85), constructSawmill, "Sawmill", 
                new String[]{ "Construct improvement on this tile:", FeatureManager.getFeatureAttributes(feature).improvementCost.ToString() + " cookies",
                              "When improved: " + FeatureManager.getFeatureAttributes(feature).improvedProductivity.ToString() + " cookies per turn"}));
        }

        images.Add(new Image(new Vector2(200, 0), new Vector2(Game.Resolution.X - 200, 100), texResIn: MenuManager.menuTex));
        buttons.Add(new Button(new Vector2(350, 50), new Vector2(150, 30), closeSpec, "Close Menu"));

        MenuManager.makeMenu(new Menu(new Vector2(0, 100), new Vector2(200, Game.Resolution.Y - 100), "Inspect Tile", buttons, images), true);
        
    }

    private static void deployAdult() { buyUnit(Units.ADULT); }

    private static void deployTeen() { buyUnit(Units.TEEN); }

    private static void deployKid() { buyUnit(Units.KID); }

    private static void constructFarm() { improveTile(Features.WHEAT); }
    private static void constructPlantation() { improveTile(Features.COCOA); }
    private static void constructRanch() { improveTile(Features.COWS); }
    private static void constructSawmill() { improveTile(Features.WOODS); }
}
