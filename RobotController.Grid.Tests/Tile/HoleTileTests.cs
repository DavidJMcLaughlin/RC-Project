using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotController.Grid.Tile;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Grid.Tile.Tests
{
    [TestClass()]
    public class HoleTileTests
    {
        [TestMethod()]
        public void IsAtLocation_GoodInput_Test()
        {
            Point startLocation = new Point(1, 1);
            Point endLocation = new Point(2, 2);

            HoleTile tile = new HoleTile(startLocation, endLocation);

            bool isAtBothLocations = (tile.IsAtLocation(startLocation) && tile.IsAtLocation(endLocation));

            Assert.IsTrue(isAtBothLocations);
        }

        [TestMethod()]
        public void IsAtLocation_BadInput_Test()
        {
            Point startLocation = new Point(1, 1);
            Point endLocation = new Point(2, 2);

            Point testLocation = new Point(3, 3);
            Point testLocation2 = new Point(4, 4);

            HoleTile tile = new HoleTile(startLocation, endLocation);

            bool isAtEitherLocation = (tile.IsAtLocation(testLocation) || tile.IsAtLocation(testLocation2));

            Assert.IsFalse(isAtEitherLocation);
        }
    }
}