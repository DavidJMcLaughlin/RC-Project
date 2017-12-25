using RobotController.Grid.Tile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.CLI.Graphics
{
    public class RobotStatusRenderer : BaseConsoleRenderer
    {
        public RobotStatusRenderer(RobotStatus status)
        {
            this.Status = status;
        }

        public RobotStatus Status { get; private set; }

        public override void Draw()
        {
            ConsoleColor startingColor = Console.ForegroundColor;
            int startingLeftIndex = Console.CursorLeft;

            string spacer = "  ";

            Console.Write("Robot: ");
            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);

            Console.Write(spacer + "Last move successful: ");
            Console.ForegroundColor = (this.Status.WasLastMoveSuccessful ? ConsoleColor.Green : ConsoleColor.Red);
            Console.Write("{0}", this.Status.WasLastMoveSuccessful);
            Console.ForegroundColor = startingColor;

            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);
            Console.Write(spacer + "Position: {0}", Program.RobotInstance.CurrentPosition);
            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);
            Console.Write(spacer + "Last obstruction: ");

            Type tileType = this.Status.LastTileEncountered.Tile.GetType();

            if (tileType == typeof(RockTile))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Rock");
            }
            else if (tileType == typeof(SpinnerTile))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                SpinnerTile st = (SpinnerTile)this.Status.LastTileEncountered.Tile;
                Console.Write("Spinner [{0}]", st.SpinAmount);
            }
            else if (tileType == typeof(HoleTile))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                HoleTile ht = (HoleTile)this.Status.LastTileEncountered.Tile;
                Console.Write("Hole to {0}", (Program.RobotInstance.CurrentPosition.Location == ht.ConnectedLocation ? ht.ConnectedLocation : ht.StartLocation));
            }

            Console.ForegroundColor = startingColor;
        }
    }
}
