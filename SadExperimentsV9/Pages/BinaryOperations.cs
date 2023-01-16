﻿namespace SadExperiments.Pages;

class BinaryOperationsPage : Page
{
    public BinaryOperationsPage()
    {
        Title = "Binary Operations";
        Summary = "Examples of binary operations on bytes.";
        AddCentered(new BinaryOperations());

        Surface.Print(3, "Nothing to do with SadConsole functionality.");
        Surface.Print(5, "Just me playing with bits in C#.");
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

    void Print(string l, string r) => Surface.Print(1, _y++, $"{l.Align(HorizontalAlignment.Right, 13)}: {r}");

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