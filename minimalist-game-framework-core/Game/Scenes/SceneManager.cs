using System.Collections.Generic;

public class SceneManager
{
    private static Scene[] scenes;
    static SceneManager()
    {
        initialize();
    }

    public static void reset()
    {
        initialize();
    }

    public static void initialize()
    {
        scenes = new Scene[3];
        
        scenes[0] = new MainMenu();
        scenes[1] = new GameScreen();
        scenes[2] = new EndScreen();
    }
    public static void init_game()
    {
        ((GameScreen)(scenes[1])).init_game();
    }
    public static void runScene(int sceneID)
    {
        scenes[sceneID].run();
    }
}