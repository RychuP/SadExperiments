using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace SadExperiments.Games.Tetris;

static class Sounds
{
    public static SoundEffect Beep = ReadFile("turn.wav");
    public static SoundEffect Drop = ReadFile("drop.wav");
    public static SoundEffect Move = ReadFile("move.wav");

    static SoundEffect ReadFile (string fileName)
    {
        string path = Path.Combine("Resources", "Sounds", fileName);
        var bytes = File.ReadAllBytes(path);
        return new SoundEffect(bytes, 44100, AudioChannels.Stereo);
    }
}
