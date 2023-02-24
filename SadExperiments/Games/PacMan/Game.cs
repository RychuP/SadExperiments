using Microsoft.VisualBasic;
using System.IO;

namespace SadExperiments.Games.PacMan;

// game logic inspired by the article at https://pacman.holenet.info/
class Game : Page
{
    public Game()
    {
        // read the blueprint file
        string path = Path.Combine("Resources", "Other", "PacMan", "Maze.txt");
        var text = File.ReadAllText(path);
        string[] lines = text.Split("\r\n");
        int width = lines[0].Length;
        int height = lines.Length;

        // create tiles
        var tiles = new Tile[width * height];
        int i = 0;
        foreach (string row in lines)
        {
            foreach (char c in row)
            {
                Point position = Point.FromIndex(i, width);
                tiles[i++] = c == '.' ? new Floor(position) : new Wall(position);
            }
        }

        //var board = new Board();
    }
}