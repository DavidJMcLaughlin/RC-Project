using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotController.Command;
using RobotController.Command.Processor;
using RobotController.Grid;
using RobotController.Operator;
using RobotController.Robot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Operator.Tests
{
    [TestClass()]
    public class RobotOperatorTests
    {
        private RobotOperator InitRobotOperatorInstance()
        {
            Simple2DGrid grid = new Simple2DGrid(0, 0, 32, 32);
            SimpleRobot simpleRobot = new SimpleRobot(grid, new Position(0, 0, CardinalDirection.North));

            RobotOperator op = new RobotOperator(simpleRobot);

            return op;
        }

        [TestMethod()]
        public void QueueCommand_Test()
        {
            bool success = false;

            RobotOperator op = this.InitRobotOperatorInstance();
            op.AcknowledgeCommandReceived += delegate (object sender, RobotCommandEventArgs e)
            {
                success = true;
            };

            RobotMoveCommand command = new RobotMoveCommand("FFLR");

            op.QueueCommand(command);

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void RunNextCommand_Test()
        {
            bool success = false;

            RobotOperator op = this.InitRobotOperatorInstance();
            op.CommandProcessedSuccessfully += delegate (object sender, RobotCommandEventArgs e)
            {
                success = true;
            };

            MovementCommandInterpreter mci = new MovementCommandInterpreter();

            RobotMoveCommand command = new RobotMoveCommand("FFLR");

            op.RegisterCommandInterpreter(command.Id, mci);

            op.QueueCommand(command);

            op.RunNextCommand();

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void RegisterCommandInterpreter_Test()
        {
            RobotOperator op = this.InitRobotOperatorInstance();

            MovementCommandInterpreter mci = new MovementCommandInterpreter();

            RobotMoveCommand command = new RobotMoveCommand("FFLR");

            op.RegisterCommandInterpreter(command.Id, mci);

            bool success = (op.RegisteredCommands.ContainsKey(command.Id) && op.RegisteredCommands.ContainsValue(mci));

            Assert.IsTrue(success);
        }
    }
}