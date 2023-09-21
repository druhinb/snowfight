class TileAttributes
{
    public Interval genspace;
    public Texture texture;
    public double attackModifier;
    public double defenceModifier;
    public double visionModifier;
    public Color minimapColor;

    /// <summary>
    /// Initialize the attributes for the tile
    /// </summary>
    /// <param name="genspace">the noise interval this tile will generate at</param>
    /// <param name="texturePath">the texture for this type of tile</param>
    public TileAttributes(Interval genspace, string texturePath, Color color)
    {
        this.genspace = genspace;
        this.texture = Engine.LoadTexture(texturePath);
        this.attackModifier = 1;
        this.defenceModifier = 1;
        this.visionModifier = 1;
        this.minimapColor = color;
    }

    /// <summary>
    /// Initialize the attributes for the tile
    /// </summary>
    /// <param name="genspace">the noise interval this tile will generate at</param>
    /// <param name="texturePath">the texture for this type of tile</param>
    public TileAttributes(Interval genspace, string texturePath, double attackModifier, double defenceModifier, Color color)
    {
        this.genspace = genspace;
        this.texture = Engine.LoadTexture(texturePath);
        this.attackModifier = attackModifier;
        this.defenceModifier = defenceModifier;
        this.minimapColor = color;
    }

    public TileAttributes(Interval genspace, string texturePath, double attackModifier, double defenceModifier, double visionModifier, Color color)
    {
        this.genspace = genspace;
        this.texture = Engine.LoadTexture(texturePath);
        this.attackModifier = attackModifier;
        this.defenceModifier = defenceModifier;
        this.visionModifier = visionModifier;
        this.minimapColor = color;
    }
}