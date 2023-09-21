public class MainMenu : Scene
{
    public MainMenu() {}

    public override void start()
    {
        SDL2.SDL_mixer.Mix_VolumeMusic(80);
        Engine.PlayMusic(soundStorage.music);
        MenuLoader.mainMenu();
    }

    public override void update()
    {
        
    }
}