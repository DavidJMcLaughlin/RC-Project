using Newtonsoft.Json;
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.CLI
{
    /// <summary>
    /// A very quick and dirty console app for demonstration purposes.
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Robot controller - David McLaughlin";

            Program.ProcessCommandLineArgs(args);

            if (Program.OperationMode != ModeOfOperation.CommandLineMode)
            {
                Program.SetupInteractiveMode();
            }
            else if (!string.IsNullOrEmpty(Program.GridPath))
            {
                if (Program.Serialize)
                {
                    Program.GenerateGrid();
                    Program.SaveGridToFile(Program.GridPath);

                    Console.WriteLine("Grid saved to '{0}'", Program.GridPath);
                }
                else if (!string.IsNullOrEmpty(Program.CommandText))
                {
                    Program.LoadGridFromFile(Program.GridPath);
                    Program.SetupController();
                    Program.RunMoveCommandOnController(Program.CommandText);

                    Program.DrawRobotPreviousPositions();
                }
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }

        private const int GRID_WIDTH = 128;
        private const int GRID_HEIGHT = 128;

        private static ModeOfOperation OperationMode = ModeOfOperation.InteractiveSingleCommand;

        private static string GridPath = string.Empty;
        private static string CommandText = string.Empty;

        private static bool Serialize = false;

        private static Random randGen = new Random();
        private static SimpleGridGenerator gGenerator;

        public static Simple2DGrid Grid;
        public static RobotOperator RobotController;
        public static SimpleRobot RobotInstance;

        public static RobotOperatorStatus OperatorStatus = new RobotOperatorStatus();
        public static RobotStatus RobotStatus = new RobotStatus();

        private static void GenerateGrid()
        {
            Program.gGenerator = new SimpleGridGenerator(new Size(Program.GRID_WIDTH, Program.GRID_HEIGHT));
            Program.gGenerator.AvailableObstrctions.Add(new HoleTile(Point.Empty));
            Program.gGenerator.AvailableObstrctions.Add(new RockTile(Point.Empty));
            Program.gGenerator.AvailableObstrctions.Add(new SpinnerTile(Point.Empty));

            Program.Grid = gGenerator.Generate();

            Console.WriteLine("New grid created: ");
            Console.WriteLine("  Bounds: {0}", Program.Grid.Bounds);
            Console.WriteLine("  Obstructions: {0} of {1}", Program.Grid.Obstructions.Count, (Program.Grid.Bounds.Width * Program.Grid.Bounds.Height));
            Console.WriteLine();
        }

        private static void SetupController()
        {
            List<Point> emptyPoints = Simple2DGrid.GetAllEmptyPoints(Program.Grid);
            Point randomStartingPoint = emptyPoints[randGen.Next(0, emptyPoints.Count)];

            Position startingPosition = new Position(randomStartingPoint, CardinalDirection.North);

            Program.RobotInstance = new SimpleRobot(Program.Grid, startingPosition);
            Program.RobotInstance.TileEncountered += RobotInstance_TileEncountered;
            Program.RobotInstance.PositionChanged += RobotInstance_PositionChanged;
            Program.RobotInstance.UnableToChangePosition += RobotInstance_UnableToChangePosition;

            Program.RobotController = new RobotOperator(Program.RobotInstance);
            Program.RobotController.RegisterCommandInterpreter(RobotMoveCommand.COMMAND_ID, new MovementCommandInterpreter());
            Program.RobotController.CommandProcessedSuccessfully += RobotController_CommandProcessedSuccessfully;
            Program.RobotController.CommandProcessedUnsuccessfully += RobotController_CommandProcessedUnsuccessfully;
            Program.RobotController.AcknowledgeCommandReceived += RobotController_AcknowledgeCommandReceived;
        }

        private static void SetupInteractiveMode()
        {
            Program.GenerateGrid();
            Program.SetupController();

            Console.WriteLine("Robot: {0}", Program.RobotInstance.StartingPosition);

            Program.WaitForInput();
        }

        private static void ProcessCommandLineArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "-grid":
                        Program.GridPath = Program.GetNextArgOrDefault(i, args);
                        Program.OperationMode = ModeOfOperation.CommandLineMode;
                        break;

                    case "-command":
                        Program.CommandText = Program.GetNextArgOrDefault(i, args).ToUpper();
                        break;

                    case "-serialize":
                        Program.Serialize = true;
                        break;
                }
            }
        }

        private static string GetNextArgOrDefault(int index, string[] args)
        {
            if ((index + 1) < args.Length)
            {
                return args[index + 1];
            }

            return string.Empty;
        }

        private static void LoadGridFromFile(string path)
        {
            if (File.Exists(path))
            {
                string fileText = File.ReadAllText(path);
                Simple2DGrid grid = JsonConvert.DeserializeObject<Simple2DGrid>(fileText, new JsonSerializerSettings()
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    TypeNameHandling = TypeNameHandling.All
                });

                Program.Grid = grid;
            }
        }

        private static void SaveGridToFile(string path)
        {
            string text = JsonConvert.SerializeObject(Program.Grid, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            });
            File.WriteAllText(path, text);
        }

        private static void WaitForInput()
        {
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(true);

                Program.SetStateFromKey(keyInfo);

                switch (Program.OperationMode)
                {
                    case ModeOfOperation.InteractiveSingleCommand:
                        Program.ProcessSingleCommand(keyInfo);
                        break;

                    case ModeOfOperation.InteractiveMultipleCommand:
                        Program.ProcessMultipleCommands();
                        break;
                }

                Program.DrawProgramOutput();
            }
            while (keyInfo.Key != ConsoleKey.Escape);
        }

        private static void SetStateFromKey(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.D1:
                    Program.RegenerateObstructions();
                    break;

                case ConsoleKey.LeftArrow:
                    if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                    {
                        Program.RobotInstance.Rotate(Rotation.CCW90);
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                    {
                        Program.RobotInstance.Rotate(Rotation.CW90);
                    }
                    break;
            }
        }

        private static void ProcessSingleCommand(ConsoleKeyInfo keyInfo)
        {
            Console.WriteLine("Press a key to move: ");

            string commandText = char.ToUpper(keyInfo.KeyChar).ToString();
            Program.RunMoveCommandOnController(commandText);
        }

        private static void ProcessMultipleCommands()
        {
            Console.Write("Please type your commands here: ");

            string commandText = Console.ReadLine().ToUpper();

            Program.RunMoveCommandOnController(commandText);
        }

        private static void RunMoveCommandOnController(string data)
        {
            RobotCommand command = new RobotMoveCommand(data);

            Program.RobotController.QueueCommand(command);
            Program.RobotController.RunNextCommand();
        }

        private static void RegenerateObstructions()
        {
            List<BaseTile> obstructions = Program.gGenerator.GenerateObstructions(Program.Grid);

            Program.Grid.Obstructions.Clear();
            Program.Grid.Obstructions.AddRange(obstructions);

            DrawProgramOutput();
        }

        private static void DrawProgramOutput()
        {
            int screenDivision = (Console.WindowWidth / 3);

            Console.Clear();

            Console.SetCursorPosition(0, 0);
            Program.DrawRobotStatus();

            Console.SetCursorPosition(screenDivision, 0);
            Program.DrawControllerStatus();

            Console.SetCursorPosition((screenDivision * 2), 0);
            Program.DrawGridStatus();

            Console.SetCursorPosition(0, (Console.WindowHeight / 2));
            Program.DrawRobotPreviousPositions();

            Console.SetCursorPosition((Console.WindowWidth / 2) - 2, (Console.WindowHeight / 2));
            Program.DrawSurroundings();

            Console.SetCursorPosition(screenDivision * 2, (Console.WindowHeight / 2));
        }

        private static void DrawControllerStatus()
        {
            int startingLeftIndex = Console.CursorLeft;

            string spacer = "  ";

            Console.Write("Controller: ");
            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);
            Console.Write(spacer + "Last command recieved:");
            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);
            Console.Write(spacer + spacer + "Id: {0}", Program.OperatorStatus.LastCommandReceived.Command.Id);
            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);
            Console.Write(spacer + spacer + "Data: {0}", Program.OperatorStatus.LastCommandReceived.Command.Data);
            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);
            Console.Write(spacer + spacer + "Timestamp: {0}", Program.OperatorStatus.LastCommandReceived.Timestamp.ToLongTimeString()); 
        }

        private static void DrawRobotStatus()
        {
            ConsoleColor startingColor = Console.ForegroundColor;
            int startingLeftIndex = Console.CursorLeft;

            string spacer = "  ";

            Console.Write("Robot: ");
            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);

            Console.Write(spacer + "Last move successful: ");
            Console.ForegroundColor = (Program.RobotStatus.WasLastMoveSuccessful ? ConsoleColor.Green : ConsoleColor.Red);
            Console.Write("{0}", Program.RobotStatus.WasLastMoveSuccessful);
            Console.ForegroundColor = startingColor;

            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);
            Console.Write(spacer + "Position: {0}", Program.RobotInstance.CurrentPosition);
            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);
            Console.Write(spacer + "Last obstruction: ");

            Type tileType = Program.RobotStatus.LastTileEncountered.Tile.GetType();

            if (tileType == typeof(RockTile))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Rock");
            }
            else if (tileType == typeof(SpinnerTile))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                SpinnerTile st = (SpinnerTile)Program.RobotStatus.LastTileEncountered.Tile;
                Console.Write("Spinner [{0}]", st.SpinAmount);
            }
            else if (tileType == typeof(HoleTile))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                HoleTile ht = (HoleTile)Program.RobotStatus.LastTileEncountered.Tile;
                Console.Write("Hole to {0}", (Program.RobotInstance.CurrentPosition.Location == ht.ConnectedLocation ? ht.ConnectedLocation : ht.StartLocation));
            }

            Console.ForegroundColor = startingColor;
        }

        private static void DrawGridStatus()
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

        private static void DrawRobotPreviousPositions()
        {
            int startingLeftIndex = Console.CursorLeft;
            int maxItemsToDraw = (Console.WindowHeight - Console.CursorTop) - 2;
            maxItemsToDraw = ((maxItemsToDraw < Program.RobotStatus.PreviousPositions.Count) ? maxItemsToDraw : Program.RobotStatus.PreviousPositions.Count);

            Console.Write("Previous {0} moves:", maxItemsToDraw);
            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);

            for (int i = (Program.RobotStatus.PreviousPositions.Count -  maxItemsToDraw); i < Program.RobotStatus.PreviousPositions.Count; i++)
            {
                Console.Write("Robot: {0}", Program.RobotStatus.PreviousPositions[i]);
                Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);
            }
        }

        private static void DrawSurroundings()
        {
            int startingLeftIndex = Console.CursorLeft;
            int startingTopIndex = Console.CursorTop;

            int viewSize = 7;

            List<BaseTile> tiles = new List<BaseTile>();

            Point center = Program.RobotInstance.CurrentPosition.Location;

            Point start = new Point(center.X - (viewSize / 2), center.Y + (viewSize / 2));

            for (int y = 0; y < viewSize; y++)
            {
                for (int x = 0; x < viewSize; x++)
                {
                    Point current = new Point(start.X + x, start.Y - y);
                    BaseTile tile = Program.Grid.GetTileAtLocation(current);

                    Console.SetCursorPosition(startingLeftIndex + x, startingTopIndex + y);

                    Type tileType = tile.GetType();

                    if (x == (viewSize / 2) && (y == viewSize / 2))
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

        private static void RobotController_AcknowledgeCommandReceived(object sender, RobotCommandEventArgs e)
        {
            Program.OperatorStatus.LastCommandReceived = e;
        }

        private static void RobotController_CommandProcessedUnsuccessfully(object sender, EventArgs e)
        {
            Console.WriteLine("Controller: Unknown command");
        }

        private static void RobotController_CommandProcessedSuccessfully(object sender, RobotCommandEventArgs e)
        {
            Program.OperatorStatus.LastSuccessfullyProcessedCommand = e;
        }

        private static void RobotInstance_PositionChanged(object sender, EventArgs e)
        {
            Program.RobotStatus.PreviousPositions.Add(Program.RobotInstance.CurrentPosition);
            Program.RobotStatus.WasLastMoveSuccessful = true;
        }

        private static void RobotInstance_TileEncountered(object sender, TileEventArgs e)
        {
            Program.RobotStatus.LastTileEncountered = e;
        }

        private static void RobotInstance_UnableToChangePosition(object sender, TileEventArgs e)
        {
            Program.RobotStatus.WasLastMoveSuccessful = false;
        }
    }
}
