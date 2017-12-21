using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Grid.Tile
{
    public class EmptyTile : BaseTile
    {
        public EmptyTile(Point location) : base(location, EmptyTile.TILE_ID)
        {
        }
        public EmptyTile(int xStartLocation, int yStartLocation) : base(xStartLocation, yStartLocation, EmptyTile.TILE_ID)
        {
        }

        public const int TILE_ID = 0;
    }
}
