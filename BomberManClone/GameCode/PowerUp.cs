using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BomberManClone
{
    enum PowerUpType
    {
        Speed,
        MoreBombs,
        ExplosionRadius
    }
    enum PowerUpState
    {
        Spawn,
        InPlay,
        Dead
    }
    class PowerUp : StaticGraphic
    {
        private PowerUpType m_type;
        private PowerUpState m_state;

        public PowerUp(Texture2D txr, Point position, PowerUpType type)
            : base(txr, position)
        {
            m_txr = txr;
            m_position = position;
            m_type = type;
            m_state = PowerUpState.Spawn;
        }
        public void UpdateMe(GameTime gt, Map currentMap)
        {
            switch (m_state)
            {
                case PowerUpState.Spawn:
                    currentMap.PowerUpOnCell(m_position);
                    m_state = PowerUpState.InPlay;
                    break;
                case PowerUpState.InPlay:
                    InPlay(gt, currentMap);
                    break;
                case PowerUpState.Dead:
                    break;
            }
        }
        public void InPlay(GameTime gt, Map currentMap)
        {

        }
        public override void DrawMe(SpriteBatch sb)
        {
            base.DrawMe(sb);
        }
    }
}
