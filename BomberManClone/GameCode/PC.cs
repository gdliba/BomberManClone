using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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
        private float m_footstepTimer;
        private bool m_hasMoved;
        private int m_numberOfBombs;
        private int m_health;
        private int m_explosionRadius;
        private Point m_startPosition;
        private Texture2D m_deathTxr;
        private Color m_tint;

        private bool m_isWalking;
        private SoundEffect m_footstepSfx;
        private SoundEffect m_deathSfx;


        public int ExplosionRadius { get { return m_explosionRadius; } }
        public int Health { get {return m_health; } }
        public Vector2 Position { get { return m_position; } }
        private PlayerState m_currentState;
        public PlayerState State { get { return m_currentState; } }
        public PC(Point startPos, Texture2D txr, int frameCount, int fps, SoundEffect footstepSfx, Texture2D deathTxr, SoundEffect deathSfx, Color tint)
            : base(startPos, txr, frameCount, fps)
        {
            m_startPosition = startPos;
            m_movementSpeed = .04f;
            m_hasMoved = false;
            m_numberOfBombs = 2;
            m_ghostMovementSpeed = .5f;
            m_health = 3;
            m_currentState = PlayerState.InPlay;
            m_explosionRadius = 3;
            m_footstepSfx = footstepSfx;
            m_footstepTimer = m_movementSpeed;
            m_deathTxr = deathTxr;
            m_deathSfx = deathSfx;
            m_tint = tint;
        }
        public bool UpdateMe(GameTime gameTime, Map currentMap,
            KeyboardState kb_curr, KeyboardState kb_old, GamePadState curr_pad, GamePadState old_pad)
        {
            switch (m_currentState)
            {
                case PlayerState.InPlay:
                    return InPlay(gameTime, currentMap, kb_curr, kb_old, curr_pad, old_pad);
                case PlayerState.Ghost:
                    GhostState(gameTime, currentMap, kb_curr, kb_old, curr_pad, old_pad);
                    return false;
                case PlayerState.Dead:
                default: 
                    return false;
            }
        }
        public bool InPlay(GameTime gameTime, Map currentMap,
            KeyboardState kb_curr, KeyboardState kb_old, GamePadState curr_pad, GamePadState old_pad)
        {

            if (currentMap.IsCellExploding(m_position.ToPoint()))
                TakeAHit(currentMap);

             // Play the footstep sounds at a speed that makes sense
            if (m_isWalking && m_footstepTimer <= 0)
            {
                PlayFootstep();
                m_footstepTimer = (.008f/m_movementSpeed); // Reset timer based on speed
            }
            m_footstepTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;



            // Declare that you are occupying the cell so that other players cannot walk into you
            currentMap.PlayerIsOccupyingCell(m_position.ToPoint());


            #region setting cell locations
            m_northCell = new Point((int)m_targetLocation.X, (int)m_targetLocation.Y - 1);
            m_southCell = new Point((int)m_targetLocation.X, (int)m_targetLocation.Y + 1);
            m_westCell = new Point((int)m_targetLocation.X - 1, (int)m_targetLocation.Y);
            m_eastCell = new Point((int)m_targetLocation.X + 1, (int)m_targetLocation.Y);
            #endregion

            //#region Movement and Cell Occupation
            //if (!m_hasMoved)
            //{
            //    if (kb_curr.IsKeyDown(Keys.W) && kb_old.IsKeyUp(Keys.W))
            //    {
            //        m_facing = Direction.North;
            //        if (currentMap.IsWalkableForPlayer(m_northCell))
            //        {
            //            m_sourceLocation = m_position;
            //            MoveMe(Direction.North);
            //        }
            //    }
            //    if (kb_curr.IsKeyDown(Keys.S) && kb_old.IsKeyUp(Keys.S))
            //    {
            //        m_facing = Direction.South;
            //        if (currentMap.IsWalkableForPlayer(m_southCell))
            //        {
            //            m_sourceLocation = m_position;
            //            MoveMe(Direction.South);
            //        }
            //    }
            //    if (kb_curr.IsKeyDown(Keys.A) && kb_old.IsKeyUp(Keys.A))
            //    {
            //        m_facing = Direction.West;
            //        if (currentMap.IsWalkableForPlayer(m_westCell))
            //        {
            //            m_sourceLocation = m_position;
            //            MoveMe(Direction.West);
            //        }
            //    }
            //    if (kb_curr.IsKeyDown(Keys.D) && kb_old.IsKeyUp(Keys.D))
            //    {
            //        m_facing = Direction.East;
            //        if (currentMap.IsWalkableForPlayer(m_eastCell))
            //        {
            //            m_sourceLocation = m_position;
            //            MoveMe(Direction.East);
            //        }
            //    }
            //}
            //if (m_position != m_targetLocation)
            //{
            //    // Player is now technically walking
            //    // So trigger the Footstep method
            //    m_isWalking = true;




            //    m_hasMoved = true;
            //    var direction = m_targetLocation - m_position;
            //    direction.Normalize();
            //    direction *= m_movementSpeed;
            //    m_position += direction;
            //    if ((m_targetLocation - m_position).Length() < .2f)
            //    {
            //        m_hasMoved = false;
            //        // Snap to the target location
            //        m_position = m_targetLocation;
            //    }
            //}
            //else
            //    m_isWalking = false;
            //#endregion
            //if (kb_curr.IsKeyDown(Keys.F) && kb_old.IsKeyUp(Keys.F))
            //{
            //    if (m_numberOfBombs > 0)
            //    {
            //        m_numberOfBombs--;
            //        return true;
            //    }
            //    else
            //        return false;
            //}
            //else
            //    return false;
            #region PAD Movement and Cell Occupation
            if (!m_hasMoved)
            {
                if (curr_pad.DPad.Up == ButtonState.Pressed && old_pad.DPad.Up == ButtonState.Released)
                {
                    m_facing = Direction.North;
                    if (currentMap.IsWalkableForPlayer(m_northCell))
                    {
                        m_sourceLocation = m_position;
                        MoveMe(Direction.North);
                    }
                }
                if (curr_pad.DPad.Down == ButtonState.Pressed && old_pad.DPad.Down == ButtonState.Released)
                {
                    m_facing = Direction.South;
                    if (currentMap.IsWalkableForPlayer(m_southCell))
                    {
                        m_sourceLocation = m_position;
                        MoveMe(Direction.South);
                    }
                }
                if (curr_pad.DPad.Left == ButtonState.Pressed && old_pad.DPad.Left == ButtonState.Released)
                {
                    m_facing = Direction.West;
                    if (currentMap.IsWalkableForPlayer(m_westCell))
                    {
                        m_sourceLocation = m_position;
                        MoveMe(Direction.West);
                    }
                }
                if (curr_pad.DPad.Right == ButtonState.Pressed && old_pad.DPad.Right == ButtonState.Released)
                {
                    m_facing = Direction.East;
                    if (currentMap.IsWalkableForPlayer(m_eastCell))
                    {
                        m_sourceLocation = m_position;
                        MoveMe(Direction.East);
                    }
                }
            }
            if (m_position != m_targetLocation)
            {
                // Player is now technically walking
                // So trigger the Footstep method
                m_isWalking = true;




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
            else
                m_isWalking = false;
            #endregion
            if (curr_pad.Buttons.X == ButtonState.Pressed && old_pad.Buttons.X == ButtonState.Released)
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
        public void PlayFootstep()
        {
            SoundEffectInstance instance = m_footstepSfx.CreateInstance();
            // Randomize pitch
            instance.Pitch = (float)(Game1.RNG.NextDouble()*.5);

            // Randomize volume
            instance.Volume = (float)(0.3 + Game1.RNG.NextDouble() * 0.4);

            // Randomize panning
            instance.Pan = (float)(Game1.RNG.NextDouble() * 0.4 - 0.2);

            instance.Play();
        }
        public void GhostState(GameTime gameTime, Map currentMap,
            KeyboardState kb_curr, KeyboardState kb_old, GamePadState curr_pad, GamePadState old_pad)
        {
            #region setting cell locations
            m_northCell = new Point((int)m_targetLocation.X, (int)m_targetLocation.Y - 1);
            m_southCell = new Point((int)m_targetLocation.X, (int)m_targetLocation.Y + 1);
            m_westCell = new Point((int)m_targetLocation.X - 1, (int)m_targetLocation.Y);
            m_eastCell = new Point((int)m_targetLocation.X + 1, (int)m_targetLocation.Y);
            #endregion

            //#region Movement
            //if (!m_hasMoved)
            //{
            //    if (kb_curr.IsKeyDown(Keys.W) && kb_old.IsKeyUp(Keys.W))
            //    {
            //        m_facing = Direction.North;
            //        if (currentMap.IsWalkableForGhost(m_northCell))
            //        {
            //            MoveMe(Direction.North);
            //        }
            //    }
            //    if (kb_curr.IsKeyDown(Keys.S) && kb_old.IsKeyUp(Keys.S))
            //    {
            //        m_facing = Direction.South;
            //        if (currentMap.IsWalkableForGhost(m_southCell))
            //        {
            //            MoveMe(Direction.South);
            //        }
            //    }
            //    if (kb_curr.IsKeyDown(Keys.A) && kb_old.IsKeyUp(Keys.A))
            //    {
            //        m_facing = Direction.West;
            //        if (currentMap.IsWalkableForGhost(m_westCell))
            //        {
            //            MoveMe(Direction.West);
            //        }
            //    }
            //    if (kb_curr.IsKeyDown(Keys.D) && kb_old.IsKeyUp(Keys.D))
            //    {
            //        m_facing = Direction.East;
            //        if (currentMap.IsWalkableForGhost(m_eastCell))
            //        {
            //            MoveMe(Direction.East);
            //        }
            //    }
            //}
            //if (m_position != m_targetLocation)
            //{
            //    m_hasMoved = true;
            //    var direction = m_targetLocation - m_position;
            //    direction.Normalize();
            //    direction *= m_ghostMovementSpeed;
            //    m_position += direction;
            //    if ((m_targetLocation - m_position).Length() < .2f)
            //    {
            //        m_hasMoved = false;
            //        // Snap to the target location
            //        m_position = m_targetLocation;

            //    }
            //}
            //#endregion
            #region PAD Movement and Cell Occupation
            if (!m_hasMoved)
            {
                if (curr_pad.DPad.Up == ButtonState.Pressed && old_pad.DPad.Up == ButtonState.Released)
                {
                    m_facing = Direction.North;
                    if (currentMap.IsWalkableForGhost(m_northCell))
                    {
                        m_sourceLocation = m_position;
                        MoveMe(Direction.North);
                    }
                }
                if (curr_pad.DPad.Down == ButtonState.Pressed && old_pad.DPad.Down == ButtonState.Released)
                {
                    m_facing = Direction.South;
                    if (currentMap.IsWalkableForGhost(m_southCell))
                    {
                        m_sourceLocation = m_position;
                        MoveMe(Direction.South);
                    }
                }
                if (curr_pad.DPad.Left == ButtonState.Pressed && old_pad.DPad.Left == ButtonState.Released)
                {
                    m_facing = Direction.West;
                    if (currentMap.IsWalkableForGhost(m_westCell))
                    {
                        m_sourceLocation = m_position;
                        MoveMe(Direction.West);
                    }
                }
                if (curr_pad.DPad.Right == ButtonState.Pressed && old_pad.DPad.Right == ButtonState.Released)
                {
                    m_facing = Direction.East;
                    if (currentMap.IsWalkableForGhost(m_eastCell))
                    {
                        m_sourceLocation = m_position;
                        MoveMe(Direction.East);
                    }
                }
            }
            if (m_position != m_targetLocation)
            {
                // Player is now technically walking
                // So trigger the Footstep method
                m_isWalking = true;




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
            else
                m_isWalking = false;
            #endregion
            // check if the player can respawn
            if (curr_pad.Buttons.X == ButtonState.Pressed && old_pad.Buttons.X == ButtonState.Released)
                if (m_health >= 0 && currentMap.IsCellSafe(m_position.ToPoint()))
                    Respawn();

            //// check if the player can respawn
            //if (kb_curr.IsKeyDown(Keys.F) && kb_old.IsKeyUp(Keys.F))
            //    if (m_shields >= 0 && currentMap.IsCellSafe(m_position.ToPoint()))
            //        Respawn();

        }
        public void Respawn()
        {
            m_currentState = PlayerState.InPlay;
        }
        public void TakeAHit(Map currentMap)
        {
            //currentMap.SetCellBackToFloor(m_position.ToPoint());
            m_health--;
            m_position = m_targetLocation;
            m_hasMoved = false;
            if (m_health >= 1)
                m_currentState = PlayerState.Ghost;
            else
            {
                m_deathSfx.Play();
                m_currentState = PlayerState.Dead;
            }
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
            m_isWalking = false;
            m_movementSpeed = .04f;
            m_hasMoved = false;
            m_numberOfBombs = 2;
            m_ghostMovementSpeed = .5f;
            m_health = 3;
            m_currentState = PlayerState.InPlay;
            m_explosionRadius = 3;
            m_targetLocation = m_startPosition.ToVector2();
            m_position = m_startPosition.ToVector2 ();
        }
        public void DrawMe(SpriteBatch sb, GameTime gt, int tileWidth, int tileHeight)
        {
            switch (m_currentState)
            {
                case PlayerState.InPlay:
                    base.DrawMe(sb, gt, tileWidth, tileHeight, m_tint);
                    break;
                case PlayerState.Ghost:
                    // Draw this when player is in ghost form
                    base.DrawMe(sb, gt, tileWidth, tileHeight, m_tint * .5f);
                    break;
                case PlayerState.Dead:
                    // Draw this when player is dead
                    sb.Draw(m_deathTxr, new Vector2(m_position.X*tileWidth, m_position.Y*tileHeight-4), Color.White);
                    break;
            }
        }
    }
}
