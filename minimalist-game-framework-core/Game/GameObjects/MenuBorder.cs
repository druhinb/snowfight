/*
 * Kirill Obraztsov
 * 
 * A system for resolving menu border calculations
 * 
 * 11/21/2022
 * 
*/
using System;
using System.Collections.Generic;
using System.Drawing;

class MenuBorder
{
    // checks collision for menus
    private Vector2 pos;
    private Vector2 size;

    private float maxXd;
    private float minXd;
    private float maxYd;
    private float minYd;

    public MenuBorder(Vector2 menuPos, Vector2 menuSize)
    {
        pos = menuPos;
        size = menuSize;
    }

    // get methods
    public float maxX() { return maxXd; }
    public float minX() { return minXd; }
    public float maxY() { return maxYd; }
    public float minY() { return minYd; }

    // takes the current lists of objects and returns whether the menu should be locked
    public bool updateConflicts(List<Button> buttons, List<Image> images)
    {
        // reset offset deltas
        maxXd = 0;
        minXd = 0;
        maxYd = 0;
        minYd = 0;

        // check if the menu can be unlocked
        bool isAligned = true;
        foreach (Button b in buttons)
        {
            // check if the button isn't a core component
            if (!b.getLock())
            {
                // check for out of bounds buttons
                Vector2 posT = b.getPos();
                Vector2 sizeT = b.getSize();
                bool getAlign = outOfBoundsCheck(b.getPos(), b.getSize());
                if (!getAlign)
                {
                    isAligned = false;
                }
            }
        }
        foreach (Image im in images)
        {
            // check if the image isn't a core component
            if (!im.getLock())
            {
                // check for out of bounds images
                Vector2 posT = im.getPos();
                Vector2 sizeT = im.getSize();
                bool getAlign = outOfBoundsCheck(im.getPos(), im.getSize());
                if (!getAlign)
                {
                    isAligned = false;
                }
            }
        }
        return isAligned;
    }

    // generic check for images and buttons
    private bool outOfBoundsCheck(Vector2 posT, Vector2 sizeT)
    {
        bool isAligned = true;
        // right oob check
        if (posT.X < pos.X + 13)
        {
            if (maxXd < (pos.X + 13) - (posT.X))
            {
                maxXd = (pos.X + 13) - (posT.X);
            }
            isAligned = false;
        }
        // down oob check
        if (posT.Y < pos.Y + 35)
        {
            if (maxYd < (pos.Y + 35) - (posT.Y))
            {
                maxYd = (pos.Y + 35) - (posT.Y);
            }
            isAligned = false;
        }
        // left oob check
        if (posT.X + sizeT.X > pos.X + size.X - 13)
        {
            if (minXd > (pos.X + size.X - 13) - (posT.X + sizeT.X))
            {
                minXd = (pos.X + size.X - 13) - (posT.X + sizeT.X);
            }
            isAligned = false;
        }
        // up oob check
        if (posT.Y + sizeT.Y > pos.Y + size.Y - 13)
        {
            if (minYd > (pos.Y + size.Y - 13) - (posT.Y + sizeT.Y))
            {
                minYd = (pos.Y + size.Y - 13) - (posT.Y + sizeT.Y);
            }
            isAligned = false;
        }
        return isAligned;
    }
}

