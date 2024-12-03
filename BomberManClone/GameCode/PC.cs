using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BomberManClone
{
    enum PlayerState
    {
        InPlay,
        Ghost,
        Dead
    }
    class PC : GameActor
    {
        private float m_movementSpeed;
        private float m_ghostMovementSpeed;
        private bool m_hasMoved;
        private int m_numberOfBombs;
        private int m_shields;
        private int m_explosionRadius;
        private Point m_startPosition;
        public int ExplosionRadius { get { return m_explosionRadius; } }
        public int Shields { get {return m_shields; } }
        public Vector2 Position { get { return m_position; } }
        private PlayerState m_currentState;
        public PC(Point startPos, Texture2D txr, int frameCount, int fps)
            :base(startPos, txr, frameCount, fps)
        {
            m_startPosition = startPos;
            m_movementSpeed = .04f;
            m_hasMoved = false;
            m_numberOfBombs = 2;
            m_ghostMovementSpeed = .5f;
            m_shields = 3;
            m_currentState = PlayerState.InPlay;
            m_explosionRadius = 3;
        }
        public bool UpdateMe(GameTime gameTime, Map currentMap,
            KeyboardState kb_curr, KeyboardState kb_old)
        {
            switch (m_currentState)
            {
                case PlayerState.InPlay:
                    return InPlay(gameTime, currentMap, kb_curr, kb_old);
                case PlayerState.Ghost:
                    GhostState(gameTime, currentMap, kb_curr, kb_old);
                    return false;
                case PlayerState.Dead:
                default: 
                    return false;
            }
        }
        public bool InPlay(GameTime gameTime, Map currentMap,
            KeyboardState kb_curr, KeyboardState kb_old)
        {

            if (currentMap.IsCellExploding(m_position.ToPoint()))
                TakeAHit(currentMap);
            
            // React to Powerups
            //if (currentMap.IsPowerUpOnCell(m_position.ToPoint()))


            // Declare that you are occupying the cell so that other players cannot walk into you
            currentMap.PlayerIsOccupyingCell(m_position.ToPoint());


            #region setting cell locations
            m_northCell = new Point((int)m_targetLocation.X, (int)m_targetLocation.Y - 1);
            m_southCell = new Point((int)m_targetLocation.X, (int)m_targetLocation.Y + 1);
            m_westCell = new Point((int)m_targetLocation.X - 1, (int)m_targetLocation.Y);
            m_eastCell = new Point((int)m_targetLocation.X + 1, (int)m_targetLocation.Y);
            #endregion

            #region Movement and Cell Occupation
            if (!m_hasMoved)
            {
                if (kb_curr.IsKeyDown(Keys.W) && kb_old.IsKeyUp(Keys.W))
                {
                    m_facing = Direction.North;
                    if (currentMap.IsWalkableForPlayer(m_northCell))
                    {
                        m_sourceLocation = m_position;
                        MoveMe(Direction.North);
                    }
                }
                if (kb_curr.IsKeyDown(Keys.S) && kb_old.IsKeyUp(Keys.S))
                {
                    m_facing = Direction.South;
                    if (currentMap.IsWalkableForPlayer(m_southCell))
                    {
                        m_sourceLocation = m_position;
                        MoveMe(Direction.South);
                    }
                }
                if (kb_curr.IsKeyDown(Keys.A) && kb_old.IsKeyUp(Keys.A))
                {
                    m_facing = Direction.West;
                    if (currentMap.IsWalkableForPlayer(m_westCell))
                    {
                        m_sourceLocation = m_position;
                        MoveMe(Direction.West);
                    }
                }
                if (kb_curr.IsKeyDown(Keys.D) && kb_old.IsKeyUp(Keys.D))
                {
                    m_facing = Direction.East;
                    if (currentMap.IsWalkableForPlayer(m_eastCell))
                    {
                        m_sourceLocation = m_position;
                        MoveMe(Direction.East);
                    }
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
                    // Snap to the target location
                    m_position = m_targetLocation;
                }
            }
            #endregion
            if (kb_curr.IsKeyDown(Keys.F) && kb_old.IsKeyUp(Keys.F))
            {
                if (m_numberOfBombs > 0)
                {
                    m_numberOfBombs--;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;

        }
        public void GhostState(GameTime gameTime, Map currentMap,
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
                    if (currentMap.IsWalkableForGhost(m_northCell))
                    {
                        MoveMe(Direction.North);
                    }
                }
                if (kb_curr.IsKeyDown(Keys.S) && kb_old.IsKeyUp(Keys.S))
                {
                    m_facing = Direction.South;
                    if (currentMap.IsWalkableForGhost(m_southCell))
                    {
                        MoveMe(Direction.South);
                    }
                }
                if (kb_curr.IsKeyDown(Keys.A) && kb_old.IsKeyUp(Keys.A))
                {
                    m_facing = Direction.West;
                    if (currentMap.IsWalkableForGhost(m_westCell))
                    {
                        MoveMe(Direction.West);
                    }
                }
                if (kb_curr.IsKeyDown(Keys.D) && kb_old.IsKeyUp(Keys.D))
                {
                    m_facing = Direction.East;
                    if (currentMap.IsWalkableForGhost(m_eastCell))
                    {
                        MoveMe(Direction.East);
                    }
                }
            }
            if (m_position != m_targetLocation)
            {
                m_hasMoved = true;
                var direction = m_targetLocation - m_position;
                direction.Normalize();
                direction *= m_ghostMovementSpeed;
                m_position += direction;
                if ((m_targetLocation - m_position).Length() < .2f)
                {
                    m_hasMoved = false;
                    // Snap to the target location
                    m_position = m_targetLocation;

                }
            }
            #endregion

           // check if the player can respawn
            if (kb_curr.IsKeyDown(Keys.F) && kb_old.IsKeyUp(Keys.F))
                if (m_shields >= 0 && currentMap.IsCellSafe(m_position.ToPoint()))
                    Respawn();
  
        }
        public void Respawn()
        {
            m_currentState = PlayerState.InPlay;
        }
        public void TakeAHit(Map currentMap)
        {
            //currentMap.SetCellBackToFloor(m_position.ToPoint());
            m_shields--;
            m_position = m_targetLocation;
            m_hasMoved = false;
            if (m_shields >= 0)
                m_currentState = PlayerState.Ghost;
            else
                m_currentState = PlayerState.Dead;
        }
        /// <summary>
        /// The use of this method is a convenient way of maintaining incapsulation
        ///  and avoiding turning 'm_numberOfBombs' into a public variable
        /// </summary>
        public void IncreaseBombCount()
        {
             m_numberOfBombs++;
        }
        public void SpeedPowerUp()
        {
            m_movementSpeed = .08f;
        }
        public void MoreBombsPowerUp()
        {
            m_numberOfBombs = 3;
        }
        public void ExplosionRadiusPowerUp()
        {
            m_explosionRadius = 4;
        }
        public void Reset()
        {
            m_movementSpeed = .04f;
            m_hasMoved = false;
            m_numberOfBombs = 2;
            m_ghostMovementSpeed = .5f;
            m_shields = 3;
            m_currentState = PlayerState.InPlay;
            m_explosionRadius = 3;
            m_targetLocation = m_startPosition.ToVector2();
            m_position = m_startPosition.ToVector2 ();
        }
        public override void DrawMe(SpriteBatch sb, GameTime gt, int tileWidth, int tileHeight, Color color)
        {
            switch (m_currentState)
            {
                case PlayerState.InPlay:
                    base.DrawMe(sb, gt, tileWidth, tileHeight, Color.White);
                    break;
                case PlayerState.Ghost:
                    // Draw this when player is in ghost form
                    base.DrawMe(sb, gt, tileWidth, tileHeight, Color.Gray);
                    break;
                case PlayerState.Dead:
                    // Draw this when player is dead
                    base.DrawMe(sb, gt, tileWidth, tileHeight, Color.Red);
                    break;
            }
        }
    }
}
