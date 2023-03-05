namespace SadExperiments.Games.PacMan;

class PowerUp : Dot
{
    // two frame animation data
    readonly TimeSpan _animationFreq = TimeSpan.FromSeconds((double)20 / 60);
    TimeSpan _timeElapsed = TimeSpan.Zero;
    int _currentAnimFrame = 0;
    readonly int _glyph = 37;

    public PowerUp(Point position) : base(position)
    {
        Value = 2;
        Appearance.Glyph = _glyph;
    }

    public override void Update(TimeSpan delta)
    {
        _timeElapsed += delta;
        if (_timeElapsed >= _animationFreq)
        {
            _timeElapsed = TimeSpan.Zero;
            Appearance.Glyph = _currentAnimFrame == 0 ? _glyph : Appearances.Floor.Glyph;
            _currentAnimFrame = _currentAnimFrame == 0 ? 1 : 0;
        }
        base.Update(delta);
    }
}