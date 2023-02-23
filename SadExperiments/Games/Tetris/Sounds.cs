using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace SadExperiments.Games.Tetris;

static class Sounds
{
    public static SoundEffectInstance Start, Lost, Rotate, Load, Plant, Line, LevelUp;

    static Sounds()
    {
        Start = FromFile("start");
        Lost = FromFile("lost");
        Rotate = FromFile("rotate");
        Load = FromFile("load");
        Plant = FromFile("plant");
        Line = FromFile("line");
        LevelUp = FromFile("levelup");
}

    static SoundEffectInstance FromFile(string fileName)
    {
        string path = Path.Combine("Resources", "Sounds", "Tetris", $"{fileName}.wav");
        return SoundEffect.FromFile(path).CreateInstance();
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
