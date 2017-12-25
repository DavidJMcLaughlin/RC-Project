﻿using Newtonsoft.Json;
using RobotController.CLI.Graphics;
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
            Program.InitializeRenderers();

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
                    Program.positionRenderer.LimitOutputToAvailableHeight = false;
                    Program.LoadGridFromFile(Program.GridPath);
                    Program.SetupController();
                    Program.RunMoveCommandOnController(Program.CommandText);

                    Program.OutputRenderer.PreviousPositionRenderer.Draw();
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

        private static ProgramOutputRenderer OutputRenderer = new ProgramOutputRenderer();

        private static RobotStatusRenderer rStatusRenderer;
        private static GridStatusRenderer gStatusRenderer;
        private static ControllerStatusRenderer cStatusRenderer;
        private static GridVisualizationRenderer visualizationRenderer;
        private static PreviousPositionRenderer positionRenderer;

        public static Simple2DGrid Grid;
        public static RobotOperator RobotController;
        public static SimpleRobot RobotInstance;

        public static RobotOperatorStatus OperatorStatus = new RobotOperatorStatus();
        public static RobotStatus RobotStatus = new RobotStatus();

        private static void InitializeRenderers()
        {
            Program.rStatusRenderer = new RobotStatusRenderer(Program.RobotStatus);
            Program.gStatusRenderer = new GridStatusRenderer();
            Program.cStatusRenderer = new ControllerStatusRenderer(Program.OperatorStatus);
            Program.visualizationRenderer = new GridVisualizationRenderer();
            Program.positionRenderer = new PreviousPositionRenderer(Program.RobotStatus);

            Program.OutputRenderer.RobotStatusRenderer = Program.rStatusRenderer;
            Program.OutputRenderer.GridStatusRenderer = Program.gStatusRenderer;
            Program.OutputRenderer.ControllerStatusRenderer = Program.cStatusRenderer;
            Program.OutputRenderer.GridVisualizationRenderer = Program.visualizationRenderer;
            Program.OutputRenderer.PreviousPositionRenderer = Program.positionRenderer;
        }

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

                    case "-multimode":
                        Program.OperationMode = ModeOfOperation.InteractiveMultipleCommand;
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

                Program.OutputRenderer.Draw();
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

            Program.OutputRenderer.Draw();
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
            Position robotPosition = Program.RobotInstance.CurrentPosition;
            BaseTile tile = Program.Grid.GetTileAtLocation(robotPosition.Location);

            Program.RobotStatus.PreviousPositions.Add(new RobotMovementData(robotPosition, tile));
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
