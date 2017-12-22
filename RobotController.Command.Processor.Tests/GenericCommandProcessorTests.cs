using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotController.Command.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotController.Robot;
using RobotController.Grid;
using System.Drawing;

namespace RobotController.Command.Processor.Tests
{
    [TestClass()]
    public class GenericCommandProcessorTests
    {
        private GenericCommandProcessor CommandProcessorToTest = new GenericCommandProcessor();

        [TestMethod()]
        public void ProcessCommand_Test()
        {
            Simple2DGrid grid = new Simple2DGrid(0, 0, 32, 32);
            Position starting = new Position(grid.Bounds.Width / 2, grid.Bounds.Height / 2, CardinalDirection.North);

            RobotMoveCommand command = new RobotMoveCommand("F");
            IControllableRobot robot = new SimpleRobot(grid, starting);

            ICommandInterpreter ici = new MovementCommandInterpreter();

            this.CommandProcessorToTest.CommandProcessorMap.Add(command.Id, ici);

            bool success = false;

            this.CommandProcessorToTest.CommandProcessedSuccessfully += delegate (object sender, RobotCommandEventArgs e)
            {
                success = true;
            };

            this.CommandProcessorToTest.CommandProcessedUnsuccessfully += delegate (object sender, RobotCommandEventArgs e)
            {
                Assert.Fail();
            };

            this.CommandProcessorToTest.ProcessCommand(command, robot);

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void IsCommandValid_GoodInput_Test()
        {
            RobotMoveCommand command = new RobotMoveCommand("F");

            ICommandInterpreter ici = new MovementCommandInterpreter();

            this.CommandProcessorToTest.CommandProcessorMap.Add(command.Id, ici);

            bool success = this.CommandProcessorToTest.IsCommandValid(command);

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void IsCommandValid_BadInput_Test()
        {
            this.CommandProcessorToTest.CommandProcessorMap.Clear();

            RobotMoveCommand command = new RobotMoveCommand("F");

            bool success = this.CommandProcessorToTest.IsCommandValid(command);

            Assert.IsFalse(success);
        }

        [TestMethod()]
        public void TryExecuteCommand_Test()
        {
            Simple2DGrid grid = new Simple2DGrid(0, 0, 32, 32);
            Position starting = new Position(grid.Bounds.Width / 2, grid.Bounds.Height / 2, CardinalDirection.North);

            RobotMoveCommand command = new RobotMoveCommand("F");
            ICommandInterpreter ici = new MovementCommandInterpreter();
            IControllableRobot robot = new SimpleRobot(grid, starting);

            this.CommandProcessorToTest.CommandProcessorMap.Add(command.Id, ici);

            bool success = this.CommandProcessorToTest.TryExecuteCommand(command, robot);

            Assert.IsTrue(success);
        }
    }
}