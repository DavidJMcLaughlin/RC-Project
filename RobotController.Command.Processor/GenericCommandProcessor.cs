using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Command.Processor
{
    /// <summary>
    /// Validates and processes RobotCommands based on the values in this.CommandProcessorMap.
    /// </summary>
    public class GenericCommandProcessor
    {
        public Dictionary<int, ICommandInterpreter> CommandProcessorMap { get; private set; } = new Dictionary<int, ICommandInterpreter>();

        public virtual void ProcessCommand(RobotCommand command, IControllableRobot robot)
        {
            if (this.IsCommandValid(command))
            {
                bool success = this.TryExecuteCommand(command, robot);
            }
        }

        /// <summary>
        /// Checks if a RobotCommand object meets the command requirements to be considered valid in this context.
        /// </summary>
        public virtual bool IsCommandValid(RobotCommand command)
        {
            if (command == null)
            {
                return false;
            }

            if (this.CommandProcessorMap == null || !this.CommandProcessorMap.ContainsKey(command.Id))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Processes a command if it has an ICommandInterpreter registered to its id.
        /// </summary>
        /// <returns>true if the command is executed.</returns>
        public bool TryExecuteCommand(RobotCommand command, IControllableRobot robot)
        {
            if (this.CommandProcessorMap != null && this.CommandProcessorMap.ContainsKey(command.Id))
            {
                ICommandInterpreter interpreter = this.CommandProcessorMap[command.Id];

                if (interpreter == null)
                {
                    return false;
                }

                interpreter.Interpret(command, robot);

                return true;
            }

            return false;
        }
    }
}
