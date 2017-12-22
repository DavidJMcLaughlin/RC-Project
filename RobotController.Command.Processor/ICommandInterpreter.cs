using RobotController.Robot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Command.Processor
{
    public interface ICommandInterpreter
    {
        void Interpret(RobotCommand command, IControllableRobot robot);
    }
}
