using System;

using System.Drawing;

namespace GameContracts
{
    public interface IGameRules
    {
        public bool Isoclines(Point start, Point end);
    }
}

