using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace SadExperiments.Games.PacMan;

static class Sounds
{
    public static SoundEffectInstance Siren, PowerDot, MunchDot, MunchGhost, Death, LevelComplete;

    static Sounds()
    {
        Siren = FromFile("siren");
        PowerDot = FromFile("power_dot");
        MunchDot = FromFile("munch_dot");
        MunchGhost = FromFile("munch_ghost");
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
        PowerDot!.Stop();
        MunchDot!.Stop();
        MunchGhost!.Stop();
        Death!.Stop();
        LevelComplete!.Stop();
    }
}