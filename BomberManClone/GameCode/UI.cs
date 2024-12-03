

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BomberManClone
{
    class HealthUI : StaticGraphic
    {
        private Texture2D m_emptyTxr;
        private int m_maxHealth;

        public HealthUI(Texture2D txr,Texture2D emptyTxr, Point pos)
            : base(txr, pos)
        {
            m_position = pos;
            m_txr = txr;
            m_emptyTxr = emptyTxr;
            m_maxHealth = 3;
        }
        public void DrawMe(SpriteBatch sb, int playerhealth)
        {
            int i = 0;
            for (; i < playerhealth; i++)
                // Draw the filled faces
                sb.Draw(m_txr, new Vector2(m_position.X, m_position.Y + m_txr.Height * i), Color.White);
            for (; i < m_maxHealth; i++)
            // Draw the empty faces
                sb.Draw(m_emptyTxr, new Vector2(m_position.X, m_position.Y + m_txr.Height * i), Color.White);
        }
    }

    class Button : StaticGraphic
    {
        private Texture2D m_txrPressed;
        private Rectangle m_rect;
        public event Action OnClick; // Event that will notify when Button is clicked


        public Button(Texture2D txr, Texture2D txrPressed, Point pos)
            : base(txr, pos)
        {
            m_txrPressed = txrPressed;
            m_txr = txr;
            m_position = pos;
            m_rect = new Rectangle(pos.X, pos.Y, txr.Width, txr.Height);
        }
        public void UpdateMe(Point mousepos, MouseState currMouse, MouseState oldMouse)
        {
            if (m_rect.Contains(mousepos)
                && currMouse.LeftButton == ButtonState.Released && oldMouse.LeftButton == ButtonState.Pressed)
                OnClick?.Invoke();
        }
        public void DrawMe(SpriteBatch sb, Point mousepos, MouseState currMouse, MouseState oldMouse, string text, SpriteFont font)
        {
            // Set up string measurements to draw text on the Button
            Vector2 textlength = font.MeasureString(text);
            var halfButtonX = m_position.X + m_txr.Width / 2;
            var halfButtonY = m_position.Y + m_txr.Height / 2;

            if (m_rect.Contains(mousepos)
                && currMouse.LeftButton == ButtonState.Pressed)
            {
                sb.Draw(m_txrPressed, new Vector2(m_position.X, m_position.Y + 5), Color.LightGray);
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
