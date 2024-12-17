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
        // Class variables
        private float m_movementSpeed;
        private float m_ghostMovementSpeed;
        private float m_footstepTimer;
        private bool m_isMoving;
        private int m_numberOfBombs;
        private int m_health;
        private int m_explosionRadius;
        private Point m_startPosition;
        private Texture2D m_deathTxr;
        private Color m_tint;
        private bool m_bombDroppedHere;

        private bool m_isWalking;
        private SoundEffect m_footstepSfx;
        private SoundEffect m_deathSfx;


        public int ExplosionRadius { get { return m_explosionRadius; } }
        public int Health { get {return m_health; } }
        public Vector2 Position { get { return m_position; } }
        private PlayerState m_currentState;
        public PlayerState State { get { return m_currentState; } }
        /// <summary>
        /// Class constructor: Ask Game1 for all relevant textures, sound effects and other relevant parameters
        /// for the player animation and tint relevant to which player you are.
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="txr"></param>
        /// <param name="frameCount"></param>
        /// <param name="fps"></param>
        /// <param name="footstepSfx"></param>
        /// <param name="deathTxr"></param>
        /// <param name="deathSfx"></param>
        /// <param name="tint"></param>
        public PC(Point startPos, Texture2D txr, int frameCount, int fps, SoundEffect footstepSfx, Texture2D deathTxr, SoundEffect deathSfx, Color tint)
            : base(startPos, txr, frameCount, fps)
        {
            m_startPosition = startPos;
            m_movementSpeed = .04f;
            m_isMoving = false;
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
            m_bombDroppedHere = false;
        }
        /// <summary>
        /// General Update method: essentially switch between the more relevant and specialised
        /// Update methods depending on PlayerStates.
        /// This Method is a boolean so that when the result is true, Game1 knows to allow the player to place a bomb.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="currentMap"></param>
        /// <param name="kb_curr"></param>
        /// <param name="kb_old"></param>
        /// <param name="curr_pad"></param>
        /// <param name="old_pad"></param>
        /// <returns></returns>
        public bool UpdateMe(GameTime gameTime, Map currentMap,
            KeyboardState kb_curr, KeyboardState kb_old, GamePadState curr_pad, GamePadState old_pad)
        {
            #region setting cell locations
            m_northCell = new Point((int)m_targetLocation.X, (int)m_targetLocation.Y - 1);
            m_southCell = new Point((int)m_targetLocation.X, (int)m_targetLocation.Y + 1);
            m_westCell = new Point((int)m_targetLocation.X - 1, (int)m_targetLocation.Y);
            m_eastCell = new Point((int)m_targetLocation.X + 1, (int)m_targetLocation.Y);
            #endregion

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
        /// <summary>
        /// When the player is "In Play" this update method should be called
        /// This method is responsible for player movements and mots player interactions with the map.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="currentMap"></param>
        /// <param name="kb_curr"></param>
        /// <param name="kb_old"></param>
        /// <param name="curr_pad"></param>
        /// <param name="old_pad"></param>
        /// <returns></returns>
        public bool InPlay(GameTime gameTime, Map currentMap,
            KeyboardState kb_curr, KeyboardState kb_old, GamePadState curr_pad, GamePadState old_pad)
        {
            // check if you're standing on a tile that is exploading
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

            // Keyboard Controls are commented out
            #region KEYBOARD Movement and Cell Occupation
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
            #endregion

            #region PAD Movement and Cell Occupation
            // if player is not currently walking to a target location
            if (!m_isMoving)
            {
                // check which direction the player wants to move in
                    // and act accordingly
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
            // if the player is travelling to a target location
            if (m_position != m_targetLocation)
            {
                // Player is now technically walking
                // So trigger the Footstep method
                m_isWalking = true;

                m_isMoving = true;
                // calculate the direction
                var direction = m_targetLocation - m_position;
                direction.Normalize();
                direction *= m_movementSpeed;
                // move to that location
                m_position += direction;
                // if the player is close enough to the target location
                if ((m_targetLocation - m_position).Length() < .2f)
                {
                    m_isMoving = false;
                    // Snap to the target location
                    m_position = m_targetLocation;
                }
            }
            else
                // don't play the footstep sound effect
                m_isWalking = false;
            #endregion
            // check if the player is allowed to place a bomb
            if (curr_pad.Buttons.X == ButtonState.Pressed && old_pad.Buttons.X == ButtonState.Released && !m_bombDroppedHere)
            {
                if (m_numberOfBombs > 0)
                {
                    m_numberOfBombs--;
                    m_bombDroppedHere = true;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;

            //// KEYBOARD CONTROLS
            /// code to place bombs
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
        }
        /// <summary>
        /// Method that adjusts the player's movement in the four cardinal directions
        /// </summary>
        /// <param name="moveDir"> When the method is called, tell it what cardinal direction you would like to move in</param>
        public override void MoveMe(Direction moveDir)
        {
            m_bombDroppedHere=false;
            base.MoveMe(moveDir);
        }
        /// <summary>
        /// Method that slightly randomises properties of the footstep sound effect
        /// to make it sound less monotonous
        /// </summary>
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
        /// <summary>
        /// When the player is hit and still has lives left they are put into "Ghost State".
        /// They now have slightly different properties such as: increased movement speed,
        /// no collision with boxes, players bombs, and can choose a safe spot to respawn on.
        /// They also can no longer place bombs. (The same button as placing a bomb is used to respawn,
        /// this does not intoduce any new buttons).
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="currentMap"></param>
        /// <param name="kb_curr"></param>
        /// <param name="kb_old"></param>
        /// <param name="curr_pad"></param>
        /// <param name="old_pad"></param>
        public void GhostState(GameTime gameTime, Map currentMap,
            KeyboardState kb_curr, KeyboardState kb_old, GamePadState curr_pad, GamePadState old_pad)
        {
            #region Keyboard Movement
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
            #endregion
            #region PAD Movement and Cell Occupation
            if (!m_isMoving)
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




                m_isMoving = true;
                var direction = m_targetLocation - m_position;
                direction.Normalize();
                direction *= m_ghostMovementSpeed;
                m_position += direction;
                if ((m_targetLocation - m_position).Length() < .2f)
                {
                    m_isMoving = false;
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
                    RespawnFromGhostState();

            //// KEYBOARD CONTROLS 
            ///check if the player can respawn
            //if (kb_curr.IsKeyDown(Keys.F) && kb_old.IsKeyUp(Keys.F))
            //    if (m_shields >= 0 && currentMap.IsCellSafe(m_position.ToPoint()))
            //        Respawn();

        }
        /// <summary>
        /// Method called to switch the player back into "in play" state from "ghost state".
        /// </summary>
        public void RespawnFromGhostState()
        {
            m_currentState = PlayerState.InPlay;
        }
        /// <summary>
        /// Teach the player to "take a hit".
        /// This method is called when player is on a cell that is currently exploading.
        /// </summary>
        /// <param name="currentMap"></param>
        public void TakeAHit(Map currentMap)
        {
            //currentMap.SetCellBackToFloor(m_position.ToPoint());
            m_health--;
            m_position = m_targetLocation;
            m_isMoving = false;
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
        /// <summary>
        ///  Same idea as previous method
        /// </summary>
        public void SpeedPowerUp()
        {
            m_movementSpeed = .08f;
        }
        /// <summary>
        ///  Same idea as previous method
        /// </summary>
        public void MoreBombsPowerUp()
        {
            m_numberOfBombs = 3;
        }
        /// <summary>
        ///  Same idea as previous method
        /// </summary>
        public void ExplosionRadiusPowerUp()
        {
            m_explosionRadius = 4;
        }
        /// <summary>
        /// Reset Method changes all relevant variables back to the starting ones in order to play
        /// the game again, without closing and reopening the game.
        /// </summary>
        public void Reset()
        {
            m_isWalking = false;
            m_movementSpeed = .04f;
            m_isMoving = false;
            m_numberOfBombs = 2;
            m_ghostMovementSpeed = .5f;
            m_health = 3;
            m_currentState = PlayerState.InPlay;
            m_explosionRadius = 3;
            m_targetLocation = m_startPosition.ToVector2();
            m_position = m_startPosition.ToVector2 ();
            m_bombDroppedHere = false;
        }
        /// <summary>
        /// Player Draw Method: take in the map's tile dimensions and draw accordingly
        /// Take into account the Player's State and draw accordingly
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="gt"></param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
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
