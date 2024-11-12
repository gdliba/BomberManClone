using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BomberManClone
{
    class PC : GameActor
    {
        private float m_movementSpeed;
        private bool m_hasMoved;
        private int m_numberOfBombs;
        public Vector2 Position { get { return m_position; } }
        public PC(Point startPos, Texture2D txr, int frameCount, int fps)
            :base(startPos, txr, frameCount, fps)
        {
            m_movementSpeed = .03f;
            m_hasMoved = false;
            m_numberOfBombs = 2;
        }
        public bool UpdateMe(GameTime gameTime, Map currentMap,
            KeyboardState kb_curr, KeyboardState kb_old)
        {
            #region setting cell locations
            m_northCell = new Point((int)m_targetLocation.X, (int)m_targetLocation.Y - 1);
            m_southCell = new Point((int)m_targetLocation.X, (int)m_targetLocation.Y + 1);
            m_westCell = new Point((int)m_targetLocation.X - 1, (int)m_targetLocation.Y);
            m_eastCell = new Point((int)m_targetLocation.X + 1, (int)m_targetLocation.Y);
            #endregion

            #region Movement
            if (!m_hasMoved)
            {
                if (kb_curr.IsKeyDown(Keys.W) && kb_old.IsKeyUp(Keys.W))
                {
                    m_facing = Direction.North;
                    if (currentMap.IsWalkableForPlayer(m_northCell))
                        MoveMe(Direction.North);
                }
                if (kb_curr.IsKeyDown(Keys.S) && kb_old.IsKeyUp(Keys.S))
                {
                    m_facing = Direction.South;
                    if (currentMap.IsWalkableForPlayer(m_southCell))
                        MoveMe(Direction.South);
                }
                if (kb_curr.IsKeyDown(Keys.A) && kb_old.IsKeyUp(Keys.A))
                {
                    m_facing = Direction.West;
                    if (currentMap.IsWalkableForPlayer(m_westCell))
                        MoveMe(Direction.West);
                }
                if (kb_curr.IsKeyDown(Keys.D) && kb_old.IsKeyUp(Keys.D))
                {
                    m_facing = Direction.East;
                    if (currentMap.IsWalkableForPlayer(m_eastCell))
                        MoveMe(Direction.East);
                }
            }
            if(m_position != m_targetLocation)
            {
                m_hasMoved = true;
                var direction = m_targetLocation - m_position;
                direction.Normalize();
                direction *= m_movementSpeed;
                m_position += direction;
                if ((m_targetLocation - m_position).Length() < .2f)
                {
                    m_hasMoved = false;
                    m_position = m_targetLocation;
                }
            }
            #endregion
            if (kb_curr.IsKeyDown(Keys.F) && kb_old.IsKeyUp(Keys.F))
            {
                if (m_numberOfBombs != 0)
                {
                    m_numberOfBombs--;
                    return 1;
                }
            }
            else
                return 0;

        }
        public override void DrawMe(SpriteBatch sb, GameTime gt, int tileWidth, int tileHeight)
        {
            base.DrawMe(sb, gt, tileWidth, tileHeight);
        }
    }
}
