using RobotController.Command;
using RobotController.Command.Processor;
using RobotController.Robot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Operator
{
    /// <summary>
    /// This object is responsible for receiving RobotCommand objects and directing an IControllableRobot based on their contents
    /// </summary>
    public class RobotOperator : IMessageReceiver
    { 
        public RobotOperator(IControllableRobot robot)
        {
            this.Robot = robot;
        }

        private Queue<RobotCommand> CommandQueue = new Queue<RobotCommand>();
        private GenericCommandProcessor CommandProcessor = new GenericCommandProcessor();

        public IControllableRobot Robot { get; private set; }

        public Dictionary<int, ICommandInterpreter> RegisteredCommands { get { return this.CommandProcessor.CommandProcessorMap; } }

        public event EventHandler<RobotCommandEventArgs> AcknowledgeCommandReceived;
        public event EventHandler<RobotCommandEventArgs> CommandProcessedSuccessfully;
        public event EventHandler CommandProcessedUnsuccessfully;

        /// <summary>
        /// Stores a command and waits for RunNextCommand() to process it.
        /// This method triggers the AcknowledgeCommandReceived event.
        /// </summary>
        public void QueueCommand(RobotCommand command)
        {
            this?.AcknowledgeCommandReceived?.Invoke(this, new RobotCommandEventArgs(command));
            this.CommandQueue.Enqueue(command);
        }

        /// <summary>
        /// Runs the next available command in the Queue
        /// </summary>
        public void RunNextCommand()
        {
            if (this.CommandQueue != null && this.CommandQueue.Count > 0)
            {
                RobotCommand command = this.CommandQueue.Dequeue();
                this.CommandProcessor.ProcessCommand(command, this.Robot);

                this?.CommandProcessedSuccessfully?.Invoke(this, new RobotCommandEventArgs(command));
            }
            else
            {
                this?.CommandProcessedUnsuccessfully?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Loops through all commands in the Queue via RunNextCommand
        /// </summary>
        public void RunAllCommands()
        {
            while (this.CommandQueue.Count > 0)
            {
                this.RunNextCommand();
            }
        }

        /// <summary>
        /// Registeres a ICommandInterpreter to be called to process any command matching a specific command id
        /// </summary>
        public void RegisterCommandInterpreter(int id, ICommandInterpreter interpreter)
        {
            this.CommandProcessor.CommandProcessorMap.Add(id, interpreter);
        }
    }
}
