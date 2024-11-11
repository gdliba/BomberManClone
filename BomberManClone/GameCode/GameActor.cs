using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BomberManClone
{
    enum Direction
    {
        North,
        East,
        South,
        West,
        None
    }
    class GameActor
    {
        protected Direction m_facing;
        protected Vector2 m_position;

        protected Texture2D m_txr;
        private int m_frameCount;
        private int m_animFrame;
        protected Rectangle m_sourceRect;

        private float m_updateTrigger;
        private int m_fps;

        // making life easier by making certain points into variables
        protected Vector2 m_northCell, m_southCell, m_eastCell, m_westCell;

        public GameActor(Point startPos, Texture2D txr, int frameCount, int fps)
        {
            m_position.X = startPos.X;
            m_position.Y = startPos.Y;
            m_txr = txr;

            m_frameCount = frameCount;
            m_animFrame = 0;
            m_sourceRect = new Rectangle(0, 0, txr.Width / m_frameCount, txr.Height/4);

            m_updateTrigger = 0;
            m_fps = fps;
            m_facing = Direction.South;

        }
        public void moveme(Direction moveDir, float deltaTime)
        {
            float speed = .1f;
            m_facing = moveDir;

            switch (moveDir)
            {
                case Direction.North:
                    m_position.Y-=speed * deltaTime;
                    break;
                case Direction.South:
                    m_position.Y+=speed * deltaTime;
                    break;
                case Direction.East:
                    m_position.X+=speed * deltaTime;
                    break;
                case Direction.West:
                    m_position.X-=speed * deltaTime;
                    break;
            }
        }

        public virtual void drawme(SpriteBatch sb, GameTime gt, int tileWidth, int tileHeight)
        {
            m_sourceRect.Y = (int)m_facing * m_sourceRect.Height;
            m_updateTrigger += (float)gt.ElapsedGameTime.TotalSeconds * m_fps;
            if (m_updateTrigger >= 1)
            {
                m_updateTrigger = 0;

                m_animFrame = (m_animFrame + 1) % m_frameCount;
                m_sourceRect.X = m_animFrame * m_sourceRect.Width;
            }
            sb.Draw(m_txr,
                new Vector2(m_position.X*16 ,(m_position.Y*16)-4), m_sourceRect, Color.White);
        }
    }
}
