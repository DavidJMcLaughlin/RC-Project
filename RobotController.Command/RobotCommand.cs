using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Command
{
    /// <summary>
    /// Defines a basic command message
    /// </summary>
    /// <exception cref="ArgumentException">Throws if this.IsDataValid(string data) is false.</exception>  
    public class RobotCommand
    {
        public RobotCommand(int id, string data)
        {
            this.Id = id;
            this.Data = data;

            if (!this.IsDataValid())
            {
                throw new ArgumentException();
            }
        }

        public const string EMPTY_DATA = "[EMPTY]";
        public const int EMPTY_ID = int.MinValue;

        /// <summary>
        /// The identifier for a command.
        /// int.MinValue is used to represent an empty identifier.
        /// </summary>
        public int Id { get; private set; } = RobotCommand.EMPTY_ID;
        public string Data { get; private set; } = RobotCommand.EMPTY_DATA;

        /// <summary>
        /// Passes the this.Data property to this.IsDataValid(string).
        /// </summary>
        /// <returns>this.IsDataValid(string).</returns>
        public bool IsDataValid()
        {
            return this.IsDataValid(this.Data);
        }
        /// <summary>
        /// Checks that the given string is a valid data string.
        /// </summary>
        public virtual bool IsDataValid(string data)
        {
            if (data == null)
            {
                return false;
            }

            return true;
        }
    }
}
