using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using SadConsole.Entities;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadExperiments.UI;
using System.IO;

namespace SadExperiments.Pages;

internal class Test : Page
{
    public Test()
    {
        string path = Path.Combine("Resources", "Sounds", "turn.wav");
        var bytes = File.ReadAllBytes(path);
        var beep = new SoundEffect(bytes, 44100, AudioChannels.Stereo);
        beep.Play();
    }
}