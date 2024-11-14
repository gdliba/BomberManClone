using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BomberManClone
{
    enum PowerUpType
    {
        one,
        two,
        three,
        dead
    }
    class PowerUp : StaticGraphic
    {
        private PowerUpType m_type;

        public PowerUp(Texture2D txr, Point position)
            : base(txr, position)
        {
            m_txr = txr;
            m_position = position;
            m_type = PowerUpType.one;
        }
        public void UpdateMe(GameTime gt)
        {

        }
        public override void DrawMe(SpriteBatch sb)
        {
            base.DrawMe(sb);
        }
    }
}
