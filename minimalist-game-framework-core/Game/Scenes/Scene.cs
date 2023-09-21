using System;

public abstract class Scene
{
    public int id;
    private bool init;

    public Scene(){}

    public abstract void start();
    public abstract void update();
    public void pre_init()
    {
        init = true;
    }
    public void run()
    {
        if (!init)
        {
            start(); init = true;
        }

        update();
    }
}