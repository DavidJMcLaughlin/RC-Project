using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotController.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RobotController.Grid.Tile;

namespace RobotController.Grid.Tests
{
    [TestClass()]
    public class Simple2DGridTests
    {
        private Simple2DGrid GridToTest = new Simple2DGrid(0, 0, 32, 32);

        [TestMethod()]
        public void Contains_GoodInput_Test()
        {
            Position testPosition = new Position(1, 1, CardinalDirection.North);

            Assert.IsTrue(this.GridToTest.Contains(testPosition));
        }
        [TestMethod()]
        public void Contains_BadInput_Test()
        {
            Position testPosition = new Position((this.GridToTest.Bounds.Width + 1), (this.GridToTest.Bounds.Height + 1), CardinalDirection.North);

            Assert.IsFalse(this.GridToTest.Contains(testPosition));
        }

        [TestMethod()]
        public void GetTileAtLocation_GoodInput_Test()
        {
            Point obstructionLocation = new Point((this.GridToTest.Bounds.Width / 2), (this.GridToTest.Bounds.Height / 2));
            RockTile target = new RockTile(obstructionLocation);

            this.GridToTest.Obstructions.Clear();
            this.GridToTest.Obstructions.Add(target);

            BaseTile foundTile = this.GridToTest.GetTileAtLocation(obstructionLocation);

            bool areEqual = (target.Equals(foundTile));

            Assert.IsTrue(areEqual);
        }

        [TestMethod()]
        public void GetTileAtLocation_BadInput_Test()
        {
            Point obstructionLocation = new Point((this.GridToTest.Bounds.Width / 2), (this.GridToTest.Bounds.Height / 2));
            Point testLocation = new Point(obstructionLocation.X + 1, obstructionLocation.Y + 1);
            RockTile target = new RockTile(obstructionLocation);

            this.GridToTest.Obstructions.Clear();
            this.GridToTest.Obstructions.Add(target);

            BaseTile foundTile = this.GridToTest.GetTileAtLocation(testLocation);

            bool areEqual = (target.Equals(foundTile));

            Assert.IsFalse(areEqual);
        }

        [TestMethod()]
        public void GetAllEmptyPoints_Test()
        {
            List<Point> points = Simple2DGrid.GetAllEmptyPoints(this.GridToTest);

            Assert.IsNotNull(points);

            foreach (Point location in points)
            {
                if (!this.GridToTest.Bounds.Contains(location))
                {
                    Assert.Fail("Point is outside of grid bounds.");
                }

                if (this.GridToTest.GetTileAtLocation(location).Id != EmptyTile.TILE_ID)
                {
                    Assert.Fail("Tile at point is not empty.");
                }
            }
        }
    }
}