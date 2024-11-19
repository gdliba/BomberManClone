using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BomberManClone
{
    enum CrateState
    {
        Spawn,
        InPlay,
        Dead
    }
    class Crate : StaticGraphic
    {
        private CrateState m_state;
        public CrateState State { get { return m_state; } }
        public Point Position { get { return m_position; } }

        public Crate(Texture2D txr, Point pos)
            :base (txr, pos)
        {
            m_txr = txr;
            m_position = pos;
            m_state = CrateState.Spawn;
        }
        public int? UpdateMe(Map currentMap)
        {
            switch (m_state)
            {
                case CrateState.Spawn:
                    currentMap.CrateOccupyingCell(m_position);
                    m_state = CrateState.InPlay;
                    return null;
                case CrateState.InPlay:
                    InPlay(currentMap);
                    return null;
                case CrateState.Dead:
                    return Dead();
                default: return 0;

            }
        }
        public void InPlay(Map currentMap)
        {
            if(currentMap.IsCellExploding(m_position))
                m_state = CrateState.Dead;
        }
        public int Dead()
        {
            return Game1.RNG.Next(0,10);
        }
    }
}
