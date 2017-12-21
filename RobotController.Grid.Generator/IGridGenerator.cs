using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Grid.Generator
{
    public interface IGridGenerator
    {
        Simple2DGrid Generate();
    }
}
