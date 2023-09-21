using System;
using System.Collections.Generic;

public abstract class TileStructure
{
    public Tuple<int, int> location;
    public string texturePath;

    public Texture renderTex;
    public static Texture cityOverlay = Engine.LoadTexture("TileStructures/city_overlay.png");
    public static Texture city = Engine.LoadTexture("TileStructures/city.png");
    public static Texture town = Engine.LoadTexture("TileStructures/town.png");
    public static Texture land = Engine.LoadTexture("TileStructures/land.png");

    public TileStructure(Tuple<int, int> location, string texturePath)
    {
        this.renderTex = Engine.LoadTexture(texturePath);
        this.texturePath = texturePath;
        this.location = location;
    }

    public virtual void render(Vector2 pos, int scale)  
    {
        Engine.DrawTexture(renderTex, pos, size: new Vector2(scale, scale), scaleMode: TextureScaleMode.Nearest);
    }

    public abstract int getFactionID();
    public abstract void setFactionID(int factionID);

    public abstract List<string> save();
    public abstract int load(ref List<string> l, int i);
}