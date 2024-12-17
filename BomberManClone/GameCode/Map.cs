using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BomberManClone
{
    /// <summary>
    /// "Cell" is used in the "Map" class so that each individual tile can
    /// learn to have a "type" and "duration".
    /// This helps in more complex logic, such as propagating explosions
    /// and setting a tile to a specific type for a certain duration,
    /// before changing to another type.
    /// </summary>
    class Cell
    {
        public int Type { get; set; }
        public float Duration { get; set; }

        public Cell(int type, float duration = -1)
        {
            Type = type;
            Duration = duration;
        }
    }
    /// <summary>
    /// This class takes in the 2D array given in Game1 describing the
    /// level layout("floorplan"). Esensially all interactions in this
    /// game use a method related to the Map class.
    /// </summary>
    class Map
    {
        private Cell[,] m_Cells;
        private int m_width;
        private int m_height;

        float m_explosionDuration;
        float m_occupiedCellDuration;
        float m_powerUpCellDuration;
        /// <summary>
        /// This method takes in the "floorplan" from Game1.
        /// It derives their position from the 2D array provided.
        /// </summary>
        /// <param name="floorplan"></param>
        public Map(int[,] floorplan)
        {
            m_width = floorplan.GetLength(0);
            m_height = floorplan.GetLength(1);

            m_Cells = new Cell[m_width, m_height];
            for (int x = 0; x < m_width; x++)
                for (int y = 0; y < m_height; y++)
                {
                    m_Cells[x, y] = new Cell(floorplan[y, x]);
                }
            // setting familiar values
            m_explosionDuration = 2f;
            m_occupiedCellDuration = .03f;
            m_powerUpCellDuration = 10f;
        }
        /// <summary>
        /// Update method for the map class.
        /// this logic looks through all cells.
        /// </summary>
        /// <param name="gt"></param>
        public void Update(GameTime gt)
        {
            // loop through all cells
            for (int x = 0; x < m_width; x++)
                for (int y = 0; y < m_height; y++)
                {
                    if (m_Cells[x, y].Duration != -1)
                    {
                        // if their duration is greater than 0
                        if(m_Cells[x, y].Duration > 0)
                        {
                            // count down
                            m_Cells[x, y].Duration -=(float)gt.ElapsedGameTime.TotalSeconds;
                            // eventually you will hit 0
                        }
                        else
                        {
                            // else change the duration to -1 and type to 1 (cell.type = 1 corresponds to the floor tyle,
                            // which is walkable for the players)
                            m_Cells[x, y].Duration = -1;
                            m_Cells[x, y].Type = 1;
                        }
                    }
                }
        }
        /// <summary>
        /// Declare that THIS cell has a crate on it
        /// </summary>
        /// <param name="idx"></param>
        public void CrateOccupyingCell(Point idx)
        {
            m_Cells[idx.X, idx.Y].Type = 9;
        }
        /// <summary>
        /// Declare that THIS cell has a powerup on it
        /// </summary>
        /// <param name="idx"></param>
        public void PowerUpOnCell(Point idx)
        {
            m_Cells[idx.X, idx.Y].Type = 10;
            m_Cells[idx.X, idx.Y].Duration = m_powerUpCellDuration;
        }
        /// <summary>
        /// Check if there is a powerup on THIS cell
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool IsPowerUpOnCell(Point idx)
        {
            if (m_Cells[idx.X, idx.Y].Type == 10) 
                return true;
            else 
                return false;
        }
        /// <summary>
        /// Check if the cell "idx" is walkable for the player
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool IsWalkableForPlayer(Point idx)
        {
            switch (m_Cells[idx.X, idx.Y].Type)
            {
                case 1:
                case 7:
                case 10:
                case 12:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Check if the cell "idx" is a safe cell for the player to respawn on
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool IsCellSafe(Point idx)
        {
            switch (m_Cells[idx.X, idx.Y].Type)
            {
                case 1:
                case 10:
                case 12:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Check if the player in Ghost State can walk on THIS cell
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool IsWalkableForGhost(Point idx)
        {
            switch (m_Cells[idx.X, idx.Y].Type)
            {
                case 3:
                case 4:
                case 8:
                    return false;
                default:
                    return true;
            }
        }
        /// <summary>
        /// Declare that THIS cell contains a bomb
        /// </summary>
        /// <param name="idx"></param>
        public void SetCellToBomb(Point idx)
        {
            m_Cells[idx.X, idx.Y].Type = 0;
        }
        /// <summary>
        /// Check if there is a bomb on THIS cell
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool IsCellBomb(Point idx)
        {
            if(m_Cells[idx.X, idx.Y].Type == 0)
                return true;
             
            return false;
        }
        /// <summary>
        /// Check if THIS cell is exploading.
        /// Useful to teach the player to take a hit if they are on an exploading cell.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool IsCellExploding(Point idx)
        {
            if (m_Cells[idx.X, idx.Y].Type == 7)
                return true;
            return false;
        }
        /// <summary>
        /// Check if the crate on THIS cell has just been hit by an explosion.
        /// If it has, it should be "breaking".
        /// When this happens return "true".
        /// This will teach the crate that is is breaking.
        /// Thus it should try to spawn a powerup.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool IsCrateBreaking(Point idx)
        {
            if (m_Cells[idx.X, idx.Y].Type == 13 || m_Cells[idx.X, idx.Y].Type == 7)
                return true;
            return false;
        }
        /// <summary>
        /// Declare that the player is on THIS cell.
        /// Useful because the cell type is changed, meaning other players
        /// cannot walk on THIS cell.
        /// </summary>
        /// <param name="idx"></param>
        public void PlayerIsOccupyingCell(Point idx)
        {
            m_Cells[idx.X, idx.Y].Type = 14;
            m_Cells[idx.X, idx.Y].Duration = m_occupiedCellDuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="gt"></param>
        /// <param name="explosionRange"></param>
        public void BombExplosion(Point idx, GameTime gt, int explosionRange)
        {
            PropagateExplosion(new Point(1, 0));
            PropagateExplosion(new Point(-1, 0));
            PropagateExplosion(new Point(0, 1));
            PropagateExplosion(new Point(0, -1));

            m_Cells[idx.X, idx.Y].Type = 7;
            m_Cells[idx.X, idx.Y].Duration = m_explosionDuration;


            void PropagateExplosion(Point dir)
            {
                for (int i = 1; i < explosionRange+1; i++)
                {
                    int newX = idx.X + dir.X * i;
                    int newY = idx.Y + dir.Y * i;

                    // Check if hitting a Crate or powerup
                    if (m_Cells[newX, newY].Type == 9)
                    {
                        m_Cells[newX, newY].Type = 13;
                        m_Cells[newX, newY].Duration = m_explosionDuration + i * .1f;
                        break;
                    }

                    // Stop explosion propagation if it hits a wall or other similar
                    else if (m_Cells[newX, newY].Type == 2 || m_Cells[newX, newY].Type == 3 
                        || m_Cells[newX, newY].Type == 4 || m_Cells[newX, newY].Type == 8)
                        break;

                    m_Cells[newX, newY].Type = 7;
                    m_Cells[newX, newY].Duration = m_explosionDuration + i * .1f;


                }
            }

        }
        public void SpawnCrates(Texture2D txr, List<Crate> crates)
        {
            for (int y = 0; y < m_Cells.GetLength(1); y++)
            {
                for(int x = 0; x < m_Cells.GetLength(0); x++)
                {
                    if (m_Cells[x, y].Type == 1)
                    {
                        if (Game1.RNG.NextSingle() < Globals.CrateSpawnChance)
                        {
                            Crate newcrate = new Crate(txr, new Point(x, y));
                            crates.Add(newcrate);
                        }

                    }
                }
            }
        }
        public void DrawMe(SpriteBatch sb, List<Texture2D> tiles)
        {
            for (int x = 0; x < m_width; x++)
                for (int y = 0; y < m_height; y++)
                {
                    if (m_Cells[x, y].Type != 7 && m_Cells[x, y].Type != 13)
                    {
                        sb.Draw(tiles[m_Cells[x, y].Type], new Vector2(x * tiles[0].Width, y * tiles[0].Height),
                            Color.White);
                    }
                    else
                    {
                        sb.Draw(tiles[m_Cells[x, y].Type], new Vector2(x * tiles[0].Width, y * tiles[0].Height),
                                Color.DeepSkyBlue);
                    }
                    //sb.DrawString(Game1.debugFont,
                    //    m_Cells[x, y].Type.ToString(), new Vector2(x * 64+32, y * 64+32), Color.Lavender);
                }
        }
    }
}
