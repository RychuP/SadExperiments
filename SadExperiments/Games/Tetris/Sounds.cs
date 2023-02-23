using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace SadExperiments.Games.Tetris;

static class Sounds
{
    public static SoundEffectInstance Start, Lost, Rotate, Load, Plant, Line, LevelUp;

    static Sounds()
    {
        Start = FromFile("start").CreateInstance();
        Lost = FromFile("lost").CreateInstance();
        Rotate = FromFile("rotate").CreateInstance();
        Load = FromFile("load").CreateInstance();
        Plant = FromFile("plant").CreateInstance();
        Line = FromFile("line").CreateInstance();
        LevelUp = FromFile("levelup").CreateInstance();
}

    static SoundEffect FromFile(string fileName)
    {
        string path = Path.Combine("Resources", "Sounds", "Tetris", $"{fileName}.wav");
        return SoundEffect.FromFile(path);
    }

    public static void StopAll()
    {
        Start!.Stop();
        Lost!.Stop();
        Rotate!.Stop();
        Load!.Stop();
        Plant!.Stop();
        Line!.Stop();
        LevelUp!.Stop();
    }
}
