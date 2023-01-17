namespace SadExperiments.Pages;

internal class WelcomePage : Page
{
    public WelcomePage()
    {
        Title = "Welcome Page";
        Summary = "Tests of various features of SadConsole and related libraries.";

        Surface.Print(3, "Press F1 or F2 to navigate between screens.");
        Surface.Print(5, "Press F3 to display the list of contents.");
        Surface.Print(7, "Use arrow keys, space button, etc to interact with individual pages.");
        Surface.Print(9, "For reference, page counter is in the top right corner.");
        Surface.Print(13, "Take everything with a pinch of salt.");
        Surface.Print(15, "These are my own attempts at learning the library");
        Surface.Print(17, "not necessarily the examples of best practice.");
    }
}