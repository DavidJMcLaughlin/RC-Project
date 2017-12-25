using RobotController.Grid;
using RobotController.Grid.Tile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace RobotController.Robot
{
    public class SimpleRobot : IControllableRobot
    {
        public SimpleRobot(Simple2DGrid grid, Position robotStartingPosition)
        {
            this.InitializeTileActionMap();

            this.Grid = grid;
            this.StartingPosition = robotStartingPosition;
            this.SetPosition(this.StartingPosition);
        }

        public Dictionary<int, Func<BaseTile, Position, Position>> TileMap { get; private set; } = new Dictionary<int, Func<BaseTile, Position, Position>>();

        /// <summary>
        /// This event is triggered when the robot's location or rotation is changed.
        /// </summary>
        public event EventHandler PositionChanged;
        public event EventHandler<TileEventArgs> TileEncountered;

        public Simple2DGrid Grid { get; private set; }

        public Position CurrentPosition { get; private set; }
        public Position PreviousPosition { get; private set; }
        public Position StartingPosition { get; private set; }

        /// <summary>
        /// Sets up the this.TileMap with some default values.
        /// </summary>
        public virtual void InitializeTileActionMap()
        {
            this.TileMap = new Dictionary<int, Func<BaseTile, Position, Position>>()
            {
                { EmptyTile.TILE_ID, AdjustPositionForEmpty },
                { RockTile.TILE_ID, AdjustPositionForRock },
                { HoleTile.TILE_ID, AdjustPositionForHole },
                { SpinnerTile.TILE_ID, AdjustPositionForSpinner }
            };
        }

        /// <summary>
        /// Returns the robot's current position.
        /// </summary>
        public virtual Position GetPosition()
        {
            return this.CurrentPosition;
        }

        /// <summary>
        /// Moved the robot to a new position thats 
        /// </summary>
        public virtual void SetPosition(Position position)
        {
            BaseTile tile = this.GetTileAtPosition(position);
            Position adjustedPosition;
            bool success = this.TryToAdjustPositionIntoTile(tile, position, out adjustedPosition);

            if (success && this.IsPositionValid(adjustedPosition))
            {
                this.PreviousPosition = this.CurrentPosition;
                this.CurrentPosition = adjustedPosition;

                this?.PositionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Rotates the robot without changing its location.
        /// </summary>
        public virtual void Rotate(Rotation rotation)
        {
            CardinalDirection newDirection = this.RotateCardinalDirection(this.CurrentPosition.Direction, rotation);
            this.SetPosition(new Position(this.CurrentPosition.Location, newDirection));
        }

        /// <summary>
        /// Gets a position that is moved 1 unit forward from the robot's current position based on which direction the robot is currently facing.
        /// </summary>
        public virtual Position GetAdvancedPosition()
        {
            Point coordinates = this.CurrentPosition.Location;

            switch ((CardinalDirection)Math.Abs((int)this.CurrentPosition.Direction))
            {
                case CardinalDirection.North:
                    coordinates = new Point(coordinates.X, coordinates.Y + 1);
                    break;

                case CardinalDirection.East:
                    coordinates = new Point(coordinates.X + 1, coordinates.Y);
                    break;

                case CardinalDirection.South:
                    coordinates = new Point(coordinates.X, coordinates.Y - 1);
                    break;

                case CardinalDirection.West:
                    coordinates = new Point(coordinates.X - 1, coordinates.Y);
                    break;
            }

            return new Position(coordinates, this.CurrentPosition.Direction);
        }

        /// <summary>
        /// Trys to adjust the robot's position based on known obstructions.
        /// </summary>
        public virtual bool TryToAdjustPositionIntoTile(BaseTile tile, Position currentPosition, out Position newPosition)
        {
            if (this.TileMap != null && this.TileMap.ContainsKey(tile.Id))
            {
                newPosition = this.TileMap[tile.Id].Invoke(tile, currentPosition);

                this?.TileEncountered?.Invoke(this, new TileEventArgs(tile));

                return true;
            }

            newPosition = currentPosition;
            return false;
        }

        public virtual Position AdjustPositionForEmpty(BaseTile tile, Position currentPosition)
        {
            return currentPosition;
        }

        public virtual Position AdjustPositionForRock(BaseTile tile, Position currentPosition)
        {
            return this.CurrentPosition;
        }

        public virtual Position AdjustPositionForHole(BaseTile tile, Position currentPosition)
        {
            HoleTile hTile = (HoleTile)tile;
            return new Position(hTile.ConnectedLocation, currentPosition.Direction);
        }

        public virtual Position AdjustPositionForSpinner(BaseTile tile, Position currentPosition)
        {
            SpinnerTile sTile = (SpinnerTile)tile;
            CardinalDirection newDirection = this.RotateCardinalDirection(currentPosition.Direction, sTile.SpinAmount);
            return new Position(currentPosition.Location, newDirection);
        }

        /// <summary>
        /// Checks if a given position is valid for this robot.
        /// </summary>
        public virtual bool IsPositionValid(Position position)
        {
            return this.Grid.Contains(position);
        }

        /// <summary>
        /// Apply a rotation value to a CardinalDirection.
        /// </summary>
        public CardinalDirection RotateCardinalDirection(CardinalDirection currentDirection, Rotation rotateDirection)
        {
            int current = (int)currentDirection;
            int next = (int)rotateDirection;
            int result = (next + current);

            if (result >= 360)
            {
                result = (result % 360);
            }
            else if (result < 0)
            {
                result = (360 + result);
            }

            return (CardinalDirection)result;
        }

        private BaseTile GetTileAtPosition(Position position)
        {
            return this.Grid.GetTileAtLocation(position.Location);
        }
    }
}
