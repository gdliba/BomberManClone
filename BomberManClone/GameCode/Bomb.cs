
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BomberManClone
{
    enum BombStates
    {
        Placed,
        Primed,
        Exploding,
        Dead
    }
    /// <summary>
    /// This class creates the bombs that the players will use throughout the
    /// entire game.
    /// </summary>
    class Bomb : StaticGraphic
    {
        private float m_fuseCounter;
        private float m_fuseTrigger;
        private int m_explosionRadius;
        private SoundEffectInstance m_fuzeInstance;

        private BombStates m_state;
        public BombStates State { get { return m_state; } }

        public event Action OnExplode; // Event that will notify when the bomb explodes

        /// <summary>
        /// The constructor takes in the "explosionRadius" from the player because,
        /// if the player has recieved the relevant powerup the explosion radius
        /// should increase.
        /// </summary>
        /// <param name="txr"></param>
        /// <param name="placedPosition"></param>
        /// <param name="explosionRadius"></param>
        /// <param name="fuzeSfx"></param>
        public Bomb(Texture2D txr, Point placedPosition, int explosionRadius, SoundEffect fuzeSfx)
            : base (txr, placedPosition)
        {
            m_txr = txr;
            m_position = placedPosition;
            m_state = BombStates.Placed;

            m_fuseCounter = 3;
            m_fuseTrigger = 0;

            m_explosionRadius = explosionRadius;
            m_fuzeInstance = fuzeSfx.CreateInstance();
            m_fuzeInstance.Volume = .2f;
        }
        /// <summary>
        /// Main update method of the class, switches states and checks if has been hit by
        /// another bomb's explosion.
        /// </summary>
        /// <param name="gt"></param>
        /// <param name="currentMap"></param>
        public void UpdateMe(GameTime gt,Map currentMap)
        {
            // if hit by an explosion of another bomb
            if (currentMap.IsCellExploding(m_position))
                m_state=BombStates.Exploding;
            switch (m_state)
            {
                // Placed state is mainly used as a convenient transition state in which
                // the "fuze" soundEffect is called only once.
                case BombStates.Placed:
                    m_fuzeInstance.Play();
                    // Set the cell you're placed on to a "bomb" cell, meaning
                    // that the player is now blocked by it.
                    currentMap.SetCellToBomb(m_position);
                    m_state = BombStates.Primed;
                    break;
                case BombStates.Primed:
                    currentMap.SetCellToBomb(m_position);
                    CountDown(gt);
                    break;
                case BombStates.Exploding:
                    Explode(currentMap, gt);
                    break;
            }
        }
        /// <summary>
        /// Method in charge of counting down the fuze timer.
        /// </summary>
        /// <param name="gt"></param>
        public void CountDown(GameTime gt)
        {
            m_fuseCounter -=(float)gt.ElapsedGameTime.TotalSeconds;
            if (m_fuseCounter < m_fuseTrigger)
                m_state = BombStates.Exploding;
        }
        /// <summary>
        /// Method called when the fuze countdown has finished.
        /// Is in charge of what happens when the bomb is meant to expload.
        /// </summary>
        /// <param name="currentMap"></param>
        /// <param name="gt"></param>
        public void Explode(Map currentMap, GameTime gt)
        {
            m_fuzeInstance.Stop();
            // Call the relevant method in the Map class, where all the heavy lifting happens
            currentMap.BombExplosion(m_position,m_explosionRadius);
            // Change state to "Dead" so that Game1 can delete you
            m_state = BombStates.Dead;

            // Notify that the bomb exploded and increment the player's bomb count
            OnExplode?.Invoke();
        }
        /// <summary>
        /// Main Draw method.
        /// Nothing fancy happening.
        /// If I were do add more polish, this would be one of the things
        /// I would have added:
        /// A variation in texture when the bomb is first placed, perhaps animated.
        /// An animated explosion that was in theme with the rest of the art style.
        /// </summary>
        /// <param name="sb"></param>
        public override void DrawMe(SpriteBatch sb)
        {
            switch (m_state)
            {
                case BombStates.Primed :
                    // Draw Primed Texture
                    base.DrawMe(sb);
                    break;
                case BombStates.Exploding :
                    // Draw The Explosion
                    break;
                default:
                    // Draw The bomb
                    base.DrawMe(sb);
                    break;

            }
        }
    }
}
