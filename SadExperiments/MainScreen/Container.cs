﻿using SadConsole.UI.Controls;
using SadConsole.UI.Windows;
using SadConsole.UI;
using SadExperiments.Pages;
using SadExperiments.Pages.Sad_Console;
using SadExperiments.Pages.Primitives;
using SadExperiments.Pages.Sad_Canvas;
using SadExperiments.Pages.Go_Rogue;

namespace SadExperiments.MainScreen;

/// <summary>
/// Main program class that coordinates display of pages and headers.
/// </summary>
class Container : ScreenObject
{
    /// <summary>
    /// Lists all available, predefined colors and allows to pick a custom one.
    /// </summary>
    public static ColorPickerPopup ColorPicker { get; } = new();

    /// <summary>
    /// Lists characters available in the default font.
    /// </summary>
    public static CharacterViewer CharacterViewer { get; } = new(1);

    readonly ContentsList _contentsList;
    readonly Header _header;
    Page _currentPage;

    readonly Page[] _pages =
    {
        new WelcomePage(),
        new AnimatedGlobe(),
        new Donut3dPage(),
        new A_Demo(),
        new LineAlgorithms(),
        new RectangleBisection(),
        new AreaPage(),
        new SplashScreens(),
        new SmoothScrolling(),
        new RectangleManipulation(),
        new StringParser(),
        new FontChanging(),
        new KeyboardAndMouse(),
        new BasicDrawing(),             // tutorial part 1
        new CursorPage(),               // tutorial part 2.1
        new OverlappingConsoles(),      // tutorial part 2.2
        new KeyboardProcessing(),
        new UpdateAndRender(),
        new BinaryOperationsPage(),
        new CharsAndCursors(),
        new MovableCharacter(),
        new MoveAndResize(),
        new FocusSetting(),
        new SurfaceShifting(),
        new CheckeredRoom(),
        new PixelNoise(),
        new CellSurfaceResizing(),
        new EasingFunctions(),
        new ImageConversion(),
        new SadCanvasPage(),
        new Instructions(),
        new EffectsAndDecorators(),
    };

    static Container()
    {
        ColorPicker.FontSize *= 0.9;
        ColorPicker.Center();
        ColorPicker.SelectedColor = Color.White;
        CharacterViewer.Center();
    }

    public Container()
    {
        // set first page as focused
        var firstPage = _pages[0];
        firstPage.IsFocused = true;

        // create a header
        _header = new(firstPage, _pages.Length);

        // instantiate data fields
        _contentsList = new ContentsList(ButtonsWithLinksToPages);
        _currentPage = firstPage;

        // remove starting console
        Game.Instance.Screen = this;
        Game.Instance.DestroyDefaultStartingConsole();

        // add children
        Children.Add(_contentsList, _currentPage, _header);

        // set page indices
        int i = 0;
        Array.ForEach(_pages, p => p.Index = i++);
    }

    public override void Update(TimeSpan delta)
    {
        var keyboard = Game.Instance.Keyboard;
        if (keyboard.HasKeysPressed)
        {
            if (ColorPicker.IsVisible)
            {
                // keep it seperate
                if (keyboard.IsKeyPressed(Keys.F4))
                    ColorPicker.Hide();
            }
            else if (CharacterViewer.IsVisible)
            {
                // keep it seperate
                if (keyboard.IsKeyPressed(Keys.F5))
                    CharacterViewer.Hide();
            }
            else
            {
                if (keyboard.IsKeyPressed(Keys.F1))
                    PrevPage();
                else if (keyboard.IsKeyPressed(Keys.F2))
                    NextPage();
                else if (keyboard.IsKeyPressed(Keys.F3))
                    ToggleContentsList();
                else if (keyboard.IsKeyPressed(Keys.F4))
                    ColorPicker.Show(true);
                else if (keyboard.IsKeyPressed(Keys.F5))
                    CharacterViewer.Show(true);
            }
        }
        base.Update(delta);
    }

    public void NextPage() => ChangePage(Direction.Right);

    public void PrevPage() => ChangePage(Direction.Left);

    void ChangePage(Direction direction)
    {
        if (_contentsList.IsVisible) 
            HideContentsList();

        int nextIndex = _currentPage.Index + (direction == Direction.Right ? 1 : -1);
        var page = nextIndex < 0              ? _pages.Last() :
                   nextIndex >= _pages.Length ? _pages.First() :
                                                _pages[nextIndex];
        ChangePage(page);
    }

    void ChangePage(Page page)
    {
        Children.Remove(_currentPage);
        Children.Add(page);
        _currentPage = page;
        _currentPage.IsFocused = true;
        _header.SetHeader(page);
        Children.MoveToTop(_header);
        if (page is IRestartable p)
            p.Restart();
    }

    public void ToggleContentsList()
    {
        if (!_contentsList.IsVisible)
        {
            // hide current page
            Children.MoveToBottom(_currentPage);

            // show contents list
            _contentsList.Show();

            // update header
            _contentsList.Index = _currentPage.Index;
            _header.SetHeader(_contentsList);
        }
        else HideContentsList();
    }

    void HideContentsList()
    {
        _contentsList.Hide();

        // focus and show current page
        Children.MoveToBottom(_contentsList);
        _currentPage.IsFocused = true;

        // restart page if possible
        if (_currentPage is IRestartable p)
            p.Restart();

        // update header
        _header.SetHeader(_currentPage);
    }

    ControlsConsole ButtonsWithLinksToPages
    {
        get
        {
            var contentsList = new ControlsConsole(Program.Width, Program.Height - TagFilters.MinimizedHeight);
            contentsList.Position = (0, TagFilters.MinimizedHeight);
            Point position = (1, 1);
            int buttonWidth = 35;

            foreach (Page page in _pages)
            {
                // create new button with a link to the page
                var button = new Button(buttonWidth, 1)
                {
                    Text = page.Title,
                    Position = position,
                    UseMouse = true,
                    UseKeyboard = false,
                };
                button.Click += (o, e) =>
                {
                    HideContentsList();
                    ChangePage(page);
                };
                contentsList.Controls.Add(button);

                // increment position
                position += Direction.Down;

                // if first column is full, start the second column
                if (position.Y == contentsList.Height - 2)
                    position = (Program.Width - buttonWidth - 1, 1);
            }
            return contentsList;
        }
    }
}