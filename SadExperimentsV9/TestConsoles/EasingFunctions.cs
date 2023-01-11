using SadConsole.Components;
using SadConsole.EasingFunctions;

namespace SadExperimentsV9.TestConsoles;

class EasingFunctions : TestConsole
{
    double _value = 0;
    readonly Linear _lerp = new() { Mode = EasingMode.In };
    readonly Quad _quad = new() { Mode = EasingMode.Out };
    TimeSpan _time,
        _totalTime = TimeSpan.FromSeconds(3);
    bool _isRunning = true;
    readonly Cursor _cursor;

    public EasingFunctions()
    {
        _cursor = new Cursor(Surface);
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
            _cursor.Print($"{_value: 0.00}");
        }
    }
}