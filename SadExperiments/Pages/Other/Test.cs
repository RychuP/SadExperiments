using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadExperiments.Pages;

internal class Test : Page
{
    public Test()
    {
        Title = "Template";
        Summary = "TemplateSummary";
        Submitter = Submitter.Rychu;
        Tags = new Tag[]
        {

        };

        var console = new ControlsConsole(Width, Height) { Parent = this };

        var listbox = new ListBox(20, 6)
        {
            Position = new Point(28, 3)
        };
        listbox.Items.Add("item 1");
        listbox.Items.Add("item 2");
        listbox.Items.Add("item 3");
        listbox.Items.Add("item 4");
        listbox.Items.Add("item 5");
        listbox.Items.Add("item 6");
        listbox.Items.Add("item 7");
        listbox.Items.Add("item 8");
        console.Controls.Add(listbox);

        listbox.Resize(10, 4);
    }
}