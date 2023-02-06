using SadExperiments;

namespace SadExperiments.Pages;

class BinaryOperationsPage : Page
{
    public BinaryOperationsPage()
    {
        Title = "Binary Operations";
        Summary = "Handy reminder of bit operations in C#.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[] { Tag.SadConsole, Tag.StringParser, Tag.Bits };

        AddCentered(new BinaryOperations());

        Surface.Print(3, ColoredString.Parser.Parse("Check out also SadConsole's [c:r f:lightgreen]Helpers[c:undo] " +
            "class when working with bits."));
        string color = "orange";
        Surface.Print(5, ColoredString.Parser.Parse($"It features useful [c:r f:{color}]SetFlag[c:undo], " +
            $"[c:r f:{color}]UnsetFlag[c:undo] and [c:r f:{color}]HasFlag[c:undo] methods."));
    }
}

class BinaryOperations : SubPage
{
    int _y = 1;

    public BinaryOperations() : base(Program.Width / 2, Program.Height / 2)
    {
        byte x = 0b1001;
        byte y = 0b1010;

        // show binary OR, XOR and AND
        Print("Variable y", GetStringInBinary(x));
        Print("Variable x", GetStringInBinary(y));
        _y++;
        Print("Inclusive OR", GetStringInBinary(x | y) + " x | y");
        Print("Exclusive OR", GetStringInBinary(x ^ y) + " x ^ y");
        Print("Binary AND", GetStringInBinary(x & y) + " x & y");
        _y++;

        // show binary left and right shift
        x = 0b1000;
        y = 2;
        Print("Variable x", GetStringInBinary(x));
        Print("Variable y", y.ToString());
        _y++;
        Print("Left Shift", GetStringInBinary(x << y) + " x << y");
        Print("Right Shift", GetStringInBinary(x >> y) + " x >> y");
    }

    void Print(string l, string r) =>
        Surface.Print(1, _y++, $"{l.Align(HorizontalAlignment.Right, 13)}: {r}");

    static string GetStringInBinary(int n)
    {
        string output = string.Empty;
        for (int position = 7; position >= 0; position--)
            output += PrintBitAt((byte)n, position);
        return output;
    }
    static string PrintBitAt(byte b, int position)
    {
        if (position > 7) return string.Empty;
        byte a = (byte)Math.Pow(2, position);
        return (b & a) == a ? "1" : "0";
    }
}