using System.Collections.Generic;

//More user-friendly references for the biomes
public enum Biomes
{
    //MOUNTAIN = 0,
    HILLS = 0,
    PLAINS = 1,
    ICELAKES = 2,
    FOREST = 3
}

static class BiomeManager
{
    private static Node<Biomes> thresholds;
    public static Dictionary<Biomes, TileAttributes> biomeProperties =
        new Dictionary<Biomes, TileAttributes>(); 

    static BiomeManager()
    {
        defineBiomes();
    }

    /// <summary>
    /// Defines all the biomes with their own unique properties 
    /// </summary>
    private static void defineBiomes()
    {
        updateBiomeProperties(Biomes.PLAINS, new TileAttributes(new Interval(0.47, 0.585), "tile/snowtile.png", Color.White));
        updateBiomeProperties(Biomes.HILLS, new TileAttributes(new Interval(0.585, 1), "tile/hilltile.png", 1.0, 1.4, 1.5, Color.Gray));
        updateBiomeProperties(Biomes.ICELAKES, new TileAttributes(new Interval(0, 0.47), "tile/icetile.png", 1.0, 0.5, 1.0, Color.LightSkyBlue));

        /*Vegetation Tiles*/
        updateBiomeProperties(Biomes.FOREST, new TileAttributes(new Interval(1.555, 1.6), "tile/foresttile.png", 0.75, 2, 0.5, Color.ForestGreen));

    }

    /// <summary>
    /// Updates a given biome's properties
    /// </summary>
    /// <param name="biome">The biome to be updated</param>
    /// <param name="attributes">The new set of attributes</param>
    private static void updateBiomeProperties(Biomes biome, TileAttributes attributes)
    {
        //Add key-value pair to biome-attribute dictionary
        biomeProperties.Add(biome, attributes);

        //Initializes a new interval tree if none, or appends to existing one 
        if (thresholds == null)
            thresholds = IntervalTree<Biomes>.insert(null, attributes.genspace, biome);
        else
            thresholds = IntervalTree<Biomes>.insert(thresholds, attributes.genspace, biome);
    }

    /// <summary>
    /// Updates a given biome's properties
    /// </summary>
    /// <param name="noiseValue">The noise value procedurally generated</param>
    /// <return>The biome that the noiseValue corresponds to</return>
    public static Biomes determineBiome(double noiseValue)
    {
        //Does the specified noise value overlap any of the intervals of the different biomes?
        return IntervalTree<Biomes>.isOverlapping(thresholds, noiseValue);
    }

    /// <summary>
    /// Gets a given biome's properties
    /// </summary>
    /// <param name="biome">The biome whose properties are to be fetched</param>
    public static TileAttributes getBiomeAttributes(Biomes biome)
    {
        //Fetch the attributes for the given biome from the dictionary
        biomeProperties.TryGetValue(biome, out TileAttributes value);

        return value;
    }
}