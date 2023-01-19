namespace SadExperiments.MainScreen;

internal class Header : ScreenSurface
{
    public const int Height = 2;
    readonly PageCounter _pageCounter;

    public Header(Page page, int pageCount) : base(Program.Width, Height)
    {
        Surface.DefaultBackground = Color.DarkGray.GetDarker();
        Surface.DefaultForeground = Color.Yellow;
        _pageCounter = new PageCounter(pageCount) { Parent = this };
        _pageCounter.Position = (Surface.Width - _pageCounter.Surface.Width, 0);
        SetHeader(page);
    }

    public void SetHeader(Page page)
    {
        Surface.Clear();
        Surface.Print(1, 0, page.Title.ToUpper());
        Surface.Print(1, 1, page.Summary, Color.White);
        _pageCounter.DisplayPageNumber(page.Index + 1);
    }
}

internal class PageCounter : ScreenSurface
{
    const string Title = "Page:";
    readonly int _pageCount;

    public PageCounter(int pageCount) : base(Title.Length + 2, Header.Height)
    {
        Surface.DefaultBackground = Color.DarkGray.GetDarker();
        Surface.DefaultForeground = Color.Yellow;
        Surface.Clear();
        _pageCount = pageCount;
        Surface.Print(0, 0, Title.Align(HorizontalAlignment.Center, Surface.Width));
    }

    public void DisplayPageNumber(int pageNumber)
    {
        if (pageNumber < 1 || pageNumber > _pageCount) throw new IndexOutOfRangeException($"Page number: {pageNumber} is not valid)");
        Surface.Print(0, 1, $"{pageNumber: 00}/{_pageCount:00}", Color.White);
    }
}