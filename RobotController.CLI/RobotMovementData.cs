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
    public class RobotMovementData
    {
        public RobotMovementData(Position position, BaseTile tileEncountered)
        {
            this.RobotPosition = position;
            this.TileEncountered = tileEncountered;
        }

        public Position RobotPosition { get; private set; }
        public Point Location { get { return this.RobotPosition.Location; } }
        public BaseTile TileEncountered { get; private set; }
    }
}
