﻿using SadConsole.UI.Controls;
using System.Diagnostics;
using static SadConsole.UI.Controls.ListBox;

namespace SadExperiments.MainScreen;

/// <summary>
/// Page filter based on tags.
/// </summary>
[DebuggerDisplay("Filter")]
class Filter : ScreenSurface
{
    #region Constants
    public const int MinimizedHeight = 4;
    public const int MaximizedHeight = Program.Height / 2;
    #endregion Constants

    #region Fields
    readonly TagSelector _tag1Selector;
    readonly TagSelector _tag2Selector;
    readonly SortMethodSelector _sortOrderSelector;
    #endregion Fields

    #region Constructors
    public Filter() : base(Program.Width, MinimizedHeight, Program.Width, MaximizedHeight)
    {
        Surface.SetDefaultColors(Header.FGColor, Header.BGColor);

        // calculate tag selector dimensions
        int spacer = 4;
        int noOfControls = 3;
        int height = Surface.Height - 1;
        int width = (Surface.Width - noOfControls * spacer) / noOfControls;

        // create tag selectors
        Point position = (spacer, 1);
        _tag1Selector = new(position, width, height);
        position += (width + spacer, 0);
        _tag2Selector = new(position, width, height);
        position += (width + spacer, 0);
        _sortOrderSelector = new(position, width, height);

        // add tag selectors to children
        Children.Add(_tag1Selector, _tag2Selector, _sortOrderSelector);

        // register event handlers
        if (Game.Instance.Screen is Container container)
            container.PageChanged += (o, e) => Minimize();
        foreach (var child in Children)
        {
            if (child is not OptionSelector os) return;
            os.TextBox.EditModeEnter += (o, e) => Maximize();

            if (os is TagSelector ts)
            {
                ts.ListBox.SelectedItemExecuted += (o, e) => FilterPages();
                ts.ClearButton.Click += (o, e) => FilterPages();
            }
            else if (os is SortMethodSelector sms)
            {
                sms.ListBox.SelectedItemExecuted += (o, e) =>
                    Container.Instance.SortPages((SortMethod)sms.ListBox.SelectedItem);
                sms.ClearButton.Click += (o, e) => 
                    Container.Instance.SortPages();
            }
        }
    }
    #endregion Constructors

    #region Properties
    /// <summary>
    /// Checks if the filter height is equal to MinimizedHeight.
    /// </summary>
    public bool IsMinimized =>
        Surface.ViewHeight == MinimizedHeight;
    #endregion Properties

    #region Methods
    /// <summary>
    /// Collapses top section of the console and hides tag selection.
    /// </summary>
    public void Minimize()
    {
        if (!IsMinimized)
        {
            Surface.ViewHeight = MinimizedHeight;
            foreach (var selector in Children)
                (selector as OptionSelector)?.Minimize();
            Minimized?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Expands top section of the console to show tag selection.
    /// </summary>
    public void Maximize()
    {
        if (IsMinimized)
        {
            Surface.ViewHeight = MaximizedHeight;
            foreach (var selector in Children)
                (selector as OptionSelector)?.Maximize();
            Maximized?.Invoke(this, EventArgs.Empty);
        }
    }

    public void SetTag(Tag tag)
    {
        _tag1Selector.ClearButton.InvokeClick();
        _tag2Selector.ClearButton.InvokeClick();
        _tag1Selector.ListBox.Execute(tag);
    }

    void ListBox_OnSelectedItemExecuted(object? sender, SelectedItemEventArgs args)
    {
        
    }

    void FilterPages()
    {
        var tag1 = (Tag?)_tag1Selector.ListBox.SelectedItem;
        var tag2 = (Tag?)_tag2Selector.ListBox.SelectedItem;
        Container.Instance.FilterPagesByTags(tag1, tag2);
    }

    // minimize on pressing esc key while filter is displayed
    public override void Update(TimeSpan delta)
    {
        var keyboard = Game.Instance.Keyboard;
        if (keyboard.HasKeysPressed && keyboard.IsKeyPressed(Keys.Escape))
            Minimize();

        base.Update(delta);
    }

    public void DisableKeyboardInTextBoxes(OptionSelector? exception = null)
    {
        foreach (var child in Children)
        {
            if (child is OptionSelector os)
            {
                if ( (exception is OptionSelector specialCase && os != specialCase) ||
                     (exception is null) )
                    os.TextBox.DisableKeyboard = true;
            }
        }
    }
    #endregion Methods

    #region Events
    public event EventHandler? Minimized;
    public event EventHandler? Maximized;
    #endregion Events
}