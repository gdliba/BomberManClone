

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BomberManClone
{
    class HealthUI : StaticGraphic
    {
        private Texture2D m_emptyTxr;
        private int m_maxHealth;
        private int m_playerHealth;

        public HealthUI(Texture2D txr,Texture2D emptyTxr, Point pos)
            : base(txr, pos)
        {
            m_position = pos;
            m_txr = txr;
            m_emptyTxr = emptyTxr;
            m_maxHealth = 3;
        }
        public void UpdateMe(int playerhealth)
        {
            m_playerHealth = playerhealth;
        }
        public override void DrawMe(SpriteBatch sb)
        {
            int i = 0;
            for (; i < m_playerHealth; i++)
                // Draw the filled faces
                sb.Draw(m_txr, new Vector2(m_position.X, m_position.Y + m_txr.Height * i), Color.White);
            for (; i < m_maxHealth; i++)
            // Draw the empty faces
                sb.Draw(m_emptyTxr, new Vector2(m_position.X, m_position.Y + m_txr.Height * i), Color.White);
        }
    }

    enum StateOfButton
    {
        Neutral,
        Hovered,
        Pressed
    }

    class Button : StaticGraphic
    {
        private Texture2D m_txrPressed;
        private Rectangle m_rect;
        public event Action OnClick; // Event that will notify when Button is clicked
        private SoundEffect m_hoverSfx, m_pressedSfx;
        private bool m_HoverSoundHasPlayed, m_PressedSoundHasPlayed;
        private StateOfButton m_state;


        public Button(Texture2D txr, Texture2D txrPressed, Point pos, SoundEffect hoverSfx, SoundEffect pressedSfx)
            : base(txr, pos)
        {
            m_txrPressed = txrPressed;
            m_txr = txr;
            m_position = pos;
            m_rect = new Rectangle(pos.X, pos.Y, txr.Width, txr.Height);
            m_hoverSfx = hoverSfx;
            m_pressedSfx = pressedSfx;
            m_HoverSoundHasPlayed = false;
            m_PressedSoundHasPlayed = false;
            m_state = StateOfButton.Neutral;
        }
        public void UpdateMe(Point mousepos, MouseState currMouse, MouseState oldMouse)
        {

            switch (m_state)
            {
                case StateOfButton.Neutral:
                    m_HoverSoundHasPlayed = false;
                    DoNeutral(mousepos);
                    break;
                case StateOfButton.Hovered:
                    m_PressedSoundHasPlayed = false;
                    DoHover(mousepos, currMouse, oldMouse);
                    break;
                case StateOfButton.Pressed:
                    DoPressed(mousepos, currMouse, oldMouse);
                    break;
            }
        }
        public void DoNeutral(Point mousepos)
        {
            if (m_rect.Contains(mousepos))
                m_state = StateOfButton.Hovered;
            
        }
        public void DoHover(Point mousepos, MouseState currMouse, MouseState oldMouse)
        {
            if (!m_rect.Contains(mousepos))
                m_state = StateOfButton.Neutral;
            if (!m_HoverSoundHasPlayed)
            {
                m_hoverSfx.Play();
                m_HoverSoundHasPlayed = true;
            }
            if (currMouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                m_state = StateOfButton.Pressed;
        }
        public void DoPressed(Point mousepos, MouseState currMouse, MouseState oldMouse)
        {
            if (!m_PressedSoundHasPlayed)
            {
                m_pressedSfx.Play();
                m_PressedSoundHasPlayed = true;
            }
            if (!m_rect.Contains(mousepos))
                m_state = StateOfButton.Neutral;
            if (currMouse.LeftButton == ButtonState.Released && oldMouse.LeftButton == ButtonState.Pressed)
            {
                OnClick.Invoke();
                m_state = StateOfButton.Hovered;
            }
        }

        public void DrawMe(SpriteBatch sb, string text, SpriteFont font)
        {

            // Set up string measurements to draw text on the Button
            Vector2 textlength = font.MeasureString(text);
            var halfButtonX = m_position.X + m_txr.Width / 2;
            var halfButtonY = m_position.Y + m_txr.Height / 2;

            switch (m_state)
            {
                case StateOfButton.Neutral:
                    sb.Draw(m_txr, new Vector2(m_position.X, m_position.Y), Color.LightGreen);
                    sb.DrawString(font, text, new Vector2(halfButtonX - (int)textlength.X / 2, halfButtonY - (int)textlength.Y / 2), Color.Black);
                    break;
                case StateOfButton.Hovered:
                    sb.Draw(m_txr, new Vector2(m_position.X, m_position.Y), Color.PaleGreen);
                    sb.DrawString(font, text, new Vector2(halfButtonX - (int)textlength.X / 2, halfButtonY - (int)textlength.Y / 2), Color.Black);
                    break;
                case StateOfButton.Pressed:
                    sb.Draw(m_txrPressed, new Vector2(m_position.X, m_position.Y + 5), Color.Orange);
                    sb.DrawString(font, text, new Vector2(halfButtonX - (int)textlength.X / 2, halfButtonY - (int)textlength.Y / 2 + 5), Color.Black);
                    break;
            }
        }
    }
}
