using SadConsole.Entities;

namespace SadExperiments.Games.Tetris;

class BlockManager : Renderer
{
    public void Add(Tetromino t)
    {
        foreach (Entity block in t.Blocks)
            Add(block);
    }

    public void Remove(Tetromino t)
    {
        foreach (Entity block in t.Blocks)
        {
            if (Entities.Contains(block))
                Remove(block);
        }
    }
}