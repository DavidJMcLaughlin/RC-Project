using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Grid.Tile
{
    /// <summary>
    /// This tile represents an obstacle that cannot be passed
    /// </summary>
    public class RockTile : BaseTile
    {
        public RockTile(Point location) : base(location, RockTile.TILE_ID)
        {
        }
        public RockTile(int xStartLocation, int yStartLocation) : base(xStartLocation, yStartLocation, RockTile.TILE_ID)
        {
        }

        public const int TILE_ID = 1;
    }
}
