using System;

public class soundStorage
{
    public static Sound button = Engine.LoadSound("SFX/Button.wav");
    public static Sound build = Engine.LoadSound("SFX/Building_Upgrade_2.mp3");
    public static Sound endTurn = Engine.LoadSound("SFX/endturn.wav");
    public static Sound[] footsteps = {Engine.LoadSound("SFX/movement2.mp3"),
                                       Engine.LoadSound("SFX/movement3.mp3")};
    public static Sound[] snowballs = {Engine.LoadSound("SFX/Snowball Impact 2.wav"),
                                       Engine.LoadSound("SFX/Snowball Impact 3.wav")};

    public static Sound newCity = Engine.LoadSound("SFX/newCity.mp3");
    public static Sound explored = Engine.LoadSound("SFX/explored.mp3");
    public static Sound capturedCity = Engine.LoadSound("SFX/captured.mp3");
    public static Sound newUnit = Engine.LoadSound("SFX/buyUnit.mp3");
    public static Sound techUnlocked = Engine.LoadSound("SFX/techUnlock.mp3");
    public static Sound unitKilled = Engine.LoadSound("SFX/killUnit.wav");

    public static Sound returnRandomSound(Sound[] sounds)
    {
        Random random = new Random();
        int randomSound = random.Next(0, sounds.Length);
        return sounds[randomSound];
    }

    public static Music music = Engine.LoadMusic("SFX/nachtmusic.mp3");

}