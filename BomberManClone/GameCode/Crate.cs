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
    /// <summary>
    /// This class inherits from StaticGraphic as it doesn't do much more than
    /// draw on screen, however it does include some logic and the "crates" are
    /// a prominent feauture in the game, thus I deem it appropriate to make it
    /// its own class.
    /// </summary>
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
        /// <summary>
        /// Update method that switches states accordingly.
        /// </summary>
        /// <param name="currentMap"></param>
        /// <returns></returns>
        public CrateState UpdateMe(Map currentMap)
        {
            switch (m_state)
            {
                case CrateState.Spawn:
                    // Declare that you are occupying the cell you are on.
                    currentMap.CrateOccupyingCell(m_position);
                    m_state = CrateState.InPlay;
                    break;
                case CrateState.InPlay:
                    InPlay(currentMap);
                    break;
            }
            return m_state;
        }
        /// <summary>
        /// If the cell you are on has been hit by an explosion, attempt to spawn a pickup.
        /// If unsuccessfull, die.
        /// </summary>
        /// <param name="currentMap"></param>
        public void InPlay(Map currentMap)
        {
            if (currentMap.IsCrateBreaking(m_position))
            {
                if (Game1.RNG.NextSingle() < Globals.PickUpSpawnChance)
                    m_state = CrateState.SpawnPickup;
                else
                    m_state = CrateState.Dead;
            }
        }
    }
}
