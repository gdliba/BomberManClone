
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics.Metrics;

namespace BomberManClone
{
    /// <summary>
    /// Class used by most to help draw them efficiently
    /// </summary>
    class StaticGraphic
    {
        protected Texture2D m_txr;
        protected Point m_position;

        public StaticGraphic(Texture2D txr, Point position)
        {
            m_txr = txr;
            m_position = position;
        }
        /// <summary>
        /// Method used to draw the screen-wide overlay in ths start screen
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="rect"></param>
        /// <param name="tint"></param>
        public virtual void DrawMeAsRect(SpriteBatch sb, Rectangle rect, Color tint)
        {
            sb.Draw(m_txr, rect, tint);
        }
        public virtual void DrawMe(SpriteBatch sb)
        {
            sb.Draw(m_txr, new Vector2(m_position.X * 64, m_position.Y * 64), Color.White);
        }
    }
}
