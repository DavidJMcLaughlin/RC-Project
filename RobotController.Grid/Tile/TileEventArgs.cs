using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Grid.Tile
{
    public class TileEventArgs : EventArgs
    {
        public TileEventArgs(BaseTile tile)
        {
            this.Tile = tile;
        }

        public BaseTile Tile { get; private set; }
    }
}
