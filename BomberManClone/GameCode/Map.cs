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
        public void RegularBombExplosion(Point idx)
        {
            PropagateExplosion(new Point(1, 0));
            PropagateExplosion(new Point(-1, 0));
            PropagateExplosion(new Point(0, 1));
            PropagateExplosion(new Point(0, -1));

            void PropagateExplosion(Point dir)
            {
                for (int i = 1; i < 4; i++)
                {
                    if (m_Cells[idx.X + dir.X * i, idx.Y + dir.Y * i] == 2) 
                        break;

                    m_Cells[idx.X + dir.X * i, idx.Y + dir.Y * i] = 0;
                }
            }
        }
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
