using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BomberManClone
{
    enum CrateState
    {
        Spawn,
        InPlay,
        Dead,
        SpawnPickup
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
        public CrateState UpdateMe(Map currentMap)
        {
            switch (m_state)
            {
                case CrateState.Spawn:
                    currentMap.CrateOccupyingCell(m_position);
                    m_state = CrateState.InPlay;
                    break;
                case CrateState.InPlay:
                    InPlay(currentMap);
                    break;
            }
            return m_state;
        }
        public void InPlay(Map currentMap)
        {
            if (currentMap.IsCellExploding(m_position))
            {
                if (Game1.RNG.NextSingle() < Globals.PickUpSpawnChance)
                    m_state = CrateState.SpawnPickup;
                else
                    m_state = CrateState.Dead;
            }
        }
    }
}
