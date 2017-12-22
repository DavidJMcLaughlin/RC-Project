using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using RobotController.Robot;
using RobotController.Grid;

namespace RobotController.Command.Processor
{
    /// <summary>
    /// An ICommandInterpreter used to process 'movement' RobotCommand objects
    /// </summary>
    public class MovementCommandInterpreter : ICommandInterpreter
    {
        public MovementCommandInterpreter()
        {
        }

        public void Interpret(RobotCommand command, IControllableRobot robot)
        {
            if (this.IsCommandValid(command))
            {
                char[] commands = this.ParseData(command.Data);

                foreach (char c in commands)
                {
                    this.MoveRobot(c, robot);
                }
            }
        }

        /// <summary>
        /// Check if the command is able to be processed
        /// </summary>
        private bool IsCommandValid(RobotCommand command)
        {
            if (command == null)
            {
                return false;
            }

            if (command.Id != RobotMoveCommand.COMMAND_ID)
            {
                return false;
            }

            if (string.IsNullOrEmpty(command.Data))
            {
                return false;
            }

            return true;
        }

        private void MoveRobot(char command, IControllableRobot robot)
        {
            switch (command)
            {
                case 'L':
                    robot.Rotate(Rotation.CCW90);
                    break;

                case 'R':
                    robot.Rotate(Rotation.CW90);
                    break;

                case 'F':
                    robot.Rotate(Rotation.None);
                    break;

                default:
                    Trace.WriteLine("Unknown command");
                    return;
            }

            Position nextPosition = robot.GetAdvancedPosition();
            robot.SetPosition(nextPosition);
        }

        private char[] ParseData(string data)
        {
            return data.ToCharArray();
        }
    }
}
