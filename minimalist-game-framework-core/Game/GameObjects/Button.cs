/*
 * Kirill Obraztsov
 * 
 * Used by menus as interactible components
 * 
 * 11/22/2022
 * 
*/
using System;
using System.Collections.Generic;

class Button
{
    // button attributes
    private Vector2 posGen; // initial position
    private Vector2 pos; // current position
    private Vector2 size;
    private Bounds2 bound;
    private Vector2 offset = Vector2.Zero;
    private string name;
    private string[] mouseOverText = null;
    private ResizableTexture customButtonTex = null;
    private ResizableTexture customButtonHoverTex = null;
    private Texture customImage = null;
    private bool isInvis;
    private bool locked;


    // meta information
    Action action;


    // initializes a button object
    public Button(Vector2 posIn, Vector2 sizeIn, Action actionIn, string nameIn = "", string[] mouseOverTextIn = null,
        ResizableTexture customButtonTexIn = null, ResizableTexture customButtonHoverTexIn = null,
        Texture customImageIn = null, bool isInvisIn = false, bool lockedIn = true)
    {
        posGen = posIn;
        size = sizeIn;
        name = nameIn;
        if (mouseOverTextIn != null)
        {
            mouseOverText = (string[]) mouseOverTextIn.Clone();
        }
        customButtonTex = customButtonTexIn;
        customButtonHoverTex = customButtonHoverTexIn;
        bound = new Bounds2(posGen, size);
        action = actionIn;
        customImage = customImageIn;
        isInvis = isInvisIn;
        locked = lockedIn;
    }

    // get methods
    public Vector2 getPos() { return pos; }
    public Vector2 getSize() { return size; }
    public bool getLock() { return locked; }

    // set methods
    public void setOffset(Vector2 newOffset) 
    { 
        if (!locked) {
            offset = newOffset;
        }
    }

    // draws the button object
    public void render()
    {
        pos = posGen + offset;
        bound = new Bounds2(pos, size);

        // check that the button needs to be rendered or just needs to exist as a hitbox
        if (!isInvis)
        {
            // checks if the mouse is inside the hitbox of the button
            float x = Engine.MousePosition.X;
            float y = Engine.MousePosition.Y;

            // checks for a custom texture before drawing
            if (customButtonTex != null)
            {
                if (x >= pos.X && x <= pos.X + size.X && y >= pos.Y && y <= pos.Y + size.Y &&
                    customButtonHoverTex != null && MenuManager.getIsTop())
                {
                    // draw hover texture and mouseover on collision
                    Engine.DrawResizableTexture(customButtonHoverTex, new Bounds2(pos, size), null, TextureBlendMode.Normal, TextureScaleMode.Nearest);
                    renderName();
                    if (mouseOverText != null)
                    {
                        renderMouseOver();
                    }
                    
                } else
                // draws the non-hovered custom texture
                {
                    Engine.DrawResizableTexture(customButtonTex, new Bounds2(pos, size), null, TextureBlendMode.Normal, TextureScaleMode.Nearest);
                    renderName();
                }
            }
            else
            {
                if (x >= pos.X && x <= pos.X + size.X && y >= pos.Y && y <= pos.Y + size.Y && MenuManager.getIsTop())
                {
                    // draw hover texture and mouseover on collision
                    Engine.DrawResizableTexture(MenuManager.buttonHoverTex, bound, null, TextureBlendMode.Normal, TextureScaleMode.Nearest);
                    renderName();
                    
                    if (mouseOverText != null)
                    {
                        renderMouseOver();
                    }
                } else
                // draws normal texture
                {
                    Engine.DrawResizableTexture(MenuManager.buttonTex, bound, null, TextureBlendMode.Normal, TextureScaleMode.Nearest);
                    renderName();
                }
            }
        }
    }

    //renders the name or custom image of the button
    private void renderName()
    {
        //  check whether to render image or name
        if (customImage != null)
        {
            Engine.DrawTexture(customImage, new Vector2(pos.X + 6, pos.Y + 6), null, new Vector2(size.X - 12, size.Y - 12), 0, null, TextureMirror.None, null, TextureBlendMode.Normal, TextureScaleMode.Nearest);
        }
        else
        {
            Engine.DrawString(name, new Vector2(pos.X + size.X / 2, pos.Y + 4), Color.Black, MenuManager.arial, TextAlignment.Center);

        }
    }

    // renders the mouseover text for the button
    private void renderMouseOver()
    {
        MenuManager.makeMouseOver(mouseOverText); 
    }

    // gets whether the button was interacted with
    public bool getClick()
    {
        // checks if the mouse is inside the hitbox of the button
        float x = Engine.MousePosition.X;
        float y = Engine.MousePosition.Y;

        return x >= pos.X && x <= pos.X + size.X &&
            y >= pos.Y && y <= pos.Y + size.Y;
    }

    // runs the target function
    public void run()
    {
        Engine.PlaySound(soundStorage.button);
        action();
    }
}