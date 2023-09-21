/*
 * Kirill Obraztsov
 * 
 * Manages menus created for the game and their components
 * 
 * 11/22/2022
 * 
*/

using System;
using System.Collections.Generic;
using System.Threading;

static class MenuManager
{
    // meta information
    private static int highMenuLevel = 0;
    private static bool isTop = false;
    public static bool blocking = false;
    public static bool playSounds = true;
    private static Stack<Menu> menus = new Stack<Menu>();

    // textures
    private static ResizableTexture exitButtonTex = Engine.LoadResizableTexture("menu/exitButtonTex.png", 0, 0, 0, 0);
    private static ResizableTexture exitButtonHoverTex = Engine.LoadResizableTexture("menu/exitButtonHoverTex.png", 0, 0, 0, 0);
    private static ResizableTexture mouseOverBoxTex = Engine.LoadResizableTexture("menu/mouseOverBoxTex.png", 4, 4, 4, 4);
    
    // common textures
    public static Texture hpBg = Engine.LoadTexture("menu/hpBg.png");
    public static Texture hpBar = Engine.LoadTexture("menu/hpBar.png");
    public static Texture attackNote = Engine.LoadTexture("menu/attackNote.png");
    public static Texture moveNote = Engine.LoadTexture("menu/moveNote.png");
    public static ResizableTexture buttonTex = Engine.LoadResizableTexture("menu/buttonResTex.png", 4, 4, 4, 4);
    public static ResizableTexture buttonHoverTex = Engine.LoadResizableTexture("menu/buttonResHoverTex.png", 4, 4, 4, 4);
    public static ResizableTexture menuTex = Engine.LoadResizableTexture("menu/menuTex.png", 10, 10, 32, 10);
    public static Font arial = Engine.LoadFont("arial.ttf", 16);
    public static Font arial32 = Engine.LoadFont("arial.ttf", 32);

    // calculations
    private static Vector2 prevMouse = Engine.MousePosition;
    private static string[] currMouseOver = null;

    // main loop that handles all menu actions
    public static void menuLoop()
    {
        // on click and if there is something to click
        if (Engine.GetMouseButtonDown(MouseButton.Left) && menus.Count >= 1)
        {
            foreach (Button b in menus.Peek().getButtons())
            {
                // check if the button is on the top menu and was clicked
                if (b.getClick())
                {
                    b.run();
                }
            }
        }

        // check for attempted menu scrolling
        if (Engine.GetMouseButtonHeld(MouseButton.Right) && menus.Count >= 1)
        {
            menus.Peek().offsetCalc(prevMouse);
        }

        renderAll();
        prevMouse = Engine.MousePosition;
        GC.Collect();
    }

    // creates a new menu object
    public static void makeMenu(Menu menu, bool important = false)
    {
        highMenuLevel++;
        menus.Push(menu);

        // create a back button
        if (!important)
        {
            getTopMenu().addButton(new Button(new Vector2(menu.getPos().X + menu.getSize().X - 27, menu.getPos().Y), new Vector2(27, 27), closeMenu, null, null, exitButtonTex, exitButtonHoverTex, null, false, true));

        }
    }

    // removes the topmost menu object, and all its assets
    public static void closeMenu()
    {
        if (menus.Count > 0)
        {
            menus.Pop();
            highMenuLevel--;
        }
    }
    public static void closeMenus(int count) { for (int i = 0; i < count; i++) closeMenu(); }
    public static void closeAllMenus() { while (menus.Count > 0) closeMenu(); }

    public static void closeBlocking()
    {
        blocking = false;
        closeMenu();
    }

    // creates a screen between player turns
    public static void antiScreenPeek(string playerName, int turnNumber, Color playerColor, bool lost)
    {
        blocking = true;
        makeMenu(new Menu(Vector2.Zero, Game.Resolution, "Next Turn"), true);
        getTopMenu().addImage(new Image(new Vector2(Game.Resolution.X / 2 - 25, Game.Resolution.Y / 2 - 85), new Vector2(50, 50), hpBar, factionOverwrite: true));
        getTopMenu().addImage(new Image(new Vector2(Game.Resolution.X / 2, Game.Resolution.Y / 2 - 25), new Vector2(50, 50), null, playerName + "'s Turn #" + turnNumber));

        if (!FactionManager.factions[GameManager.getCurrentFaction()].lost)
        {
            getTopMenu().addButton(new Button(new Vector2(Game.Resolution.X / 2 - 50, Game.Resolution.Y / 2), new Vector2(100, 25), closeBlocking, "Start Turn"));
        }
        else
        {
            getTopMenu().addImage(new Image(new Vector2(Game.Resolution.X / 2 - 150, Game.Resolution.Y / 2 + 30), new Vector2(50, 50), attackNote));
            getTopMenu().addImage(new Image(new Vector2(Game.Resolution.X / 2 + 25, Game.Resolution.Y / 2 + 60), new Vector2(50, 50), null, "You have lost the game this turn"));
            getTopMenu().addImage(new Image(new Vector2(Game.Resolution.X / 2, Game.Resolution.Y / 2 + 90), new Vector2(50, 50), null, "as a result of losing all units and buildings."));
            getTopMenu().addImage(new Image(new Vector2(Game.Resolution.X / 2, Game.Resolution.Y / 2 + 120), new Vector2(50, 50), null, "You may spectate the board this turn,"));
            getTopMenu().addImage(new Image(new Vector2(Game.Resolution.X / 2, Game.Resolution.Y / 2 + 150), new Vector2(50, 50), null, "but all your next turns will be skipped."));

            getTopMenu().addButton(new Button(new Vector2(Game.Resolution.X / 2 - 50, Game.Resolution.Y / 2 + 250), new Vector2(100, 25), closeBlocking, "OK"));
        }
    }


    // returns the topmost menu for edits
    public static Menu getTopMenu() { return menus.Peek(); }

    // returns whether the selected menu is the topmost one
    public static bool getIsTop() { return isTop; }

    // returns stack size, for certain operations
    public static int getMenuAmt() { return menus.Count; }

    // draws every menu and button

    public static void renderAll()
    {
        MenuLoader.volImage.setString(MenuLoader.volume.ToString());

        if (MenuLoader.muted) { MenuLoader.muteImage.setImage(MenuLoader.soundOff); }
        else { MenuLoader.muteImage.setImage(MenuLoader.soundOn); }

        currMouseOver = null;
        // create a new array to render all the menus
        Menu[] totMenus = new Menu[menus.Count];
        menus.CopyTo(totMenus, 0);

        for (int i = totMenus.Length - 1; i >= 0; i--)
        {
            // render all the components of the given menu
            Menu curr = totMenus[i];

            // only render hover buttons on the topmost menu
            if (curr.Equals(getTopMenu())) { isTop = true; }

            curr.render();
            isTop = false;
        }
        if (currMouseOver != null)
        {
            renderMouseOver(currMouseOver);
        }
    }

    // saves a mouseover to get rendered later
    public static void makeMouseOver(string[] mouseOverText)
    {
        currMouseOver = mouseOverText;
    }

    // creates a mouseover box for an image or button
    public static void renderMouseOver(string[] mouseOverText)
    {
        Vector2 mouse = new Vector2(Engine.MousePosition.X + 7, Engine.MousePosition.Y);

        // get the box size
        int maxLength = 0;
        int maxHeight = mouseOverText.Length;
        for (int i = 0; i < mouseOverText.Length; i++)
        {
            if (mouseOverText[i].Length > maxLength)
            {
                maxLength = mouseOverText[i].Length;
            }
        }
        Vector2 boxSize = new Vector2(10 + maxLength * 9, 12 + maxHeight * 18);

        // lock text box to the game window
        while (mouse.X + boxSize.X >= Game.Resolution.X && mouse.X - 3 > 0)
        {
            mouse.X -= 1;
        }
        while (mouse.Y + boxSize.Y >= Game.Resolution.Y && mouse.Y - 3 > 0)
        {
            mouse.Y -= 1;
        }

        // draw a box and its contents
        Engine.DrawResizableTexture(mouseOverBoxTex, new Bounds2(mouse, boxSize), null, TextureBlendMode.Normal, TextureScaleMode.Nearest);
        for (int i = 0; i < mouseOverText.Length; i++)
        {
            Engine.DrawString(mouseOverText[i], new Vector2(mouse.X + 7, mouse.Y + 5 + i * 18), Color.Black, arial, TextAlignment.Left);

        }
    }
}
