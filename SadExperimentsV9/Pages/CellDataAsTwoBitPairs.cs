namespace SadExperiments.Pages;

internal class CellDataAsTwoBitPairs : Page
{
    public CellDataAsTwoBitPairs()
    {
        Title = "Cell Data As Two Bit Pairs";
        Summary = "Storing cell data as two-bit pairs and accessing it with Point coordinates.";

        // cells are two bit pairs (01 - wall, 10 - color, 00 - empty)
        byte[] data = new byte[]
        {
                0b01000000,     // these 6 bytes hold data for 24 cells ->
                0b00100000,     // in this example a 6x4 grid
                0b00001000,
                0b01000000,
                0b00100000,
                0b00001000
        };

        // flags for testing the two-bit pairs
        int[][] flags = new int[][]
        {
                new int[] { 128, 64},
                new int[] { 32, 16 },
                new int[] { 8, 4 },
                new int[] { 2, 1 }
        };

        // define the conversion rate from 1d array to 2d array
        int width = 6;
        float dataWidth = (float)width / 4;

        // define point to test
        Point p = (0, 2);

        // convert point to two-bit location in data
        float i = p.Y * dataWidth + (float)p.X / 4;
        int pointer = (int)Math.Floor(i);
        float reminder = i - pointer;
        int cellNumber = Convert.ToInt32(reminder / 0.25f);
        byte b = data[pointer];
        int[] flag = flags[cellNumber];

        // pull cell content
        var cellContent = Helpers.HasFlag(b, flag[1]) ? 1 : Helpers.HasFlag(b, flag[0]) ? 2 : 0;
    }
}