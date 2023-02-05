using static SadExperiments.MainScreen.Container;

namespace SadExperiments.MainScreen;

/// <summary>
/// Page counter displayed in the top right corner of the <see cref="Header"/>.
/// </summary>
class PageCounter : ScreenSurface
{
    #region Constants
    const string Title = "Page:";

    // char displayed between _index and _total
    const string Devider = "/";

    // maximum number of digits in either _index or _total
    const int MaxNumberLength = 2;
    #endregion Constants

    #region Fields
    // text printed when a number is invalid
    string _hyphens = new('-', MaxNumberLength);

    // position for the 'index' number
    readonly Point _indexPosition = (1, 1);

    // position for the 'total' number
    readonly Point _pageCountPosition;

    // total number of pages
    int _pageCount;
    #endregion Fields

    #region Constructors
    public PageCounter() : base(Title.Length + 2, 2)
    {
        // set default colors
        Surface.SetDefaultColors(Header.FGColor, Header.BGColor);

        // print title
        Surface.Print(Point.Zero, Title.Align(HorizontalAlignment.Center, Surface.Width), Color.Yellow);

        // print devider
        var deviderPosition = _indexPosition + (MaxNumberLength, 0);
        Surface.Print(deviderPosition, Devider, Color.Yellow);

        // calculate position for the 'total' number
        _pageCountPosition = deviderPosition + (Devider.Length, 0);
    }
    #endregion Constructors

    #region Methods
    // prepends number with 0s to make the total string length of MaxNumberLength
    static string FormatNumber(int number)
    {
        string result = number.ToString();
        if (result.Length > MaxNumberLength)
            throw new ArgumentException("Number of digits in the argument exceeds MaxNumberLength.", nameof(number));
        string padding = new('0', MaxNumberLength - result.Length);
        return padding + result;
    }

    // prints hyphens at the given position
    void PrintHyphens(Point position)
    {
        Surface.Print(position, _hyphens);
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

    public void Container_OnPageChanged(object? sender, EventArgs args)
    {
        if (Root.CurrentPage is ContentsList)
        {
            PrintHyphens(_indexPosition);
        }
        else
        {
            // add 1 to offset the zero index
            int pageNumber = Root.CurrentPage.Index + 1;

            // check if the page number is between limits
            if (pageNumber < 1 || pageNumber > _pageCount)
                throw new IndexOutOfRangeException($"Invalid page number: {pageNumber}.");

            // print the number to the surface
            string index = FormatNumber(pageNumber);
            Surface.Print(_indexPosition, index);
        }
    }

    public void Container_OnPageListChanged(object? sender, EventArgs args)
    {
        _pageCount = Root.PageList.Length;

        if (_pageCount == 0)
        {
            PrintHyphens(_indexPosition);
            PrintHyphens(_pageCountPosition);
        }
        else
        {
            string pageCount = FormatNumber(_pageCount);
            Surface.Print(_pageCountPosition, pageCount);
        }
    }
    #endregion Methods
}