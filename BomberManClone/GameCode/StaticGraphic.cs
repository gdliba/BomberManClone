
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BomberManClone
{
    class StaticGraphic
    {
        protected Texture2D m_txr;
        protected Point m_position;

        public StaticGraphic(Texture2D txr, Point position)
        {
            m_txr = txr;
            m_position = position;
        }
        public virtual void DrawMe(SpriteBatch sb)
        {
            sb.Draw(m_txr, new Vector2(m_position.X * 16, m_position.Y * 16), Color.White);
        }
    }
}
