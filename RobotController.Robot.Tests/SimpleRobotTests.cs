using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotController.Robot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotController.Grid;
using RobotController.Grid.Tile;
using RobotController.Command;
using System.Drawing;

namespace RobotController.Robot.Tests
{
    [TestClass()]
    public class SimpleRobotTests
    {
        private SimpleRobot InitSimpleRobotInstance()
        {
            Simple2DGrid grid = new Simple2DGrid(0, 0, 32, 32);
            Position startingPosition = new Position(1, 1, CardinalDirection.North);

            SimpleRobot robot = new SimpleRobot(grid, startingPosition);

            return robot;
        }

        [TestMethod()]
        public void SetPosition_GoodInput_Test()
        {
            bool success = false;

            SimpleRobot robot = this.InitSimpleRobotInstance();
            robot.PositionChanged += delegate (object sender, EventArgs e)
            {
                success = true;
            };

            robot.SetPosition(robot.CurrentPosition);

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void SetPosition_BadInput_Test()
        {
            bool moved = false;

            SimpleRobot robot = this.InitSimpleRobotInstance();
            robot.PositionChanged += delegate (object sender, EventArgs e)
            {
                moved = true;
            };

            robot.SetPosition(new Position(robot.Grid.Bounds.Width + 1, robot.Grid.Bounds.Height + 1, CardinalDirection.North));

            Assert.IsFalse(moved);
        }

        [TestMethod()]
        public void Rotate_Test()
        {
            SimpleRobot robot = this.InitSimpleRobotInstance();
            robot.Rotate(Rotation.CW90);

            bool success = (robot.CurrentPosition.Direction == CardinalDirection.East);

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void GetAdvancedPosition_Test()
        {
            SimpleRobot robot = this.InitSimpleRobotInstance();
            Position aPosition = robot.GetAdvancedPosition();

            bool hasSameDirection = (robot.CurrentPosition.Direction == aPosition.Direction);
            bool xLocationIsOneOff = (robot.CurrentPosition.X == (aPosition.X - 1) || robot.CurrentPosition.X == (aPosition.X + 1));
            bool yLocationIsOneOff = (robot.CurrentPosition.Y == (aPosition.Y - 1) || robot.CurrentPosition.Y == (aPosition.Y + 1));
            bool isLocationOneOff = (xLocationIsOneOff || yLocationIsOneOff);

            bool success = (hasSameDirection && isLocationOneOff);

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void AdjustPositionForEmptyTest()
        {
            SimpleRobot robot = this.InitSimpleRobotInstance();

            EmptyTile tile = new EmptyTile(robot.GetPosition().Location);

            Position newPosition = robot.GetAdvancedPosition();
            Position adjustedPosition = robot.AdjustPositionForEmpty(tile, newPosition);

            bool success = (newPosition.Location == adjustedPosition.Location) && (newPosition.Direction == adjustedPosition.Direction);

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void AdjustPositionForRock_Test()
        {
            SimpleRobot robot = this.InitSimpleRobotInstance();

            RockTile tile = new RockTile(robot.GetPosition().Location);

            Position oldPosition = robot.GetPosition();
            Position newPosition = robot.GetAdvancedPosition();
            Position adjustedPosition = robot.AdjustPositionForRock(tile, newPosition);

            bool success = (adjustedPosition.Location == oldPosition.Location) && (adjustedPosition.Direction == oldPosition.Direction);

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void AdjustPositionForHole_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AdjustPositionForSpinner_Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IsPositionValid_GoodInput_Test()
        {
            SimpleRobot robot = this.InitSimpleRobotInstance();
            Position position = new Position(robot.Grid.Bounds.Width / 2, robot.Grid.Bounds.Height / 2, CardinalDirection.North);

            bool success = robot.IsPositionValid(position);

            Assert.IsTrue(success);
        }

        [TestMethod()]
        public void IsPositionValid_BadInput_Test()
        {
            SimpleRobot robot = this.InitSimpleRobotInstance();
            Position position = new Position(robot.Grid.Bounds.Width + 1, robot.Grid.Bounds.Height + 1, CardinalDirection.North);

            bool success = robot.IsPositionValid(position);

            Assert.IsFalse(success);
        }

        [TestMethod()]
        public void RotateCardinalDirectionFromRotation_Test()
        {
            SimpleRobot robot = this.InitSimpleRobotInstance();
            CardinalDirection newDirection = robot.RotateCardinalDirection(CardinalDirection.North, Rotation.CW90);

            bool success = (newDirection == CardinalDirection.East);

            Assert.IsTrue(success);
        }
    }
}