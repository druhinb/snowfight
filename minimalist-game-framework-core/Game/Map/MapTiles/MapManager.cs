using System;
using System.Collections;
using System.Collections.Generic;

static class MapManager
{
    private static int displayHalfWidth, displayHalfHeight;
    private static Vector2 displayOffset;

    public static int width, height;
    private static TileMap map;
    private static Unit[,] units;

    //Which algorithm to be used for generation?
    private static string algorithm;
    private static int scale = 64;
    public static Tuple<int, int> selectedTile;

    /// <summary>
    /// Create and generate map.
    /// </summary>
    /// <param name="mapSize">Size of map in tiles. (width, height)</param>
    /// <param name="algorithm">Algorithm used to generate map.</param>
    /// <param name="sink_edges">???</param>
    /// <param name="seed">Seed used to generate map.</param>
    /// <param name="displayHalf">Half of display size in pixels. (halfWidth, halfHeight)</param>
    /// <param name="displayOffset">Offset of the display from the center of the screen. (x, y)</param>
    public static void createMap(Tuple<int, int> mapSize, string algorithm, bool sink_edges,
    double seed, Tuple<int, int> displayHalf, Vector2 displayOffset, double townDensity, double abundance)

    {
        scale = 64;
        selectedTile = null;

        MapManager.width = mapSize.Item1;
        MapManager.height = mapSize.Item2;
        MapManager.displayHalfWidth = displayHalf.Item1;
        MapManager.displayHalfHeight = displayHalf.Item2;
        MapManager.displayOffset = displayOffset;

        generate(algorithm, sink_edges, seed, townDensity, abundance);
    }


    public static Biomes getBiome(Tuple<int, int> tileIndices) { return map.getBiome(tileIndices); }
    public static int getStructureID(Tuple<int, int> tileIndices) { return map.getStructureID(tileIndices); }
    public static Features getFeature(Tuple<int, int> tileIndices) { return map.getFeature(tileIndices); }
    public static bool isImproved(Tuple<int, int> tileIndices) { return map.isImproved(tileIndices); }
    public static int getFactionID(Tuple<int, int> tileIndices) { return map.getFactionID(tileIndices); }
    public static void setFactionID(Tuple<int, int> tileIndices, int faction) { map.setFactionID(tileIndices, faction); }
    public static double getProductivity(Tuple<int, int> tileIndices) { return map.getProductivity(tileIndices); }

    public static bool isOccupied(Tuple<int, int> tileIndices) { return map.isOccupied(tileIndices); }

    public static int occupiedBy(Tuple<int, int> tileIndices) { return map.occupiedBy(tileIndices); }

    public static void setHighlight(Tuple<int, int> tileIndices, Color color) { map.setHighlight(tileIndices, color); }
    public static void setOverlay(Tuple<int, int> tileIndices, Texture texture) {map.setOverlay(tileIndices, texture);}
    public static void setStructure(Tuple<int, int> tileIndices, int structureID)
        { map.setStructure(tileIndices, structureID); }
    public static void setFeature(Tuple<int, int> tileIndices, Features feature) { map.setFeature(tileIndices, feature); }


    public static bool occupy(Tuple<int, int> tileIndices, int unitID)
    { return map.occupy(tileIndices, unitID); }

    public static bool deoccupy(Tuple<int, int> tileIndices, int unitID)
    { return map.deoccupy(tileIndices, unitID); }

    public static void improve(Tuple<int, int> tileIndices) { map.improve(tileIndices); }

    public static void renderObfuscated(Tuple<int, int> tileIndices, Vector2 pos, int scale) { map.renderObfuscated(tileIndices, pos, scale); }
    public static void renderTile(Tuple<int, int> tileIndices, Vector2 pos, int scale) { map.renderTile(tileIndices, pos, scale); }
    public static void renderAll(Tuple<int, int> tileIndices, Vector2 pos, int scale) { map.renderAll(tileIndices, pos, scale); }

    // Changes the scale of the map (how many pixels per tile)
    public static void setScale(int newScale) { scale = newScale; }

    // Generate map using algorithm and seed
    private static void generate(string algorithm, bool sink_edges, double seed, double townDensity, double abundance)
    {
        int plainsCount = 0;
        MapManager.algorithm = algorithm;

        //refreshes the "map"
        map = new TileMap(width, height);

        //Generates new noise value for every tile
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                double noiseValue, vegetationValue = 0;

                noiseValue = Perlin.OctavePerlin((float)x / width, (float)y / height, seed, 4, 1.2);
                Biomes biome = BiomeManager.determineBiome(noiseValue);

                vegetationValue = Perlin.OctavePerlin((float)x / width, (float)y / height, seed + 1, 5, 1.1) + 1;
                Biomes vegetation = BiomeManager.determineBiome(vegetationValue) == Biomes.FOREST ? Biomes.FOREST : default(Biomes);

                //Which biome is the tile according to its noise value?
                map.setTile(new MapTile(new Tuple<int, int>(x, y), biome));

                if (biome == Biomes.PLAINS)
                {
                    plainsCount++;
                    if (vegetation == Biomes.FOREST)
                    {
                        map.setTile(new MapTile(new Tuple<int, int>(x, y), Biomes.FOREST));
                        setFeature(new Tuple<int, int>(x, y), Features.WOODS);
                    }
                }
            }
        }

        placeTowns(plainsCount, townDensity, seed);
        placeFeatures(plainsCount, abundance, seed);
    }

    private static void placeTowns(int plainsTiles, double houseDensity, double seed)
    {
        int numTowns = (int)(0.0875 * (double)plainsTiles * houseDensity);
        Random rnd = new Random((int)seed * 1000000); int rndx = 0; int rndy = 0;

        Tuple<int, int> tileIndices = new Tuple<int, int>(rndx, rndy);

        for (int i = 0; i < numTowns; i++)
        {
            do
            {
                rndx = rnd.Next(width); rndy = rnd.Next(height);
                tileIndices = new Tuple<int, int>(rndx, rndy);
            }
            while (getBiome(tileIndices) != Biomes.PLAINS);
            if (rndx >= 3 && rndy >= 3 && rndx <= MapGenMenu.mapHalfSize * 2 - 4 && rndy <= MapGenMenu.mapHalfSize * 2 - 4)
            {
                int id = TileStructureManager.createTown(tileIndices);
            }
        }
    }

    private static void placeFeatures(int plainsTiles, double abundance, double seed)
    {
        int numTowns = (int)(0.6 * (double)plainsTiles * abundance);
        Random rnd = new Random((int)seed * 10000); int rndx = 0; int rndy = 0;

        Tuple<int, int> tileIndices = new Tuple<int, int>(rndx, rndy);

        for (int i = 0; i < numTowns; i++)
        {
            do
            {
                rndx = rnd.Next(width); rndy = rnd.Next(height);
                tileIndices = new Tuple<int, int>(rndx, rndy);
            }
            while (getBiome(tileIndices) != Biomes.PLAINS);
            
            Features temp = FeatureManager.determineFeature(rnd.NextDouble());
            if (getStructureID(tileIndices) == -1)
            {
                if (temp != default(Features))
                    setFeature(tileIndices, temp);
            }
        }
    }

    /// <summary>
    /// Get world-space coordinates of a tile.
    /// </summary>
    /// <param name="mapIndices">Indices of the tile. (x, y)</param>
    /// <returns>World-space coordinates of the center of the tile. (x, y)</returns>
    public static Vector2 getCoordinates(Tuple<int, int> mapIndices)
    { return new Vector2(mapIndices.Item1 - width / 2, mapIndices.Item2 - height / 2) * scale; }

    /// <summary>
    /// Returns screen coordinates for a given tile.
    /// </summary>
    /// <param name="cameraPosition">Current position of the camera. (x, y)</param>
    /// <param name="mapCoordinates">Indices of the tile. (x, y)</param>
    /// <returns></returns>
    public static Vector2 getScreenCoordinates(Vector2 cameraPosition, Tuple<int, int> mapCoordinates)
    {
        int halfMapWidth = width * scale / 2, halfMapHeight = height * scale / 2;

        Vector2 tilePos = new Vector2(scale * mapCoordinates.Item1 - halfMapWidth, scale * mapCoordinates.Item2 - halfMapHeight) - cameraPosition +
                new Vector2(displayHalfWidth, displayHalfHeight) + displayOffset;

        return tilePos;
    }

    /// <summary>
    /// Returns the world-space indices for a given tile.
    /// </summary>
    /// <param name="cameraPosition">Current position of the camera. (x, y)</param>
    /// <param name="tilePosition">Screen coordinates of the tile. (x, y)</param>
    /// <returns></returns>
    public static Tuple<int, int> getIndices(Vector2 cameraPosition, Vector2 tilePosition)
    {
        int halfMapWidth = width * scale / 2, halfMapHeight = height * scale / 2;
        float mapX = cameraPosition.X + halfMapWidth + tilePosition.X - displayOffset.X - displayHalfWidth;
        float mapY = cameraPosition.Y + halfMapHeight + tilePosition.Y - displayOffset.Y - displayHalfHeight;

        int x = (int)(mapX / scale), y = (int)(mapY / scale);

        // check that the indexes are valid
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return null;
        }
        else
        {
            return new Tuple<int, int>(x, y);
        }
    }

    /// <summary>
    /// Fetch the entire map.
    /// </summary>
    public static MapTile[,] getMap() { return map.getMap(); }

    /// <summary>
    /// Render the map accounting for the faction's vision and the camera position.
    /// </summary>
    /// <param name="cameraPosition">Where the camera is currently at.</param>
    /// <param name="faction">Current faction.</param>
    public static void render(Vector2 cameraPosition, int factionID)
    {
        int halfMapWidth = width * scale / 2, halfMapHeight = height * scale / 2;
        int mapX = (int)(cameraPosition.X) + halfMapWidth, mapY = (int)(cameraPosition.Y) + halfMapHeight;

        int leftX = (mapX - displayHalfWidth) / scale, rightX = (mapX + displayHalfWidth) / scale + 1;
        int topY = (mapY - displayHalfHeight) / scale, bottomY = (mapY + displayHalfHeight) / scale + 1;

        for (int tileX = leftX; tileX <= rightX; tileX++)
        {
            for (int tileY = topY; tileY <= bottomY; tileY++)
            {
                // Render tile based on faction vision
                if (tileX < 0 || tileX >= width || tileY < 0 || tileY >= height) continue;
                Tuple<int, int> tileIndices = new Tuple<int, int>(tileX, tileY);
                Vector2 tilePos = new Vector2(scale * tileX - halfMapWidth, scale * tileY - halfMapHeight) - cameraPosition +
                    new Vector2(displayHalfWidth, displayHalfHeight) + displayOffset;

                if (FactionManager.inVision(factionID, tileIndices))
                {
                    renderAll(tileIndices, tilePos, scale);
                }
                else if (FactionManager.explored(factionID, tileIndices)) renderTile(tileIndices, tilePos, scale);
                else { renderObfuscated(tileIndices, tilePos, scale); }
                //if (faction.inVision(tileX, tileY)) map.getTile(tileX, tileY).renderAll(tilePos, scale);
                //else if (faction.explored(tileX, tileY)) map.getTile(tileX, tileY).renderTile(tilePos, scale);
            }
        }

        MenuLoader.renderMinimap();
        Engine.DrawRectEmpty(new Bounds2(
            new Vector2((cameraPosition.X + halfMapWidth - displayHalfWidth) / scale * ((float)(200) / MapManager.width), Game.Resolution.Y - (float)(200) + (cameraPosition.Y + halfMapHeight - displayHalfHeight) / scale * ((float)(200) / MapManager.height)),
            new Vector2(displayHalfWidth * 2 / scale * ((float)(200) / MapManager.width), displayHalfHeight * 2 / scale * ((float)(200) / MapManager.height))), Color.Goldenrod);
    }

    /// <summary>
    /// Get the four surrounding spaces.
    /// </summary>
    /// <param name="tile">Tile to be evaluated for neighbors</param>
    /// <returns>List of four surrounding spaces. (tile/null)</returns>
    public static List<Tuple<int, int>> getFourSurrounding(Tuple<int, int> tile)
    {
        List<Tuple<int, int>> surrounding = new List<Tuple<int, int>>();
        int x = tile.Item1, y = tile.Item2;
        Tuple<int, int> l, r, u, d;
        l = new Tuple<int, int>(x - 1, y);
        r = new Tuple<int, int>(x + 1, y);
        u = new Tuple<int, int>(x, y - 1);
        d = new Tuple<int, int>(x, y + 1);
        surrounding.Add(l);
        surrounding.Add(r);
        surrounding.Add(u);
        surrounding.Add(d);
        return surrounding;
    }

    /// <summary>
    /// Get all tiles surrounding a given tile.
    /// </summary>
    /// <param name="tile">The tile to be evaluated for neighbors.</param>
    /// <returns>A list of the neighboring tiles.</returns>
    public static List<Tuple<int, int>> neighboringTiles(Tuple<int, int> tile)
    {
        List<Tuple<int, int>> surrounding = getFourSurrounding(tile);
        List<Tuple<int, int>> neighbors = new List<Tuple<int, int>>();

        foreach (Tuple<int, int> mt in surrounding)
            if (mt.Item1 >= 0 && mt.Item1 < width && mt.Item2 >= 0 && mt.Item2 < height) neighbors.Add(mt);

        return neighbors;
    }

    /// <summary>
    /// Highlights a given tile with the appropraite colour.
    /// </summary>
    /// <param name="tileIndices">The tile to be highlighted.</param>
    public static void select(Tuple<int, int> tileIndices, int factionID)
    {
        // Deselects the existing tile 
        deselect();
        UnitManager.deselect();

        int unitID = occupiedBy(tileIndices);
        // If the tile is occupied by something
        if (unitID != -1 || getStructureID(tileIndices) != -1 || getFactionID(tileIndices) != -1)
        {
            // Select the given tile
            selectedTile = tileIndices;

            // Set the tile to blue if there's a friendly unit
            if ((unitID != -1 && UnitManager.getFactionID(occupiedBy(tileIndices)) == factionID) ||
                ((getStructureID(tileIndices) != -1) &&
                TileStructureManager.getFactionID(getStructureID(tileIndices)) == factionID))
            {
                setHighlight(selectedTile, Color.Blue);
            }
            if (getFactionID(tileIndices) == factionID)
            {
                setHighlight(selectedTile, Color.Blue);
                MenuLoader.inspectTile(tileIndices);
            }
            // Set the tile to red if there's an unfriendly unit on it
            else if (unitID != -1 && UnitManager.getFactionID(unitID) != factionID)
            {
                UnitManager.deselect();
                TileStructureManager.deselect();
                setHighlight(selectedTile, Color.Red);
            }
        }
    }

    /// <summary>
    /// Deselects the tile by setting the colour to transparent.
    /// </summary>
    public static void deselect()
    {
        if (selectedTile != null)
        {
            MapManager.setHighlight(selectedTile, Color.Transparent);
            if (getFactionID(selectedTile) != -1 && MapManager.getFactionID(selectedTile) ==
                GameManager.currentFaction && MenuManager.getMenuAmt() > 1)
                MenuManager.closeMenu();
        }

        selectedTile = null;
    }

    public static Tuple<int, int> getRandomTile()
    {
        Random rnd = new Random();
        return new Tuple<int, int>(rnd.Next(), rnd.Next());
    }

    public static Tuple<int, int> getRandomTile(Biomes biome)
    {
        Random rnd = new Random();
        Tuple<int, int> temp;

        do
        {
            temp = new Tuple<int, int>(rnd.Next(width), rnd.Next(height));
        }
        while (getBiome(temp) != biome);
        return temp;
    }

    public static bool withinMapBounds(Vector2 position)
    {
        if (position.X < displayOffset.X + (2 * displayHalfWidth)
            && position.X > displayOffset.X
            && position.Y < displayOffset.Y + (2 * displayHalfHeight)
            && position.Y > displayOffset.Y)
        {
            return true;
        }
        return false;
    }

    static List<string> l = new List<string>();
    public static List<string> save()
    {
        l.Clear();

        Utilities.write(ref l, width);
        Utilities.write(ref l, height);
        Utilities.write(ref l, scale);

        List<string> ml = map.save();
        foreach (string str in ml) l.Add(str);

        return l;
    }
    public static int load(ref List<string> l, int i)
    {
        Utilities.read(ref width, l[i++]);
        Utilities.read(ref height, l[i++]);
        Utilities.read(ref scale, l[i++]);

        i = map.load(ref l, i);

        return i;
    }
}

