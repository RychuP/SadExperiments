namespace SadExperiments;

/// <summary>
/// Time counter useful for delaying code in <see cref="IScreenObject"/> Update/Render.
/// </summary>
class DeltaTime
{
    public TimeSpan Treshold { get; set; } = TimeSpan.Zero;
    public TimeSpan Current { get; set; } = TimeSpan.Zero;
    public DeltaTime(double target) => Treshold = TimeSpan.FromSeconds(target);
    public void Add(TimeSpan ts)
    {
        Current += ts;
        if (Current >= Treshold)
        {
            Zero(); 
            OnTresholdReached();
        }
    }

    protected virtual void OnTresholdReached()
    {
        TresholdReached?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? TresholdReached;
    public void Zero() => Current = TimeSpan.Zero;
}