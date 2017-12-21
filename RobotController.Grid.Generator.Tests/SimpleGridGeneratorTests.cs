using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotController.Grid.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Grid.Generator.Tests
{
    [TestClass()]
    public class SimpleGridGeneratorTests
    {
        private SimpleGridGenerator GeneratorToTest = new SimpleGridGenerator(32, 32);

        [TestMethod()]
        public void Generate_Test()
        {
            Simple2DGrid grid = this.GeneratorToTest.Generate();

            bool hasProperStartingLocation = (this.GeneratorToTest.StartingXValue == grid.Bounds.X && this.GeneratorToTest.StartingYValue == grid.Bounds.Y);
            bool isProperSize = (this.GeneratorToTest.GridSize == grid.Bounds.Size);
            bool allTrue = (hasProperStartingLocation && isProperSize);

            Assert.IsTrue(allTrue);
        }

        [TestMethod()]
        public void GenerateObstructions_Test()
        {
            int totalObstructions = 4;
            this.GeneratorToTest.MaxObstructions = totalObstructions;

            Simple2DGrid grid = this.GeneratorToTest.Generate();

            bool hasProperAmountOfObstructions = (totalObstructions == grid.Obstructions.Count);

            Assert.IsTrue(hasProperAmountOfObstructions);
        }

        [TestMethod()]
        public void GetDefaultMaxObstructions_Test()
        {
            int totalObstructions = this.GeneratorToTest.GetDefaultMaxObstructions();
            int gridTileCount = (this.GeneratorToTest.GridSize.Width * this.GeneratorToTest.GridSize.Height);

            bool notTooSmall = (totalObstructions > 0);
            bool notTooBig = (totalObstructions < gridTileCount);
            bool allTrue = (notTooBig && notTooSmall);

            Assert.IsTrue(allTrue);
        }
    }
}