using System.Collections.Generic;

//More user-friendly references for the features
public enum Features
{
    NONE = 0,
    WOODS = 1,
    COCOA = 2,
    COWS = 3,
    WHEAT = 4
}

static class FeatureManager
{
    private static Node<Features> probabilities;
    public static Dictionary<Features, FeatureAttributes> featureProperties =
     new Dictionary<Features, FeatureAttributes>(); 

    static FeatureManager()
    {
        defineFeatures();
    }

    /// <summary>
    /// Defines all the features with their own unique properties 
    /// </summary>
    private static void defineFeatures()
    {
        updateFeatureProperties(Features.COCOA, new FeatureAttributes(new Interval(0, 0.15), "tile/feature/cocoa.png", "tile/buildings/plantation.png", "SFX/cocoa.wav", 0.3, 0.9, 16));
        updateFeatureProperties(Features.WOODS, new FeatureAttributes("tile/foresttile.png", "tile/buildings/sawmill.png", "SFX/forest.wav",  0.0, 0.3, 5));
        updateFeatureProperties(Features.COWS, new FeatureAttributes(new Interval(0.75, 0.85), "tile/feature/cow.png", "tile/buildings/ranch.png", "SFX/cow.wav", 0.3, 1.2, 20));
        updateFeatureProperties(Features.WHEAT, new FeatureAttributes(new Interval(0.85, 1.0), "tile/feature/wheat.png", "tile/buildings/farm.png", "SFX/wheat.wav",  0.2, 0.7, 12));

    }

    /// <summary>
    /// Updates a given feature's properties
    /// </summary>
    /// <param name="feature">The feature to be updated</param>
    /// <param name="attributes">The new set of attributes</param>
    private static void updateFeatureProperties(Features feature, FeatureAttributes attributes)
    {
        //Add key-value pair to feature-attribute dictionary
        featureProperties.Add(feature, attributes);

        //Initializes a new interval tree if none, or appends to existing one 
        if (attributes.genChance != null)
        {
            if (probabilities == null) 
                probabilities = IntervalTree<Features>.insert(null, attributes.genChance, feature);
            else
                probabilities = IntervalTree<Features>.insert(probabilities, attributes.genChance, feature);
        }
    }

    /// <summary>
    /// Finds a noise value's corresponding feature.
    /// </summary>
    /// <param name="noiseValue">The noise value procedurally generated</param>
    /// <returns>The feature that the noiseValue corresponds to</returns>
    public static Features determineFeature(double noiseValue)
    {
        //Does the specified noise value overlap any of the intervals of the different features?
        return IntervalTree<Features>.isOverlapping(probabilities, noiseValue);
    }

    /// <summary>
    /// Gets a given feature's properties
    /// </summary>
    /// <param name="feature">The feature whose properties are to be fetched</param>
    public static FeatureAttributes getFeatureAttributes(Features feature)
    {
        //Fetch the attributes for the given feature from the dictionary
        featureProperties.TryGetValue(feature, out FeatureAttributes value);

        return value;
    }
}