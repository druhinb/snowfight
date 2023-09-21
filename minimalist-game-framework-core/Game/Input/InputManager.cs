using System;
using System.Linq;

static class InputManager
{

    static InputManager()
    {

    }

    public static void handleButtonInput(Vector2 cameraPosition)
    {
        checkTile(cameraPosition);
    }
    
    /*Checks to see if a given tile has  been clicked or not*/
    private static void checkTile(Vector2 cameraPosition)
    {
        //If left clicked
        if (Engine.GetMouseButtonDown(MouseButton.Left))
        {
            if (MapManager.withinMapBounds(Engine.MousePosition))
            {
                MapManager.deselect();
                //Select the tile where the click occurred
                Tuple<int, int> tileIndices = MapManager.getIndices(cameraPosition, Engine.MousePosition);
                if (tileIndices != null && FactionManager.factions[GameManager.currentFaction].inVision(tileIndices))
                    selectTile(tileIndices, GameManager.currentFaction);    
            }
        }

        //If right clicked
        else if (Engine.GetMouseButtonDown(MouseButton.Right))
        {
            //Get the tile upon which the click occurred 
            Tuple<int, int> tileIndices = MapManager.getIndices(cameraPosition, Engine.MousePosition);

            //If the player has a unit selected
            int selectedUnitID = UnitManager.selectedUnitID;
            if (selectedUnitID != -1 && FactionManager.factions[GameManager.currentFaction].inVision(tileIndices))
            {
                //If the tile right-clicked on is one the unit can move to, move unit there
                if (UnitManager.selUnitRange.moveToAbleTiles.Contains(tileIndices))
                {
                    UnitManager.moveTo(selectedUnitID, tileIndices);
                    MapManager.deselect();
                }
                //If the tile right-clicked on is one that the unit can attack, attack
                else if (UnitManager.selUnitRange.attackableTiles.Contains(tileIndices))
                {
                    UnitManager.attack(selectedUnitID, tileIndices);
                    MapManager.deselect();
                }
            }
        }
    }

    /*Selects the tile that's been clicked*/
    private static void selectTile(Tuple<int, int> tileIndices, int factionID)
    {
        if (tileIndices != null)
        {
            //If there's something on the tile
            int structureID = MapManager.getStructureID(tileIndices);

            int unitID = MapManager.occupiedBy(tileIndices);

            if (unitID != -1)
            {
                //Select the tile (highlight)
                MapManager.select(tileIndices, factionID);
                if (UnitManager.getFactionID(unitID) == factionID)
                    UnitManager.selectUnit(MapManager.occupiedBy(tileIndices));
            }

            else if ((structureID != -1 && (TileStructureManager.getStructureType(structureID) == typeof(City))) ||
                MapManager.getFactionID(tileIndices) != -1)
            {
                //Select the tile (highlight)
                MapManager.select(tileIndices, factionID);

                //if there's a city on the tile, select it
                if (MapManager.getStructureID(tileIndices) != -1 &&
                    TileStructureManager.getStructureType(MapManager.getStructureID(tileIndices)) == typeof(City) &&
                    TileStructureManager.getFactionID(MapManager.getStructureID(tileIndices)) == factionID)
                {
                    TileStructureManager.selectCity(MapManager.getStructureID(tileIndices));
                }
            }

            //If the tile is empty, deselect whatever was selected 
            else
            {
                MapManager.deselect(); UnitManager.deselect(); TileStructureManager.deselect();
            }
        }
        else
        {
            MapManager.deselect(); UnitManager.deselect(); TileStructureManager.deselect();
        }
    }
}