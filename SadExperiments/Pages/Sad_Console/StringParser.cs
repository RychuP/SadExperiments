using SadExperiments;

namespace SadExperiments.Pages.Sad_Console;

// string parser https://sadconsole.com/v8/articles/string-parser.html
internal class StringParser : Page
{
    public StringParser()
    {
        Title = "String Parser";
        Summary = "Thraka's tutorial on string parser from version 8.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.StringParser, Tag.Color };

        int row = 3;

        this.Print(1, row++, ColoredString.Parser.Parse("[c:r f:Aqua]Strings with commands embedded"));
        Print();

        row += 4;
        UsePrintProcessor = true;
        this.Print(1, row++, "[c:r f:Aqua]Strings when run through the processor");
        Print();

        void Print()
        {
            row++;
            this.Print(1, row++, "Normal [c:r f:Blue][c:r b:Yellow]and colored");
            this.Print(1, row++, "Normal [c:r f:0,255,0][c:r b:128,128,0,255]and colored");
            this.Print(1, row++, "Normal [c:r f:0,0,255,128][c:r b:255,255,0]and [c:r f:0,0,255,64]co[c:r f:0,0,255,32]lo[c:r f:0,0,255,16]red");
            this.Print(1, row++, "[c:r f:red]Colored and [c:r f:x,128,x]adjusted");
            this.Print(1, row++, "Normal and [c:r f:blue][c:r b:yellow]colored and[c:undo] undo [c:undo]text");
        }
    }
}