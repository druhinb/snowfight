using System;
using System.Collections.Generic;

class Game
{
    public static readonly string Title = "Game";
    public static readonly Vector2 Resolution = new Vector2(1280, 720);

    public Game()
    {

    }

    public void Update()
    {
        EngineModifications.updatePerFrameEngine();
        GameManager.tick();
    }
}
