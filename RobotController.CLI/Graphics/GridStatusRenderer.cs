using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.CLI.Graphics
{
    public class GridStatusRenderer : BaseConsoleRenderer
    {
        public override void Draw()
        {
            ConsoleColor startingColor = Console.ForegroundColor;

            int startingLeftIndex = Console.CursorLeft;

            string spacer = "  ";

            Console.Write("Grid: ");
            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);
            Console.Write(spacer + "Size: {0}x{1}", Program.Grid.Bounds.Width, Program.Grid.Bounds.Height);
            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);
            Console.Write(spacer + "Total tiles: {0}", (Program.Grid.Bounds.Width * Program.Grid.Bounds.Height));
            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);
            Console.Write(spacer + "Total obstructions: {0}", Program.Grid.Obstructions.Count);

            Console.ForegroundColor = ConsoleColor.DarkGray;

            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);
            Console.Write(spacer + "[1] to regenerate obstructions");

            Console.ForegroundColor = startingColor;
        }
    }
}
