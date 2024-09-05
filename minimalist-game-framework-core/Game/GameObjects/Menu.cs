using System;
using System.Collections.Generic;

// menu objects
class Menu
{
    // textures

    // attributes
    private List<Button> buttons = new List<Button>();
    private List<Image> images = new List<Image>();
    private Vector2 pos;
    private Vector2 size;
    private string name;
    private Bounds2 bound;
    private Vector2 offset = new Vector2(0, 0);
    private bool locked = true;
    private MenuBorder border;

    public Menu(Vector2 posIn, Vector2 sizeIn, string nameIn, List<Button> buttonsIn = null, List<Image> imagesIn = null)
    {
        pos = posIn;
        size = sizeIn;

        name = nameIn;
        bound = new Bounds2(pos, size);
        if (buttonsIn != null)
        {
            buttons = buttonsIn;
        }
        if (imagesIn != null)
        {
            images = imagesIn;
        }
        border = new MenuBorder(pos, size);

    }

    // get methods
    public Vector2 getPos() { return pos; }
    public Vector2 getSize() { return size; }
    public Vector2 getOffset() { return offset; }
    public bool getLock() { return locked; }
    public List<Button> getButtons() { return buttons; }
    public List<Image> getImages() { return images; }

    // add methods
    public void addButton(Button button) { buttons.Add(button); }
    public void addImage(Image image) { images.Add(image); }

    // set methods 
    public void setButtons(List<Button> buttonsIn) { buttons = buttonsIn; }
    public void setImages(List<Image> imagesIn) { images = imagesIn; }
    public void resetImages() { images = new List<Image>(); }

    // calculate new offset
    public void offsetCalc(Vector2 prevMouse)
    {
        // unlock menu if not aligned, lock otherwise
        locked = border.updateConflicts(buttons, images);

        // if the menu is not locked, change the offset of all its components
        if (!locked)
        {
            // calculate a new legal offset
            Vector2 mouseChange = (Engine.MousePosition - prevMouse);
            if (mouseChange.X > border.maxX())
            {
                mouseChange.X = border.maxX();
            }
            if (mouseChange.X < border.minX())
            {
                mouseChange.X = border.minX();
            }
            if (mouseChange.Y > border.maxY())
            {
                mouseChange.Y = border.maxY();
            }
            if (mouseChange.Y < border.minY())
            {
                mouseChange.Y = border.minY();
            }
            offset += mouseChange;

            // set the offsets for all the objects as well
            foreach (Button b in getButtons())
            {
                b.setOffset(offset);
            }
            foreach (Image im in getImages())
            {
                im.setOffset(offset);
            }
        }
    }

    // renders the menu
    public void render()
    {
        Engine.DrawResizableTexture(MenuManager.menuTex, bound);
        Engine.DrawString(name, new Vector2(pos.X + 5, pos.Y + 5), Color.Black, MenuManager.arial);

        // render the components
        foreach (Image im in images)
        {
            im.render();
        }
        foreach (Button b in buttons)
        {
            b.render();
        }
    }
}