using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BomberManClone
{
    class Map
    {
        private int[,] m_Cells;
        private int m_width;
        private int m_height;

        float m_explosionDuration;

        public Map(int[,] floorplan)
        {
            m_width = floorplan.GetLength(0);
            m_height = floorplan.GetLength(1);

            m_Cells = new int[m_width, m_height];
            for (int x = 0; x < m_width; x++)
                for (int y = 0; y < m_height; y++)
                {
                    m_Cells[x, y] = floorplan[y, x];
                }
            m_explosionDuration = 2f;
        }
        public bool IsWalkableForPlayer(Point idx)
        {
            switch (m_Cells[idx.X, idx.Y])
            {
                case 1:
                    return true;
                default:
                    return false;
            }
        }
        public void SetCellToBomb(Point idx)
        {
            m_Cells[idx.X, idx.Y] = 0;
        }
        public bool IsCellExploding(Point idx)
        {
            if (m_Cells[idx.X, idx.Y] == 7)
                return true;
            return false;
        }
        public void RegularBombExplosion(Point idx, GameTime gt, int explosionRange)
        {
            // Local Variables
            List<Point> affectedCells = new List<Point>();

            PropagateExplosion(new Point(1, 0));
            PropagateExplosion(new Point(-1, 0));
            PropagateExplosion(new Point(0, 1));
            PropagateExplosion(new Point(0, -1));

            m_Cells[idx.X,idx.Y] = 7;
            affectedCells.Add(new Point(idx.X, idx.Y));
            //DoExplosionDurationCountdown(gt, affectedCells);                                              // TODO FIX

            void PropagateExplosion(Point dir)
            {
                for (int i = 1; i < explosionRange+1; i++)
                {
                    int newX = idx.X + dir.X * i;
                    int newY = idx.Y + dir.Y * i;

                    // Check if out of bounds
                    if (newX < 0 || newX >= m_Cells.GetLength(0) || newY < 0 || newY >= m_Cells.GetLength(1))
                        break;

                    // Stop explosion propagation if it hits a wall or other similar
                    if (m_Cells[newX, newY] == 2 || m_Cells[newX, newY] == 3)
                        break;

                    m_Cells[newX, newY] = 7;

                    // Add the affected cell to the list
                    affectedCells.Add(new Point(newX, newY));

                }
            }

        }
                                                                                                               // TODO FIX
        //public void DoExplosionDurationCountdown(GameTime gt, List<Point> affectedCells)
        //{
           
        //    if (m_explosionDuration > 0)
        //        do
        //            m_explosionDuration -= (float)gt.ElapsedGameTime.TotalSeconds;
        //        while (m_explosionDuration > 0);
        //    void TurnTilesBackToNormal(List<Point> affectedCells)
        //    {
        //        for (int i = 0; i < affectedCells.Count; i++)
        //        {
        //            m_Cells[affectedCells[i].X, affectedCells[i].Y] = 3;
        //        }
        //    }
        //    TurnTilesBackToNormal(affectedCells);
        //}
        public void DrawMe(SpriteBatch sb, List<Texture2D> tiles)
        {
            for (int x = 0; x < m_width; x++)
                for (int y = 0; y < m_height; y++)
                {
                    sb.Draw(tiles[m_Cells[x, y]], new Vector2(x * tiles[0].Width, y * tiles[0].Height),
                        Color.White);
                    sb.DrawString(Game1.debugFont,
                        m_Cells[x, y].ToString(),new Vector2(x * 16, y * 16), Color.White);
                }
        }
    }
}
