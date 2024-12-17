using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        protected Vector2 m_targetLocation;
        protected Vector2 m_sourceLocation;

        protected Texture2D m_txr;
        private int m_frameCount;
        private int m_animFrame;
        protected Rectangle m_sourceRect;

        private float m_updateTrigger;
        private int m_fps;

        // making life easier by making certain points
        // into variables that will be referenced often
        protected Point m_northCell, m_southCell, m_eastCell, m_westCell;

        public GameActor(Point startPos, Texture2D txr, int frameCount, int fps)
        {
            m_targetLocation = m_position = startPos.ToVector2();
            m_txr = txr;

            m_frameCount = frameCount;
            m_animFrame = 0;
            m_sourceRect = new Rectangle(0, 0, txr.Width / m_frameCount, txr.Height/4);

            m_updateTrigger = 0;
            m_fps = fps;
            m_facing = Direction.South;
        }
        /// <summary>
        /// Move the GameActor in the cardinal directions when called.
        /// </summary>
        /// <param name="moveDir"> Takes in the cardinal direction that the GameActor wants to move in</param>
        public virtual void MoveMe(Direction moveDir)
        {
            if (m_targetLocation != m_position)
                return;

            m_facing = moveDir;

            switch (moveDir)
            {
                case Direction.North:
                    m_targetLocation += new Vector2(0, -1);
                    break;
                case Direction.South:
                    m_targetLocation += new Vector2(0, +1);
                    break;
                case Direction.East:
                    m_targetLocation += new Vector2(+1, 0);
                    break;
                case Direction.West:
                    m_targetLocation += new Vector2(-1, 0);
                    break;
            }
        }
        /// <summary>
        /// Draw method that cycles through the spritesheet and "animates" the game actor.
        /// Draws the sprite with respect to the tile height and width.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="gt"></param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        /// <param name="color"></param>
        public virtual void DrawMe(SpriteBatch sb, GameTime gt, int tileWidth, int tileHeight, Color color)
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
                new Vector2(m_position.X*64 ,(m_position.Y*64)-4), m_sourceRect, color);
        }
    }
}
