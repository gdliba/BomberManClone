using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BomberManClone
{
    enum PowerUpType
    {
        Speed,
        MoreBombs,
        ExplosionRadius,
        Count
    }
    enum PowerUpState
    {
        Spawn,
        InPlay,
        Dead
    }
    /// <summary>
    /// Powerup class inherits from static graphic as it has a lot in common.
    /// It's a static graphic with more fancy states and methods.
    /// I opted in to using two enums for this class:
    /// One for the type of powerup and one for the state the powerup is in.
    /// I chose to do it this way as I can teach the powerup to act accordignly
    /// depending on what type of powerup it is.
    /// It is also easy to expand if I were to add more types of powerups with more effects.
    /// </summary>
    class PowerUp : StaticGraphic
    {
        private PowerUpType m_type;
        public PowerUpType Type {  get { return m_type; } }
        private PowerUpState m_state;
        public PowerUpState State {  get { return m_state; } }

        public Point Position { get { return m_position; } }

        private float m_powerUpDuration;
        private SoundEffect m_spawnSfx;

        public PowerUp(Texture2D txr, Point position, PowerUpType type, SoundEffect spawnSfx)
            : base(txr, position)
        {
            m_txr = txr;
            m_position = position;
            m_type = type;
            m_state = PowerUpState.Spawn;
            m_powerUpDuration = 10f;
            m_spawnSfx = spawnSfx;
        }
        /// <summary>
        /// Main update method for the class.
        /// Switches states and acts accordingly.
        /// </summary>
        /// <param name="gt"></param>
        /// <param name="currentMap"></param>
        public void UpdateMe(GameTime gt, Map currentMap)
        {
            switch (m_state)
            {
                case PowerUpState.Spawn:
                    // When told to, place a powerup on the corresponding cell
                    PlacePowerUpOnCell(currentMap);
                    // then change state
                    m_state = PowerUpState.InPlay;
                    break;
                case PowerUpState.InPlay:
                    InPlay(gt, currentMap);
                    break;
                case PowerUpState.Dead:
                    break;
            }
        }
        /// <summary>
        /// This method only really checks to see if there is an explosion where
        /// the powerup is attempting to spawn.
        /// If there isn't, you're free to spawn.
        /// </summary>
        /// <param name="currentMap"></param>
        public void PlacePowerUpOnCell(Map currentMap)
        {
            // If the cell you're about to spawn on is exploding, don't spawn
            if (!currentMap.IsCellExploding(m_position))
            {
                currentMap.PowerUpOnCell(m_position);
                m_spawnSfx.Play();
            }
        }
        /// <summary>
        /// This method counts down the timer that dictates the lifetime of the powerup.
        /// It then changes state to "Dead" to notify Game1 that it should be deleted.
        /// </summary>
        /// <param name="gt"></param>
        /// <param name="currentMap"></param>
        public void InPlay(GameTime gt, Map currentMap)
        {
            m_powerUpDuration -= (float)gt.ElapsedGameTime.TotalSeconds;
            if (m_powerUpDuration < 0)
                m_state = PowerUpState.Dead;
            if (currentMap.IsCellExploding(m_position))
                m_state = PowerUpState.Dead;
        }
        /// <summary>
        /// Draw method currently only draws the poweup on screen.
        /// One thing I would have liked to do is make it blink accordigly,
        /// giving a sense of urgency to the players to pick them up.
        /// </summary>
        /// <param name="sb"></param>
        public override void DrawMe(SpriteBatch sb)
        {
            base.DrawMe(sb);
        }
    }
}
