using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.CLI.Graphics
{
    public class ProgramOutputRenderer : IConsoleRenderer
    {
        public RobotStatusRenderer RobotStatusRenderer { get; set; }
        public GridStatusRenderer GridStatusRenderer { get; set; }
        public ControllerStatusRenderer ControllerStatusRenderer { get; set; }
        public GridVisualizationRenderer GridVisualizationRenderer { get; set; }
        public PreviousPositionRenderer PreviousPositionRenderer { get; set; }

        public int ScreenDivision { get; set; } = (Console.WindowWidth / 3);

        public void Draw()
        {
            Console.Clear();

            Console.SetCursorPosition(0, 0);
            this.TryToRender(this.RobotStatusRenderer);

            Console.SetCursorPosition(this.ScreenDivision, 0);
            this.TryToRender(this.ControllerStatusRenderer);

            Console.SetCursorPosition((this.ScreenDivision * 2), 0);
            this.TryToRender(this.GridStatusRenderer);

            Console.SetCursorPosition(0, (Console.WindowHeight / 3));
            this.TryToRender(this.PreviousPositionRenderer);

            Console.SetCursorPosition((Console.WindowWidth / 2) - 2, (Console.WindowHeight / 2));
            this.TryToRender(this.GridVisualizationRenderer);

            Console.SetCursorPosition(this.ScreenDivision * 2, (Console.WindowHeight / 2));
        }

        private bool TryToRender(BaseConsoleRenderer renderer)
        {
            if (renderer != null && renderer.Enabled)
            {
                renderer.Draw();

                return true;
            }

            return false;
        }
    }
}
