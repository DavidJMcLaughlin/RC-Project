using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotController.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Command.Tests
{
    [TestClass()]
    public class RobotCommandTests
    {
        [TestMethod()]
        public void IsDataValid_GoodInput_Test()
        {
            int id = 0;
            string data = "test";

            RobotCommand command = new RobotCommand(id, data);

            Assert.IsNotNull(command.Data);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void IsDataValid_BadInput_Test()
        {
            int id = 0;
            string data = null;

            RobotCommand command = new RobotCommand(id, data);
        }
    }
}