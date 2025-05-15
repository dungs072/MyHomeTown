using System.Collections.Generic;

public class AudioKey
{
    // just add Audio key here (1)
    public const string MUSIC = "MUSIC";
    public const string SOUND = "SOUND";

    // just add Audio key here (2)
    public static List<string> GetAllAudioKeys()
    {
        return new List<string>
        {
            MUSIC,
            SOUND
        };
    }
}
public class AudioPath
{
    public const string AUDIO_PATH = "Assets/Audio/Sound/";

}
