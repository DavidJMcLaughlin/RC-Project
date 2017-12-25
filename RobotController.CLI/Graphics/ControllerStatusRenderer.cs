using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.CLI.Graphics
{
    public class ControllerStatusRenderer : BaseConsoleRenderer
    {
        public ControllerStatusRenderer(RobotOperatorStatus status)
        {
            this.Status = status;
        }

        public RobotOperatorStatus Status { get; private set; }

        public override void Draw()
        {
            int startingLeftIndex = Console.CursorLeft;

            string spacer = "  ";

            Console.Write("Controller: ");
            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);
            Console.Write(spacer + "Last command recieved:");
            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);
            Console.Write(spacer + spacer + "Id: {0}", this.Status.LastCommandReceived.Command.Id);
            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);
            Console.Write(spacer + spacer + "Data: {0}", this.Status.LastCommandReceived.Command.Data);
            Console.SetCursorPosition(startingLeftIndex, Console.CursorTop + 1);
            Console.Write(spacer + spacer + "Timestamp: {0}", this.Status.LastCommandReceived.Timestamp.ToLongTimeString());
        }
    }
}
