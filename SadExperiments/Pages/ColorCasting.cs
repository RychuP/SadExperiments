namespace SadExperiments.Pages;

internal class ColorCasting : Page
{
    public ColorCasting()
    {
        Title = "Color Casting";
        Summary = "Casting colors to byte spans.";
        Submitter = Submitter.Rychu;

        Color[] colors = new Color[]
            {
                Color.Red,
                Color.Green,
                Color.Blue,
                Color.White,
                Color.Black,
                Color.Transparent
            };

        // dummy byte array
        byte[] pixels = new byte[colors.Length * 4];

        // convert colors to a byte span
        Span<byte> byteSpan = System.Runtime.InteropServices.MemoryMarshal.AsBytes(colors.AsSpan());

        // check the length with the dummy array
        bool numberOfBytesIsEqual = pixels.Length == byteSpan.Length;

        // convert the byte span back to array of colors
        var colors2 = System.Runtime.InteropServices.MemoryMarshal.Cast<byte, Color>(byteSpan).ToArray();

        // test colors
        var appearance = new ColoredGlyph(colors2[0], colors2[1]);
        this.Print(1, 1, "Test of colors.", appearance);
    }
}