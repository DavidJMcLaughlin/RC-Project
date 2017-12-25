using RobotController.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Operator
{
    public interface IMessageReceiver
    {
        void QueueCommand(RobotCommand command);
        void RunNextCommand();
    }
}
