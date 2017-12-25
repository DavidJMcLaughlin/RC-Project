using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.CLI.Graphics
{
    public abstract class BaseConsoleRenderer : IConsoleRenderer
    {
        public bool Enabled { get; set; } = true;

        public abstract void Draw();
    }
}
