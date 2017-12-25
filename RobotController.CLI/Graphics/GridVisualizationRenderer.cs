using RobotController.Grid;
using RobotController.Grid.Tile;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.CLI.Graphics
{
    public class GridVisualizationRenderer : BaseConsoleRenderer
    {
        public int ViewSize { get; set; } = 7;

        public override void Draw()
        {
            int startingLeftIndex = Console.CursorLeft;
            int startingTopIndex = Console.CursorTop;

            List<BaseTile> tiles = new List<BaseTile>();

            Point center = Program.RobotInstance.CurrentPosition.Location;

            Point start = new Point(center.X - (this.ViewSize / 2), center.Y + (this.ViewSize / 2));

            for (int y = 0; y < this.ViewSize; y++)
            {
                for (int x = 0; x < this.ViewSize; x++)
                {
                    Point current = new Point(start.X + x, start.Y - y);
                    BaseTile tile = Program.Grid.GetTileAtLocation(current);

                    Console.SetCursorPosition(startingLeftIndex + x, startingTopIndex + y);

                    Type tileType = tile.GetType();

                    if (x == (this.ViewSize / 2) && (y == this.ViewSize / 2))
                    {
                        if (Program.RobotInstance.CurrentPosition.Direction == CardinalDirection.North)
                        {
                            Console.Write("┴");
                        }
                        else if (Program.RobotInstance.CurrentPosition.Direction == CardinalDirection.East)
                        {
                            Console.Write("├");
                        }
                        else if (Program.RobotInstance.CurrentPosition.Direction == CardinalDirection.South)
                        {
                            Console.Write("┬");
                        }
                        else if (Program.RobotInstance.CurrentPosition.Direction == CardinalDirection.West)
                        {
                            Console.Write("┤");
                        }
                    }
                    else if (tileType == typeof(RockTile))
                    {
                        Console.Write("R");
                    }
                    else if (tileType == typeof(SpinnerTile))
                    {
                        Console.Write("S");
                    }
                    else if (tileType == typeof(HoleTile))
                    {
                        Console.Write("H");
                    }
                    else if (Program.Grid.Bounds.Contains(current))
                    {
                        Console.Write("░");
                    }
                }
            }
        }
    }
}
