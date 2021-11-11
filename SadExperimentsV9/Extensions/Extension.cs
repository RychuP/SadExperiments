using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadExperimentsV9
{
    public static class Extensions
    {
        /// <summary>
        /// Prints text centered on the surface.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <param name="y"></param>
        /// <param name="c"></param>
        public static void Print(this ScreenSurface s, string t, int y, Color? c = null) => 
            s.Surface.Print(0, y, t.Align(HorizontalAlignment.Center, s.Surface.Width), c ?? s.Surface.DefaultForeground);


        /// <summary>
        /// Prints text using <see cref="TheDrawFont"/> and horizontal alignment specified. Calculates x coordinate. Truncates string to fit it in one line.
        /// </summary>
        /// <param name="cellSurface">Class implementing <see cref="ICellSurface"/>.</param>
        /// <param name="y">Y coordinate of the surface.</param>
        /// <param name="text">Text to print.</param>
        /// <param name="drawFont">Instance of the <see cref="TheDrawFont"/> to use.</param>
        /// <param name="alignment"><see cref="HorizontalAlignment"/> to use.</param>
        /// <param name="padding">Amount of regular font characters used as horizontal padding on both sides of the output.</param>
        public static void PrintTheDraw(this ICellSurface cellSurface, int y, string text, TheDrawFont drawFont, HorizontalAlignment alignment, int padding = 0)
        {
            if (drawFont is null) return;

            int spaceWidth = GetTheDrawSpaceCharWidth(drawFont),
                textLength = 0,
                printWidth = cellSurface.Width - padding * 2;
            string tempText = string.Empty;

            foreach (var item in text)
            {
                char currentChar = item;
                int charWidth = 0;

                if (drawFont.IsCharacterSupported(item))
                {
                    var charInfo = drawFont.GetCharacter(currentChar);
                    charWidth = charInfo.Width;
                }
                else
                {
                    currentChar = ' ';
                    charWidth = spaceWidth;
                }

                textLength += charWidth;

                if (textLength > printWidth)
                {
                    textLength -= charWidth;
                    break;
                }

                tempText += currentChar;
            }

            int x = alignment switch
            {
                HorizontalAlignment.Center => (printWidth - textLength) / 2,
                HorizontalAlignment.Right => printWidth - textLength,
                _ => 0
            };

            PrintTheDraw(cellSurface, x + padding, y, tempText, drawFont);
        }

        static int GetTheDrawSpaceCharWidth(TheDrawFont drawFont) => drawFont.IsCharacterSupported(' ') ? drawFont.GetCharacter(' ').Width :
                                                                     drawFont.IsCharacterSupported('a') ? drawFont.GetCharacter('a').Width :
                                                                     drawFont.IsCharacterSupported('i') ? drawFont.GetCharacter('i').Width :
                                                                     drawFont.IsCharacterSupported('1') ? drawFont.GetCharacter('1').Width :
                                                                     2;

        /// <summary>
        /// Prints text using <see cref="TheDrawFont"/>.
        /// </summary>
        /// <param name="cellSurface">Class implementing <see cref="ICellSurface"/>.</param>
        /// <param name="x">X coordinate of the surface.</param>
        /// <param name="y">Y coordinate of the surface.</param>
        /// <param name="text">Text to print.</param>
        /// <param name="drawFont">Instance of the <see cref="TheDrawFont"/> to use.</param>
        public static void PrintTheDraw(this ICellSurface cellSurface, int x, int y, string text, TheDrawFont drawFont)
        {
            if (drawFont is null) return;

            int xPos = x;
            int yPos = y;
            int tempHeight = 0;

            foreach (var item in text)
            {
                if (drawFont.IsCharacterSupported(item))
                {
                    var charInfo = drawFont.GetCharacter(item);

                    if (xPos + charInfo.Width >= cellSurface.Width)
                    {
                        yPos += tempHeight + 1;
                        xPos = 0;
                    }

                    if (yPos >= cellSurface.Height)
                        break;

                    var surfaceCharacter = drawFont.GetSurface(item);

                    if (surfaceCharacter != null)
                    {
                        surfaceCharacter.Copy(cellSurface, xPos, yPos);

                        if (surfaceCharacter.Height > tempHeight)
                            tempHeight = surfaceCharacter.Height;
                    }

                    xPos += charInfo.Width;
                }
                else if (item == ' ')
                {
                    xPos += GetTheDrawSpaceCharWidth(drawFont);
                }
            }
        }
    }
}
