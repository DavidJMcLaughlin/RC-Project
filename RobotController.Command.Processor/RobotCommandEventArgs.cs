using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Command.Processor
{
    public class RobotCommandEventArgs : EventArgs
    {
        public RobotCommandEventArgs(RobotCommand command)
        {
            this.Command = command;
            this.Timestamp = DateTime.Now;
        }

        public RobotCommand Command { get; private set; }
        /// <summary>
        /// The DateTime of when the RobotCommandEventArgs object was created.
        /// </summary>
        public DateTime Timestamp { get; private set; }
    }
}
