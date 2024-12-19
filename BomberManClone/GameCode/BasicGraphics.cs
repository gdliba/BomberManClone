
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
    //class Animated2D : StaticGraphic
    //{
    //    protected Vector2 m_velocity;
    //    protected Vector2 m_rectPos;
    //    protected Rectangle m_rect;
    //    protected Rectangle m_srcRect;
    //    protected float m_updateTrigger;
    //    protected int m_framesPerSecond;
    //    public Animated2D(Texture2D spriteSheet, int fps, Rectangle rect, Point position)
    //        : base(spriteSheet, position)
    //    {
    //        m_srcRect = new Rectangle(0, 0, rect.Width, rect.Height);
    //        m_updateTrigger = 0;
    //        m_framesPerSecond = fps;

    //        m_rectPos = new Vector2(rect.X, rect.Y);
    //        m_velocity = Vector2.Zero;
    //        m_rect = rect;
    //    }

    //    public virtual void updateme(GameTime gt)
    //    {
    //        m_updateTrigger += (float)gt.ElapsedGameTime.TotalSeconds * m_framesPerSecond;

    //        if (m_updateTrigger >= 1)
    //        {
    //            m_updateTrigger = 0;
    //            m_srcRect.X += m_srcRect.Width;
    //            if (m_srcRect.X == m_txr.Width)
    //                m_srcRect.X = 0;
    //        }

    //        m_rectPos = m_rectPos + m_velocity;
    //    }

    //    public override void DrawMe(SpriteBatch sBatch)
    //    {
    //        m_rect.X = (int)m_rectPos.X;
    //        m_rect.Y = (int)m_rectPos.Y;

    //        sBatch.Draw(m_txr, m_rect, m_srcRect, Color.White);
    //    }
    //}
}
