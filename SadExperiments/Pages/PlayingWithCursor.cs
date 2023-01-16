namespace SadExperiments.Pages;

// http://sadconsole.com/v9/articles/tutorials/getting-started/part-2-cursor-parents.html
internal class PlayingWithCursor : Page
{
    public PlayingWithCursor()
    {
        Title = "Playing With Cursor";
        Summary = "Following Thraka's tutorials: part 2, cursors.";

        Cursor.PrintAppearanceMatchesHost = false;
        Cursor.Move(3, 3)
            .Print("Kato is my favorite dog.")
            .NewLine()
            .Down(1)
            .SetPrintAppearance(Color.LightGreen)
            .Right(3)
            .Print("No, Birdie is my favorite dog.")
            .NewLine()
            .Down(1)
            .SetPrintAppearance(Color.LightPink)
            .Right(3)
            .Print("What's yours?")
            .NewLine()
            .Down(1)
            .SetPrintAppearance(Color.White)
            .Right(3)
            .Print(">")
            .Right(1);

        Cursor.IsVisible = true;
        Cursor.IsEnabled = true;
    }
}