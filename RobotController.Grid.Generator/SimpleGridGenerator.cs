using RobotController.Grid.Tile;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController.Grid.Generator
{
    /// <summary>
    /// Generates Simple2DGrid objects
    /// </summary>
    public class SimpleGridGenerator : IGridGenerator
    {
        public SimpleGridGenerator(int width, int height) : this(new Size(width, height))
        {
        }
        public SimpleGridGenerator(Size gridSize)
        {
            this.GridSize = gridSize;
            this.AvailableObstrctions = this.GetDefaultAvailableObstructions();
        }

        private Random randomGen = new Random();

        /// <summary>
        /// Stores an instance of a BaseTile type that can be added to a grid's Obstructions list.
        /// This HashSet will be populated with this.GetDefaultAvailableObstructions() when SimpleGridGenerator is initialized.
        /// </summary>
        public HashSet<BaseTile> AvailableObstrctions { get; private set; } = new HashSet<BaseTile>();

        public Size GridSize { get; set; }
        public int StartingXValue { get; set; } = 0;
        public int StartingYValue { get; set; } = 0;

        /// <summary>
        /// If this values is negative it will default to this.GetDefaultMaxObstructions().
        /// </summary>
        public int MaxObstructions { get; set; } = -1;

        /// <summary>
        /// Creates a new Basic2DGrid based on {BasicGridGenerator} properties.
        /// </summary>
        public virtual Simple2DGrid Generate()
        {
            Simple2DGrid grid = this.GetGrid();

            List<BaseTile> obstructions = this.GenerateObstructions(grid);
            grid.Obstructions.Clear();
            grid.Obstructions.AddRange(obstructions);

            return grid;
        }

        /// <summary>
        /// Creates a list of tile objects at random empty points on an existing grid from this.AvailableObstrctions.
        /// </summary>
        public virtual List<BaseTile> GenerateObstructions(Simple2DGrid grid)
        {
            if (this.MaxObstructions < 0)
            {
                this.MaxObstructions = this.GetDefaultMaxObstructions();
            }

            List<BaseTile> obstructions = new List<BaseTile>();
            List<Point> freeLocations = Simple2DGrid.GetAllEmptyPoints(grid);

            for (int i = 0; i < this.MaxObstructions; i++)
            {
                int randomIndex = this.randomGen.Next(0, freeLocations.Count);

                Point location = freeLocations[randomIndex];
                BaseTile tile = this.GetRandomTile(location);

                // Create a new instance of the type of tile selected otherwise all BaseTiles of the same type, in the obstructions list will have the same properties
                BaseTile newTile = (BaseTile)Activator.CreateInstance(tile.GetType(), location);

                freeLocations.RemoveAt(randomIndex);

                if (newTile.GetType() == typeof(HoleTile)) // Initialize after freeLocations.RemoveAt so we don't pick the same location twice
                {
                    randomIndex = this.randomGen.Next(0, freeLocations.Count);

                    HoleTile hTile = (HoleTile)newTile;
                    Point connectionLocation = freeLocations[randomIndex];
                    hTile.ConnectedLocation = connectionLocation;

                    freeLocations.RemoveAt(randomIndex);
                }

                if (newTile.GetType() == typeof(SpinnerTile))
                {
                    SpinnerTile sTile = (SpinnerTile)newTile;

                    int randomeRoationRaw = (this.randomGen.Next(1, 3) * 90);
                    randomeRoationRaw = (this.randomGen.Next(0, 1) == 1 ? randomeRoationRaw * -1 : randomeRoationRaw); // Randomly turn the roation negative

                    Rotation randomRotation = (Rotation)randomeRoationRaw;

                    sTile.SpinAmount = randomRotation;
                }

                obstructions.Add(newTile);
            }

            return obstructions;
        }

        /// <summary>
        /// Returns a default HashSet of obstruction tiles.
        /// </summary>
        public virtual HashSet<BaseTile> GetDefaultAvailableObstructions()
        {
            HashSet<BaseTile> availableObstructions = new HashSet<BaseTile>()
            {
                new HoleTile(0, 0),
                new RockTile(0, 0),
                new SpinnerTile(0, 0)
            };

            return availableObstructions;
        }

        /// <summary>
        /// Returns a recommended number of max obstruction tiles based on this.GridSize.
        /// </summary>
        public virtual int GetDefaultMaxObstructions()
        {
            int division = 20;

            int totalTiles = this.GridSize.Width * this.GridSize.Height;

            int maxObstructions = 1;

            if (totalTiles > division)
            {
                maxObstructions = (totalTiles / division);
            }

            return maxObstructions;
        }

        /// <summary>
        /// Picks a random tile from {BasicGridGenerator}.AvailableObstrctions.
        /// </summary>
        private BaseTile GetRandomTile(Point location)
        {
            int index = this.randomGen.Next(0, this.AvailableObstrctions.Count);

            BaseTile tile = this.AvailableObstrctions.ElementAt(index);
            tile.StartLocation = location;

            return tile;
        }

        /// <summary>
        /// Creates a grid based on the properties of this class.
        /// </summary>
        private Simple2DGrid GetGrid()
        {
            Simple2DGrid grid = new Simple2DGrid(this.StartingXValue, this.StartingYValue, this.GridSize.Width, this.GridSize.Height);
            return grid;
        }
    }
}
