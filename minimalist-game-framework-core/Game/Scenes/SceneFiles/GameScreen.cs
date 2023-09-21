using System;

public class GameScreen : Scene
{
    public GameScreen() {}

    public override void start()
    { 
        GameManager.cam.setScale(FactionManager.getLastCameraState(0).Item2);
        GameManager.cam.setPosition(FactionManager.getLastCameraState(0).Item1);
        UnitManager.resetMovementPoints();
    }

    public void init_game() { pre_init(); }

    public override void update()
    {
        GameManager.cam.handleInput();

        if (!MenuManager.blocking)
            InputManager.handleButtonInput(GameManager.cam.getPosition());

        MenuLoader.updateToolbar(GameManager.currentFaction);
        MapManager.setScale(GameManager.cam.getScale());
        MapManager.render(GameManager.cam.getPosition(), GameManager.currentFaction);
    }
}