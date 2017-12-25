using RobotController.Command.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.CLI
{
    public class RobotOperatorStatus
    {
        public int RegisteredCommandsCount { get; set; }
        public RobotCommandEventArgs LastCommandReceived { get; set; } = new RobotCommandEventArgs(new Command.RobotCommand(-1, string.Empty));
        public RobotCommandEventArgs LastSuccessfullyProcessedCommand { get; set; } = new RobotCommandEventArgs(new Command.RobotCommand(-1, string.Empty));
    }
}
