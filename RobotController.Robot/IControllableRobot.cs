using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotController.Grid;

namespace RobotController.Robot
{
    public interface IControllableRobot
    {
        Position GetPosition();
        Position GetAdvancedPosition();

        void SetPosition(Position position);
        void Rotate(Rotation rotiation);
    }
}
