using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BomberManClone
{
    class Button : StaticGraphic
    {
        private Texture2D m_txrPressed;
        private Rectangle m_rect;

        public Button(Texture2D txr, Texture2D txrPressed, Point pos)
            : base (txr, pos)
        {
            m_txrPressed = txrPressed;
            m_txr = txr;
            m_position = pos;
            m_rect = new Rectangle(pos.X,pos.Y,txr.Width,txr.Height);
        }
        public void DrawMe(SpriteBatch sb, Point mousepos, MouseState currMouse, MouseState oldMouse, string text, SpriteFont font)
        {
            // Set up string measurements to draw text on the Button
            Vector2 textlength = font.MeasureString(text);
            var halfButtonX = m_position.X + m_txr.Width/2;
            var halfButtonY = m_position.Y + m_txr.Height/2;

            if (m_rect.Contains(mousepos)
                && currMouse.LeftButton == ButtonState.Pressed)
            {
                sb.Draw(m_txrPressed, new Vector2(m_position.X, m_position.Y+5), Color.Lavender);
                sb.DrawString(font, text, new Vector2(halfButtonX - (int)textlength.X / 2, halfButtonY - (int)textlength.Y / 2 + 5), Color.Black);
            }
            else
            {
                sb.Draw(m_txr, new Vector2(m_position.X, m_position.Y), Color.Lavender);
                sb.DrawString(font, text, new Vector2(halfButtonX - (int)textlength.X / 2, halfButtonY - (int)textlength.Y / 2), Color.Black);
            }
        }
    }
}
