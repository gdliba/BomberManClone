using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using static System.Reflection.Metadata.BlobBuilder;

namespace BomberManClone
{
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
    class Map
    {
        private Cell[,] m_Cells;
        private int m_width;
        private int m_height;

        float m_explosionDuration;
        float m_occupiedCellDuration;
        float m_powerUpCellDuration;

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
            m_explosionDuration = 2f;
            m_occupiedCellDuration = .03f;
            m_powerUpCellDuration = 10f;
        }
        public void Update(GameTime gt)
        {
            for (int x = 0; x < m_width; x++)
                for (int y = 0; y < m_height; y++)
                {
                    if (m_Cells[x, y].Duration != -1)
                    {
                        if(m_Cells[x, y].Duration > 0)
                        {
                            m_Cells[x, y].Duration -=(float)gt.ElapsedGameTime.TotalSeconds;
                        }
                        else
                        {
                            m_Cells[x, y].Duration = -1;
                            m_Cells[x, y].Type = 1;
                        }
                    }
                }
        }
        public void CrateOccupyingCell(Point idx)
        {
            m_Cells[idx.X, idx.Y].Type = 9;
        }
        public void PowerUpOnCell(Point idx)
        {
            m_Cells[idx.X, idx.Y].Type = 10;
            m_Cells[idx.X, idx.Y].Duration = m_powerUpCellDuration;
        }
        public bool IsPowerUpOnCell(Point idx)
        {
            if (m_Cells[idx.X, idx.Y].Type == 10) 
                return true;
            else 
                return false;
        }
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
        public bool IsCellSafe(Point idx)
        {
            switch (m_Cells[idx.X, idx.Y].Type)
            {
                case 1:
                    return true;
                default:
                    return false;
            }
        }
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
        public void SetCellToBomb(Point idx)
        {
            m_Cells[idx.X, idx.Y].Type = 0;
        }
        public bool IsCellExploding(Point idx)
        {
            if (m_Cells[idx.X, idx.Y].Type == 7)
                return true;
            return false;
        }
        public void PlayerIsOccupyingCell(Point idx)
        {
            m_Cells[idx.X, idx.Y].Type = 0;
            m_Cells[idx.X, idx.Y].Duration = m_occupiedCellDuration;
        }
        public void RegularBombExplosion(Point idx, GameTime gt, int explosionRange)
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

                    // Check if out of bounds
                    if (newX < 0 || newX >= m_Cells.GetLength(0) || newY < 0 || newY >= m_Cells.GetLength(1))
                        break;

                    // Check if hitting a Crate
                    if (m_Cells[newX, newY].Type == 9)
                    {
                        m_Cells[newX, newY].Type = 7;
                        m_Cells[newX, newY].Duration = m_explosionDuration + i * .1f;
                        break;
                    }

                    // Stop explosion propagation if it hits a wall or other similar
                    else if (m_Cells[newX, newY].Type == 2 || m_Cells[newX, newY].Type == 3 
                        || m_Cells[newX, newY].Type == 4 || m_Cells[newX, newY].Type == 8)
                        break;

                    m_Cells[newX, newY].Type = 7;
                    m_Cells[newX, newY].Duration = m_explosionDuration+i*.1f;


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
                        var roll = Game1.RNG.Next(0, 10);
                        if (roll < 5)
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
                    sb.Draw(tiles[m_Cells[x, y].Type], new Vector2(x * tiles[0].Width, y * tiles[0].Height),
                        Color.White);
                    sb.DrawString(Game1.debugFont,
                        m_Cells[x, y].Type.ToString(), new Vector2(x * 64+32, y * 64+32), Color.Lavender);
                }
        }
    }
}
