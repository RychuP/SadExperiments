using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace SadExperiments.Games.PacMan;

static class Sounds
{
    public static SoundEffectInstance Siren, Munch, Death, LevelComplete;

    static Sounds()
    {
        Siren = FromFile("siren");
        Munch = FromFile("munch");
        Death = FromFile("death");
        LevelComplete = FromFile("level_complete");
    }

    static SoundEffectInstance FromFile(string fileName)
    {
        string path = Path.Combine("Resources", "Sounds", "PacMan", $"{fileName}.wav");
        return SoundEffect.FromFile(path).CreateInstance();
    }

    public static void StopAll()
    {
        Siren!.Stop();
        Munch!.Stop();
        Death!.Stop();
        LevelComplete!.Stop();
    }
}