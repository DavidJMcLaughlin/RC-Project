using RobotController.Grid.Tile;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Grid
{
    /// <summary>
    /// A 2D Grid
    /// </summary>
    public class Simple2DGrid
    {
        public Simple2DGrid() : this(new Rectangle(0, 0, 16, 16))
        {
        }
        public Simple2DGrid(int startingX, int startingY, int width, int height) : this(new Rectangle(startingX, startingY, width, height))
        {
        }
        public Simple2DGrid(Rectangle bounds)
        {
            this.Bounds = bounds;
        }

        public Rectangle Bounds { get; private set; } = Rectangle.Empty;
        public List<BaseTile> Obstructions { get; private set; } = new List<BaseTile>();

        /// <summary>
        /// Check if a point is within the bounds of this grid
        /// </summary>
        public bool Contains(Position position)
        {
            return this.Bounds.Contains(position.Location);
        }

        /// <summary>
        /// Gets any obstruction at a given location, if no obstruction is found an empty tile is returned
        /// </summary>
        public virtual BaseTile GetTileAtLocation(Point location)
        {
            foreach (BaseTile tile in this.Obstructions)
            {
                if (tile != null && tile.IsAtLocation(location))
                {
                    return tile;
                }
            }

            return new EmptyTile(location);
        }

        /// <summary>
        /// Randomly finds an empty location on an existing grid
        /// </summary>
        public static List<Point> GetAllEmptyPoints(Simple2DGrid grid)
        {
            List<Point> freeLocations = new List<Point>();

            for (int x = 0; x < grid.Bounds.Width; x++)
            {
                for (int y = 0; y < grid.Bounds.Height; y++)
                {
                    Point location = new Point(x, y);
                    BaseTile tile = grid.GetTileAtLocation(location);

                    if (tile.Id == EmptyTile.TILE_ID)
                    {
                        freeLocations.Add(location);
                    }
                }
            }

            return freeLocations;
        }
    }
}
