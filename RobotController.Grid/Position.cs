using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Grid
{
    /// <summary>
    /// Stores location and rotation
    /// </summary>
    public struct Position
    {
        public Position(int xPosition, int yPosition, CardinalDirection direction) : this(new Point(xPosition, yPosition), direction)
        {
        }
        public Position(Point location, CardinalDirection direction)
        {
            this.Location = location;
            this.Direction = direction;
        }

        public Point Location { get; set; }
        public CardinalDirection Direction { get; set; }

        public int X { get { return this.Location.X; } }
        public int Y { get { return this.Location.Y; } }
    }
}
