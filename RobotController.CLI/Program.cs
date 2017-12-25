using RobotController.Command;
using RobotController.Command.Processor;
using RobotController.Grid;
using RobotController.Grid.Generator;
using RobotController.Grid.Tile;
using RobotController.Operator;
using RobotController.Robot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.CLI
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Robot controller";

            Program.GenerateGrid();
            Program.SetupController();

            ConsoleKeyInfo keyInfo;

            Console.WriteLine("Robot: {0}", Program.RobotInstance.StartingPosition);

            do
            {
                keyInfo = Console.ReadKey(true);

                Program.RobotController.QueueCommand(new RobotMoveCommand(char.ToUpper(keyInfo.KeyChar).ToString()));
                Program.RobotController.RunNextCommand();
            }
            while (keyInfo.Key != ConsoleKey.Escape);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }

        private static Random randGen = new Random();
        private static SimpleGridGenerator gGenerator;
        public static Simple2DGrid Grid;
        public static RobotOperator RobotController;
        public static SimpleRobot RobotInstance;

        public static void GenerateGrid()
        {
            Program.gGenerator = new SimpleGridGenerator(new Size(128, 128));
            Program.gGenerator.AvailableObstrctions.Add(new HoleTile(Point.Empty));
            Program.gGenerator.AvailableObstrctions.Add(new RockTile(Point.Empty));
            Program.gGenerator.AvailableObstrctions.Add(new SpinnerTile(Point.Empty));

            Program.Grid = gGenerator.Generate();

            Console.WriteLine("New grid created: ");
            Console.WriteLine("  Bounds: {0}", Program.Grid.Bounds);
            Console.WriteLine("  Obstructions: {0} of {1}", Program.Grid.Obstructions.Count, (Program.Grid.Bounds.Width * Program.Grid.Bounds.Height));
            Console.WriteLine();
        }

        public static void SetupController()
        {
            List<Point> emptyPoints = Simple2DGrid.GetAllEmptyPoints(Program.Grid);
            Point randomStartingPoint = emptyPoints[randGen.Next(0, emptyPoints.Count)];

            Position startingPosition = new Position(randomStartingPoint, CardinalDirection.North);

            Program.RobotInstance = new SimpleRobot(Program.Grid, startingPosition);
            Program.RobotInstance.TileEncountered += RobotInstance_TileEncountered;
            Program.RobotInstance.PositionChanged += RobotInstance_PositionChanged;

            Program.RobotController = new RobotOperator(Program.RobotInstance);
            Program.RobotController.RegisterCommandInterpreter(RobotMoveCommand.COMMAND_ID, new MovementCommandInterpreter());
            Program.RobotController.CommandProcessedSuccessfully += RobotController_CommandProcessedSuccessfully;
            Program.RobotController.CommandProcessedUnsuccessfully += RobotController_CommandProcessedUnsuccessfully;
        }

        private static void RobotController_CommandProcessedUnsuccessfully(object sender, EventArgs e)
        {
            Console.WriteLine("Controller: Unknown command");
        }

        private static void RobotController_CommandProcessedSuccessfully(object sender, RobotCommandEventArgs e)
        {
            Console.WriteLine("Controller: Processed command [id={0}, data={1}] at {2}", e.Command.Id, e.Command.Data, e.Timestamp.ToShortTimeString());
        }

        private static void RobotInstance_PositionChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Robot: from {0} to {1}", Program.RobotInstance.PreviousPosition, Program.RobotInstance.CurrentPosition);
        }

        private static void RobotInstance_TileEncountered(object sender, TileEventArgs e)
        {
            Type tileType = e.Tile.GetType();

            if (tileType == typeof(RockTile))
            {
                Console.WriteLine("Unable to move to position because of a rock");
            }
            else if (tileType == typeof(SpinnerTile))
            {
                SpinnerTile st = (SpinnerTile)e.Tile;
                Console.WriteLine("Encountered a spinner set to [{0}]", st.SpinAmount);
            }
            else if (tileType == typeof(HoleTile))
            {
                HoleTile ht = (HoleTile)e.Tile;
                Console.WriteLine("Encountered a hole connected to {0}", ht.ConnectedLocation);
            }
        }
    }
}
