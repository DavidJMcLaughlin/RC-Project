using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Command
{
    public class RobotMoveCommand : RobotCommand
    {
        public RobotMoveCommand(string data) : base(RobotMoveCommand.COMMAND_ID, data)
        {
        }

        public const int COMMAND_ID = 1;
    }
}
