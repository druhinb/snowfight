/*
 * Kirill Obraztsov
 * 
 * Used by menus as static components
 * 
 * 11/20/2022
 * 
*/
using System;
using System.Collections.Generic;

class Image
{
    // textures
    private Texture tex;
    private ResizableTexture texRes;

    // image attributes
    private Vector2 posGen;
    private Vector2 pos;
    private Vector2 size;
    private Vector2 offset = Vector2.Zero;
    private string[] mouseOverText = null;
    private string text;
    private bool locked;
    private bool factionOverwrite;

    // initializes an image object
    public Image(Vector2 posIn, Vector2 sizeIn, Texture texIn = null, string textIn = "",
        string[] mouseOverTextIn = null, bool lockedIn = true, ResizableTexture texResIn = null, bool factionOverwrite = false)
    {
        posGen = posIn;
        size = sizeIn;
        tex = texIn;
        text = textIn;
        mouseOverText = mouseOverTextIn;
        locked = lockedIn;
        texRes = texResIn;
        this.factionOverwrite = factionOverwrite;
    }

    // get methods
    public Vector2 getPos() { return pos; }
    public Vector2 getSize() { return size; }
    public bool getLock() { return locked; }

    // set methods
    public void setOffset(Vector2 newOffset)
    {
        if (!locked)
        {
            offset = newOffset;
        }
    }

    public void setString(String str)
    {
        text = str;
    }

    public void setImage(Texture tex)
    {
        this.tex = tex;
    }

    // draws the button object
    public void render()
    {
        pos = posGen + offset;

        if (text != "")
        {
            Engine.DrawString(text, pos, Color.Black, MenuManager.arial, TextAlignment.Center);
        }
        else
        {
            Color useColor;
            if (factionOverwrite)
            {
                useColor = FactionManager.getFactionColor(GameManager.currentFaction);
            } else
            {
                useColor = Color.White;
            }
            if (tex != null) Engine.DrawTexture(tex, pos, useColor, size, 0, null, TextureMirror.None, null, TextureBlendMode.Normal, TextureScaleMode.Nearest);
            else Engine.DrawResizableTexture(texRes, new Bounds2(pos, size), null, TextureBlendMode.Normal, TextureScaleMode.Nearest);
        }

        // checks if the mouse is inside the hitbox of the image
        float x = Engine.MousePosition.X;
        float y = Engine.MousePosition.Y;

        if (x >= pos.X && x <= pos.X + size.X && y >= pos.Y && y <= pos.Y + size.Y &&
            MenuManager.getIsTop() && mouseOverText != null)
        {
            renderMouseOver();
        }
    }

    // renders the mouseover text for the image
    private void renderMouseOver()
    {
        MenuManager.makeMouseOver(mouseOverText);
    }
}
