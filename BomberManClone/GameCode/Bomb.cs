
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        private BombStates m_state;
        public BombStates State { get { return m_state; } }

        public Bomb(Texture2D txr, Point placedPosition)
            : base (txr, placedPosition)
        {
            m_txr = txr;
            m_position = placedPosition;
            m_state = BombStates.Placed;

            m_fuseCounter = 3;
            m_fuseTrigger = 0;
        }
        public void UpdateMe(GameTime gt,Map currentMap)
        {
            currentMap.SetCellToBomb(m_position);

            switch (m_state)
            {
                case BombStates.Placed:
                    m_state = BombStates.Primed;
                    break;
                case BombStates.Primed:
                    CountDown(gt, currentMap);
                    break;
                case BombStates.Exploding:
                    Explode(currentMap);
                    break;
            }
        }
        public void CountDown(GameTime gt, Map currentMap)
        {
            m_fuseCounter -=(float)gt.ElapsedGameTime.TotalSeconds;
            if (m_fuseCounter < m_fuseTrigger)
                m_state = BombStates.Exploding;
        }
        public void Explode(Map currentMap)
        {
            currentMap.RegularBombExplosion(m_position);
            m_state = BombStates.Dead;
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
