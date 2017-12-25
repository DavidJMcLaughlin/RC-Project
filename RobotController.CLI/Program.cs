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

            do
            {
                Console.WriteLine("X={0}, Y={1}, D={2}", Program.RobotInstance.CurrentPosition.X, Program.RobotInstance.CurrentPosition.Y, Program.RobotInstance.CurrentPosition.Direction);

                keyInfo = Console.ReadKey(true);

                Program.RobotController.QueueCommand(new RobotMoveCommand(char.ToUpper(keyInfo.KeyChar).ToString()));
                Program.RobotController.RunNextCommand();
            }
            while (keyInfo.Key != ConsoleKey.Escape);
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
        }

        public static void SetupController()
        {
            List<Point> emptyPoints = Simple2DGrid.GetAllEmptyPoints(Program.Grid);
            Point randomStartingPoint = emptyPoints[randGen.Next(0, emptyPoints.Count)];

            Position startingPosition = new Position(randomStartingPoint, CardinalDirection.North);

            Program.RobotInstance = new SimpleRobot(Program.Grid, startingPosition);

            Program.RobotController = new RobotOperator(Program.RobotInstance);
            Program.RobotController.RegisterCommandInterpreter(RobotMoveCommand.COMMAND_ID, new MovementCommandInterpreter());
        }
    }
}
