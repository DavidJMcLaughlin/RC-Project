using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Grid.Tile
{
    public abstract class BaseTile
    {
        public BaseTile(int xStartLocation, int yStartLocation, int id) : this(new Point(xStartLocation, yStartLocation), id)
        {
        }
        public BaseTile(Point location, int id)
        {
            this.StartLocation = location;
            this.Id = id;
        }

        public Point StartLocation { get; set; } = Point.Empty;
        public int Id { get; private set; } = int.MinValue;

        /// <summary>
        /// Checks if this tile is at a given location
        /// </summary>
        public virtual bool IsAtLocation(Point location)
        {
            return (location == this.StartLocation);
        }
    }
}
