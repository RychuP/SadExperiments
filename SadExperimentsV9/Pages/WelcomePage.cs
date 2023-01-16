namespace SadExperiments.Pages;

internal class WelcomePage : Page
{
    public WelcomePage()
    {
        Title = "SadConsole Experiments";
        Summary = "Tests of various features of SadConsole and related libraries.";

        Surface.Print(3, "Press F1 or F2 to navigate between screens.");
        Surface.Print(5, "Arrow and other keys can also be used to interact with some content.");
        Surface.Print(7, "For reference, page counter is in the top right corner.");
        Surface.Print(11, "Not everything here is presented as best practice.");
        Surface.Print(13, "These are my own attempts at learning the library");
        Surface.Print(15, "that I am sharing with the community. Enjoy.");
    }
}