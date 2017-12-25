using RobotController.Grid;
using RobotController.Grid.Tile;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.CLI
{
    public class RobotStatus
    {
        public bool WasLastMoveSuccessful { get; set; }
        public TileEventArgs LastTileEncountered { get; set; } = new TileEventArgs(new EmptyTile(Point.Empty));

        public List<RobotMovementData> PreviousPositions { get; private set; } = new List<RobotMovementData>();
    }
}
