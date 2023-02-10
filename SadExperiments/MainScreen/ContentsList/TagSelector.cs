using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using System.Diagnostics;

namespace SadExperiments.MainScreen;

[DebuggerDisplay("TagSelector")]
class TagSelector : OptionSelector
{
    ListBox _editListBox;

    public TagSelector(Point position, int w, int h) : base("Filter by tag:", position, w, h)
    {
        if (Game.Instance.Screen is Container container)
            container.TagsChanged += Container_OnTagsChanged;

        _editListBox = new(Width - 2, Height - MinimizedHeight - 2);
        _editListBox.Position = (1, MinimizedHeight + 1);
        _editListBox.SingleClickItemExecute = true;

        // register event handlers
        TextBox.EditingTextChanged += TextBox_OnEditTextChanged;
        // TODO: fix incremental search when EditModeExit/EditModeEnter events are fixed
        TextBox.EditModeExit += TextBox_OnEditModeExit;
        TextBox.EditModeEnter += TextBox_OnEditModeEnter;
        _editListBox.SelectedItemExecuted += (o, e) => ListBox.Execute(e.Item);
    }

    protected override void OnMouseLeftClicked(MouseScreenObjectState state)
    {
        base.OnMouseLeftClicked(state);
        PopulateEditListBoxWithTags();
    }

    protected virtual void TextBox_OnEditModeEnter(object? o, EventArgs e)
    {
        if (Controls.Contains(ListBox))
            ReplaceListBox();
    }

    protected virtual void TextBox_OnEditModeExit(object? o, EventArgs e)
    {
        if (Controls.Contains(_editListBox))
        {
            Controls.Remove(_editListBox);
            Controls.Add(ListBox);
        }
    }

    // ugly hacks to make the list box working with faulty EditModeExit/EditModeEnter events
    protected virtual void TextBox_OnEditTextChanged(object? o, EventArgs e)
    {
        if (Controls.Contains(ListBox))
        {
            if (_editListBox.SelectedItem is null)
                ReplaceListBox();
        }
        else if (Controls.Contains(_editListBox))
        {
            if (_editListBox.SelectedItem is null)
            {
                _editListBox.Items.Clear();
                string editingText = TextBox.EditingText.ToLower();
                foreach (var tag in Container.Instance.Tags)
                {
                    string tagText = tag.ToString().ToLower();
                    if (tagText.Contains(editingText))
                        _editListBox.Items.Add(tag);
                }
            }
        }
    }

    void ReplaceListBox()
    {
        Controls.Remove(ListBox);
        Controls.Add(_editListBox);
    }

    void PopulateEditListBoxWithTags()
    {
        _editListBox.Items.Clear();
        foreach (var tag in Container.Instance.Tags)
            _editListBox.Items.Add(tag);
    }

    protected virtual void Container_OnTagsChanged(object? o, EventArgs e)
    {
        var selectedItem = ListBox.SelectedItem;
        DisplayTags();
        SetSelectedItem(selectedItem);
    }

    void DisplayTags()
    {
        ListBox.Items.Clear();
        foreach (Tag tag in Container.Instance.Tags)
            ListBox.Items.Add(tag);
    }

    void SetSelectedItem(object? selectedItem)
    {
        if (selectedItem is Tag tag)
            if (Container.Instance.Tags.Contains(tag))
                ListBox.SelectedItem = tag;
    }
}

class MyTextBox : TextBox
{
    static MyTextBox() =>
        Library.Default.SetControlTheme(typeof(MyTextBox), new TextBoxTheme());

    public MyTextBox(int width) : base(width)
    {
        EditModeEnter += (o, e) => EditingText = string.Empty;
    }
}

class MyListBox : ListBox
{
    static MyListBox() =>
        Library.Default.SetControlTheme(typeof(MyListBox), new ListBoxTheme(new ScrollBarTheme()));

    public MyListBox(int w, int h) : base(w, h) { }

    public void Execute(object? selectedItem)
    {
        if (Items.Contains(selectedItem))
        {
            SelectedItem = selectedItem;
            OnItemAction();
        }
    }
}