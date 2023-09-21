class FeatureAttributes
{
    public Interval genChance;
    public string improvedTexturePath, soundPath;
    public double unimprovedProductivity;
    public double improvedProductivity;
    public double improvementCost;

    public Texture texture;
    public Sound sound;


    /// <summary>
    /// Initialize the attributes for the tile
    /// </summary>
    /// <param name="genspace">the noise interval this tile will generate at</param>
    /// <param name="texturePath">the texture for this type of tile</param>
    public FeatureAttributes(Interval genChance, string texturePath, string improvedTexPath, string soundPath, double unimprovedProductivity, double improvedProductivity, double improvementCost)
    {
        this.genChance = genChance;
        this.improvedTexturePath = improvedTexPath;
        this.soundPath = soundPath;
        this.unimprovedProductivity = unimprovedProductivity;
        this.improvedProductivity = improvedProductivity;
        this.improvementCost = improvementCost;

        this.texture = Engine.LoadTexture(texturePath);
        this.sound = Engine.LoadSound(soundPath);
    }

    public FeatureAttributes(string texturePath, string improvedTexPath, string soundPath, double unimprovedProductivity, double improvedProductivity, double improvementCost)
    {
        this.texture = Engine.LoadTexture(texturePath);
        this.improvedTexturePath = improvedTexPath;
        this.unimprovedProductivity = unimprovedProductivity;
        this.improvedProductivity = improvedProductivity;
        this.improvementCost = improvementCost;
        this.sound = Engine.LoadSound(soundPath);
    }
}