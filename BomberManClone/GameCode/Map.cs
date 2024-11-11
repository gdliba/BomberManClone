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
        public bool isWalkableForPlayer(Vector2 idx)
        {
            // Convert Vector2 to grid indices
            int gridX = (int)idx.X;
            int gridY = (int)idx.Y;
            
            switch (m_Cells[gridX, gridY])
            {
                case 1:
                    return true;
                default:
                    return false;
            }
            
        }
        public void drawme(SpriteBatch sb, List<Texture2D> tiles)
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
