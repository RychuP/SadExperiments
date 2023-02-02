using SadConsole.Components;
using SadConsole.EasingFunctions;

namespace SadExperiments.Pages.Sad_Console;

class EasingFunctions : Page
{
    double _value = 0;
    readonly Linear _lerp = new() { Mode = EasingMode.In };
    readonly Quad _quad = new() { Mode = EasingMode.Out };
    TimeSpan _time,
        _totalTime = TimeSpan.FromSeconds(3);
    bool _isRunning = true;

    public EasingFunctions()
    {
        Title = "Easing Functions";
        Summary = "Testing how the values change in the easing functions.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.Easing };

        Cursor.NewLine();
    }

    public override void Update(TimeSpan delta)
    {
        if (_isRunning)
        {
            _time += delta;
            if (_time >= _totalTime)
            {
                _time = _totalTime;
                _isRunning = false;
            }

            //_value = _lerp.Ease(_time.TotalMilliseconds, 0, 10, _totalTime.TotalMilliseconds);
            _value = _quad.Ease(_time.TotalMilliseconds, 20, -5, _totalTime.TotalMilliseconds);
            Cursor.Print($"{_value: 0.00}");

            if (!_isRunning)
                Cursor.NewLine().Down(1).Print("Quad Easing Function");
        }
    }
}