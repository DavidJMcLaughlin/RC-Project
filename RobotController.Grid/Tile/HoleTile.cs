using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Grid.Tile
{
    /// <summary>
    /// This tile represents a hole in the ground and can be connected with another location to make a portal
    /// </summary>
    public class HoleTile : BaseTile
    {
        public HoleTile(Point location) : base(location, HoleTile.TILE_ID)
        {
        }
        public HoleTile(Point startLocation, Point connectedLocation) : base(startLocation, HoleTile.TILE_ID)
        {
            this.ConnectedLocation = connectedLocation;
        }
        public HoleTile(int xStartLocation, int yStartLocation) : base(xStartLocation, yStartLocation, HoleTile.TILE_ID)
        {
        }
        public HoleTile(int xStartLocation, int yStartLocation, int xConnectedLocation, int yConnectedLocation) : base(xStartLocation, yStartLocation, HoleTile.TILE_ID)
        {
            this.ConnectedLocation = new Point(xConnectedLocation, yConnectedLocation);
        }

        public const int TILE_ID = 2;

        public Point ConnectedLocation { get; set; } = Point.Empty;

        public override bool IsAtLocation(Point location)
        {
            return (base.IsAtLocation(location) || (location == this.ConnectedLocation));
        }
    }
}
