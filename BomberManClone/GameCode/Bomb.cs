
using Microsoft.Xna.Framework;
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
    class Bomb : StaticGraphic
    {
        private float m_fuseCounter;
        private float m_fuseTrigger;
        private int m_explosionRange;

        private BombStates m_state;
        public BombStates State { get { return m_state; } }

        public event Action OnExplode; // Event that will notify when the bomb explodes


        public Bomb(Texture2D txr, Point placedPosition)
            : base (txr, placedPosition)
        {
            m_txr = txr;
            m_position = placedPosition;
            m_state = BombStates.Placed;

            m_fuseCounter = 3;
            m_fuseTrigger = 0;

            m_explosionRange = 3;
        }
        public void UpdateMe(GameTime gt,Map currentMap)
        {
            if (currentMap.IsCellExploding(m_position))
                m_state=BombStates.Exploding;
            switch (m_state)
            {
                case BombStates.Placed:
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
        public void CountDown(GameTime gt)
        {
            m_fuseCounter -=(float)gt.ElapsedGameTime.TotalSeconds;
            if (m_fuseCounter < m_fuseTrigger)
                m_state = BombStates.Exploding;
        }
        public void Explode(Map currentMap, GameTime gt)
        {
            currentMap.RegularBombExplosion(m_position,gt,m_explosionRange);
            m_state = BombStates.Dead;

            // Notify that the bomb exploded and increment the player's bomb count
            OnExplode?.Invoke();
        }
        public override void DrawMe(SpriteBatch sb)
        {
            switch (m_state)
            {
                case BombStates.Primed :
                    // Draw Primed Texture                                  TODO
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
