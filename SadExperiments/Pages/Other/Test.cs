using SadConsole.Components;

namespace SadExperiments.Pages;

internal class Test : Page
{
    public Test()
    {
        var timer = new Timer(TimeSpan.FromSeconds(0.1d));
        SadComponents.Add(timer);
        timer.TimerElapsed += Timer_OnTimerElapsed;
    }

    void Timer_OnTimerElapsed(object? sender, EventArgs e)
    {
        if (sender is Timer t)
            t.IsPaused = true;
    }
}