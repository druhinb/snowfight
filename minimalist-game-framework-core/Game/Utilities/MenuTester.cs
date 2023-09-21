/*
 * Kirill Obraztsov
 * 
 * A system for testing menu functionality
 * 
 * 11/21/2022
 * 
*/
using System;
using System.Collections.Generic;

class MenuTester
{
    private static Texture closeTex = Engine.LoadTexture("menu/testTex.png");
    private static Texture menuTex = Engine.LoadTexture("menu/menuTex.png");
    private static Texture infoTex = Engine.LoadTexture("menu/infoIcon.png");

    // testing a variety of menus and buttons
    public static void testScreen()
    {

        // components
        string[] mouseOverText1 = { "this is mouseover text!", "and this is a second line!" };
        string[] mouseOverText2 = { "this is an information icon", "it does not scroll like other menu elements"};
        string[] mouseOverText3 = { "Warning!", "This button closes the menu!" };
        List<Button> buttons1 = new List<Button>();
        buttons1.Add(new Button(new Vector2(500, 500), new Vector2(200, 50), forceClose, "", mouseOverText3, null, null, closeTex));

        // menus
        MenuManager.makeMenu(new Menu(new Vector2(100, 50), new Vector2(500, 100), "wide window"), true);
        MenuManager.getTopMenu().addButton(new Button(new Vector2(150, 85), new Vector2(200, 50), act, "this is a button", mouseOverText1));
        MenuManager.makeMenu(new Menu(new Vector2(0, 0), new Vector2(300, 300), "test window"));
        MenuManager.makeMenu(new Menu(new Vector2(90, 30), new Vector2(450, 220), "wide window numero dos", buttons1));
        MenuManager.getTopMenu().addImage(new Image(new Vector2(420, 100), new Vector2(18, 18), infoTex, "", mouseOverText2, true));
        MenuManager.makeMenu(new Menu(new Vector2(70, 60), new Vector2(300, 1800), "wide window numero tres"));
        MenuManager.closeMenu();
        MenuManager.makeMenu(new Menu(new Vector2(130, 120), new Vector2(400, 250), "wide window numero quatro"));

        // anti-screenpeek test
        // MenuManager.antiScreenPeek("Gapon", 1, closeTex);
    
    }

    // a menu closer to an actual use case
    public static void bigMenuTest()
    {
        // components
        string[] mouseOverText1 = { "Oops, this buttons generates off-screen!" };
        string[] mouseOverText2 = { "Click here to make a new pop-up.", "Don't worry, you can resume your work in this menu afterwards." };
        string[] mouseOverText3 = { "This menu is scrollable!", "Use right click and drag to pan.", "See if you can discover all the hidden buttons!" };


        List<Button> buttons1 = new List<Button>();
        buttons1.Add(new Button(new Vector2(500, 500), new Vector2(100, 100), makeDummyMenu, "", mouseOverText2, null, null, menuTex));
        buttons1.Add(new Button(new Vector2(-100, -100), new Vector2(200, 50), act, "", mouseOverText1, null, null, closeTex));

        List<Image> images1 = new List<Image>();
        images1.Add(new Image(new Vector2(Game.Resolution.X - 22, 35), new Vector2(9, 9), infoTex, "", mouseOverText3, true));

        // menus
        MenuManager.makeMenu(new Menu(Vector2.Zero, Game.Resolution, "wide window", buttons1, images1), true);
    }

    // example actions
    private static void act()
    {

    }
    private static void forceClose()
    {
        MenuManager.closeMenu();
    }

    private static void makeDummyMenu()
    {
        string[] mouseOverText3 = { "Warning!", "This button closes the menu!" };
        List<Button> customExit = new List<Button>();
        customExit.Add(new Button(new Vector2(50, 50), new Vector2(100, 100), forceClose, "", mouseOverText3, null, null, closeTex));
        MenuManager.makeMenu(new Menu(Vector2.Zero, Game.Resolution, "wide window", customExit), true);
    }
}
