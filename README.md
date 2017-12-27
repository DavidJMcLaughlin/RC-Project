# Robot controller
A C# robot controller for navigating a 2D grid with various obstructions.

## Getting started:
This program is built against .NET 4.7.1 and will require Newtonsoft.Json v10.0.3

#### Branch
To build this project you'll need to use the wip-main branch, it contains the code that is working but not quite release ready. At this time there is no 'release' ready code in the master branch.

#### Running the example program
To run the example program build RobotController.CLI. By default it will start in interactive mode which allows you to send commands to a RobotController in realtime and view its movements on a rendered grid. Interactive mode will also show you some information like if the robot was able to act on the last move command, what the last encountered tile was, some information about the controller state, etc.

You can also start the program with some command line flags if you only want to get the coordinates of the robot's movements.

##### Interactive mode controls
Keyboard Control | Description
------------ | -------------
[F] | Moves the robot 1 space forward in what ever direction it is currently facing
[L] | Rotates the robot 90° clockwise then moves it one space forward
[R] | Rotates the robot 90° counterclockwise then moves it one space forward
[1] | Generates a new list of obstruction tiles for the given grid
[Ctrl]+[LeftArrow] | Rotates the robot 90° clockwise without changing its location
[Ctrl]+[RightArrow] | Rotates the robot 90° counterclockwise without changing its location

##### Command line flags
Flag | Syntax | Description
------------ | ------------- | ------------ 
-grid | -grid FILE-PATH | Sets the the path to load/save a grid to
-command | -command COMMAND-STRING | Sets the command to send to the robot
-serialize | -serialize | Switches the program into serialize mode where a random grid is generated and saved to the file-path specified with -grid
-multimode | -multimode | Starts the program in interactive mode but allows you to send more than a single move instruction in a single command
-startpos | -startpos X Y | Allows you to set a fixed starting position for the robot. Without this flag a random empty spot will be picked at launch

##### Command line examples
Example | Description
------------ | -------------
-grid myGrid.json -serialize | Generates a new random grid and serializes it to "{Working Directory}\myGrid.json"
-grid myGrid.json -command lrlflfrllffffflr | Attempts to load a json serialized grid from "{Working Directory}\myGrid.json" and sends a move command to the robot with "lrlflfrllffffflr" for the data
-grid myGrid.json -command lrlflfrllffffflr -startpos 12 16 | Attempts to load a json serialized grid from "{Working Directory}\myGrid.json", sets the robot's starting position to x=12,y=16, then sends a move command to the robot with "lrlflfrllffffflr" for the data
-grid myGrid.json -startpos 12 16 | Attempts to load a json serialized grid from "{Working Directory}\myGrid.json", sets the robot's starting position to x=12,y=16, then launches the normal interactive mode

## Tiles:
Currently the program has a few tile types the robot can encounter.

Tile | Description
------------ | -------------
EmptyTile | This tile will not hinder the SimpleRobot's movement in any way
RockTile | This tile will prevent the SimpleRobot from moving onto the space it occupies 
HoleTile | This tile is connected with another location on the grid, moving onto it will teleport the SimpleRobot
SpinnerTile | This tile has a Rotation property that will adjust a SimpleRobot's current direction when moved onto this tile type

## Project breakdown:

#### RobotController.Grid
This library contains everything for setting up a grid.
* __BaseTile.cs:__ defines what a grid tile object should have.
* __Simple2DGrid.cs:__ a grid for use with IControllableRobot implementations. Holds a list of BaseTile objects for given locations.

#### RobotController.Grid.Generator
This library contains objects for generating a random Simple2DGrid object.
* __IGridGenerator.cs:__ an interface for defining what a grid generator should be able to do.
* __SimpleGridGenerator.cs:__ an implementation of IGridGenerator.

#### RobotController.Command
Defines what properties a RobotCommand should have.
* __RobotCommand.cs:__ the base class for RobotCommandObjects.
* __RobotMoveCommand.cs:__ has command id 1 so any messages with id 1 should be treated as a move command.

#### RobotController.Command.Processor
Defines some objects for processing RobotComand objects.
* __ICommandInterpreter.cs:__ an interface for an object that will handle specific RobotCommand objects passed to it and interacting with an IControllableRobot based on the contents of the command.
* __GenericCommandProcessor.cs:__ an object for storing a map of int32 to ICommandInterpreter objects. This allows you to register a specific implementation of ICommandInterpreter to a known RobotCommand id. So when a command is passed to GenericCommandProcessor it can be processed by what ever implementation of ICommandInterpreter was registered to deal with it.
* __MovementCommandInterpreter.cs:__ implements ICommandInterpreter to handle RobotMoveCommand objects and move an IControllableRobot based on the data of the command.

#### RobotController.Robot
* __IControllableRobot.cs:__ defines a controllable robot interface.
* __SimpleRobot.cs:__ a basic IControllableRobot implementation.

#### RobotController.Operator
* __IMessageReceiver.cs:__ an interface that can store RobotCommand objects and process them at a later time.
* __RobotOperator.cs:__ implements IMessageReceiver. Receives RobotCommand objects and adjust a stored IControllableRobot object based on the commands received.

#### RobotController.CLI
A quick and dirty console application for testing an IControllableRobot on a grid.

## Unit Tests:
Unit tests are setup with MSTestv2. Projects that end with .Tests are the unit tests
