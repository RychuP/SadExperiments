﻿namespace SadExperiments.Games.PacMan.Ghosts.Behaviours;

interface IEatenBehaviour
{
    Destination RunBackHome(Board board, Point ghostPosition, Direction ghostDirection);
}
