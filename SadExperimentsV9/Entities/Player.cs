using SadConsole.Entities;
using SadRogue.Primitives;

namespace SadExperimentsV9
{
    class Player : Entity
    {
        public Player() : base(Color.Yellow, Color.DarkGray, 1, 1)
        {
            Position = (4, 4);
        }

        public void MoveTo(Point p)
        {
            Position = p;
        }

        public Point GetNextMove(Point direction)
        {
            return Position.Translate(direction);
        }
    }
}