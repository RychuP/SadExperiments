namespace SadExperiments.MainScreen;

/// <summary>
/// Page counter displayed in the top right corner of the <see cref="Header"/>.
/// </summary>
class PageCounter : ScreenSurface
{
    const string Title = "Page:";
    const string Devider = "/";
    const int MaxNumberLength = 2;

    readonly Point _titlePosition = Point.Zero;
    readonly Point _indexPosition = (1, 1);
    readonly Point _totalPosition;

    int _total;

    public PageCounter(int pageCount) : base(Title.Length + 2, 2)
    {
        // set default colors
        Surface.SetDefaultColors(Header.FGColor, Header.BGColor);

        // print title
        Surface.Print(_titlePosition, Title.Align(HorizontalAlignment.Center, Surface.Width), Color.Yellow);

        // print devider
        var maxNumberOffset = (MaxNumberLength, 0);
        Surface.Print(_indexPosition + maxNumberOffset, Devider, Color.Yellow);

        // set total
        _totalPosition = _indexPosition + maxNumberOffset + (Devider.Length, 0);
        Total = pageCount;
    }

    /// <summary>
    /// Total number of available pages.
    /// </summary>
    /// <exception cref="ArgumentException">In case the number is negative.</exception>
    public int Total
    {
        get => _total;
        set
        {
            if (value < 0) 
                throw new ArgumentException("Total cannot be negative.", nameof(value));
            _total = value;

            // print total
            string total = FormatNumber(value);
            Surface.Print(_totalPosition, total);
        }
    }

    // prepends number with 0s to make the total string length of MaxNumberLength
    string FormatNumber(int number)
    {
        string result = number.ToString();
        if (result.Length > MaxNumberLength)
            throw new ArgumentException("Number of digits in the argument exceeds MaxNumberLength.", nameof(number));
        string padding = new string('0', MaxNumberLength - result.Length);
        return padding + result;
    }

    /// <summary>
    /// Displays the given <see cref="Page"/> index.
    /// </summary>
    /// <param name="pageNumber"><see cref="Page"/> index to display.</param>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public void ShowIndex(int pageNumber)
    {
        if (pageNumber == -1)
        {
            string dummyIndex = new('-', MaxNumberLength);
            Surface.Print(_indexPosition, dummyIndex);
        }

        else
        {
            // add 1 to offset the zero index
            pageNumber += 1;

            // check if the page number is between limits
            if (pageNumber < 1 || pageNumber > Total)
                throw new IndexOutOfRangeException($"Invalid page number: {pageNumber}.");

            // print the number to the surface
            string index = FormatNumber(pageNumber);
            Surface.Print(_indexPosition, index);
        }
    }

    // minimizes header if the mouse falls outside the header zone
    protected override void OnMouseExit(MouseScreenObjectState state)
    {
        if (Parent is Header header)
        {
            // create the mouse state for the parent and check if the mouse is still on it
            var parentState = new MouseScreenObjectState(header, state.Mouse);
            if (!parentState.IsOnScreenObject)
                header.Minimize();
        }
        base.OnMouseExit(state);
    }

    // maximizes header since the page counter is part of it
    protected override void OnMouseEnter(MouseScreenObjectState state)
    {
        if (Parent is Header header && header.IsMinimized)
            header.Maximize();
        base.OnMouseEnter(state);
    }
}