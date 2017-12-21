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
    public class BaseTileTests
    {
        [TestMethod()]
        public void IsAtLocation_GoodInput_Test()
        {
            Point testLocation = new Point(1, 1);
            BaseTile tile = new EmptyTile(testLocation);

            Assert.IsTrue(tile.IsAtLocation(testLocation));
        }

        [TestMethod()]
        public void IsAtLocation_BadInput_Test()
        {
            Point tileLocation = new Point(1, 1);
            Point testLocation = new Point(tileLocation.X + 1, tileLocation.Y + 1);

            BaseTile tile = new EmptyTile(tileLocation);

            Assert.IsFalse(tile.IsAtLocation(testLocation));
        }
    }
}