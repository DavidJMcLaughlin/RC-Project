using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.CLI.Graphics
{
    public class PreviousPositionRenderer : RobotStatusRenderer
    {
        public PreviousPositionRenderer(RobotStatus status) : base(status)
        {
        }

        public bool LimitOutputToAvailableHeight { get; set; } = true;

        public override void Draw()
        {
            int startingLeftIndex = Console.CursorLeft;
            int maxItemsToDraw = Program.RobotStatus.PreviousPositions.Count;
            if (this.LimitOutputToAvailableHeight)
            {
                maxItemsToDraw = (Console.WindowHeight - Console.CursorTop) - 2;
            }
            maxItemsToDraw = ((maxItemsToDraw < Program.RobotStatus.PreviousPositions.Count) ? maxItemsToDraw : Program.RobotStatus.PreviousPositions.Count);

            Console.Write("Previous {0} moves:", maxItemsToDraw);
            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);

            for (int i = (Program.RobotStatus.PreviousPositions.Count - maxItemsToDraw); i < Program.RobotStatus.PreviousPositions.Count; i++)
            {
                Console.Write("Robot: {0} ", this.Status.PreviousPositions[i].RobotPosition);
                Console.Write(this.Status.PreviousPositions[i].TileEncountered.GetType().Name);
                Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);
            }
        }
    }
}
