using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Grid.Tile
{
    /// <summary>
    /// This tile will try to rotate an entity that steps on to it
    /// </summary>
    public class SpinnerTile : BaseTile
    {
        public SpinnerTile(Point location) : base(location, SpinnerTile.TILE_ID)
        {
        }
        public SpinnerTile(int xStartLocation, int yStartLocation) : base(xStartLocation, yStartLocation, SpinnerTile.TILE_ID)
        {
        }
        public SpinnerTile(int xStartLocation, int yStartLocation, Rotation spinAmount) : base(xStartLocation, yStartLocation, SpinnerTile.TILE_ID)
        {
            this.SpinAmount = spinAmount;
        }

        public const int TILE_ID = 3;

        public Rotation SpinAmount { get; set; } = Rotation.None;
    }
}
