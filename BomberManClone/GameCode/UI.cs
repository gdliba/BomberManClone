using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BomberManClone
{
    /// <summary>
    /// The HealthUI class inherits from the StaticGraphic as I need it to
    /// draw on screen similarly to the parent.
    /// Given that I need more complex logic in this particular case, 
    /// I've made it it's own class.
    /// </summary>
    class HealthUI : StaticGraphic
    {
        private Texture2D m_emptyTxr;
        private int m_maxHealth;
        private int m_playerHealth;

        /// <summary>
        /// The constructor takes in 2 textures and the position it should be drawn in.
        /// The positions change based on which player it references.
        /// </summary>
        /// <param name="txr"></param>
        /// <param name="emptyTxr"></param>
        /// <param name="pos"></param>
        public HealthUI(Texture2D txr,Texture2D emptyTxr, Point pos)
            : base(txr, pos)
        {
            m_position = pos;
            m_txr = txr;
            m_emptyTxr = emptyTxr;
            m_maxHealth = 3;
        }
        /// <summary>
        /// Nothing complex in the update method, I just need to
        /// take in the player's health at all times so that the UI can
        /// draw accordingly as that is most of what the UI does.
        /// </summary>
        /// <param name="playerhealth"></param>
        public void UpdateMe(int playerhealth)
        {
            m_playerHealth = playerhealth;
        }
        /// <summary>
        /// The draw method is where the bulk of the work goes on.
        /// It is checking the player health and drawing a "full face" for every life
        /// the player has. Otherwise it should draw an "empty face".
        /// </summary>
        /// <param name="sb"></param>
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
    class PowerUpUI : StaticGraphic
    {
        private int m_numberOfSpeedPickups, m_numberOfExplosionPickups, m_numberOfBombIncreasePickups;
        private Texture2D m_speedTxr, m_explosionRadiusTxr, m_numberOfBombsTxr;
        public PowerUpUI(Texture2D speedTxr, Texture2D explosionRadiusTxr, Texture2D numberOfBombsTxr, Point pos) :
            base(speedTxr, pos)
        {
            m_position = pos;
            m_speedTxr = speedTxr;
            m_explosionRadiusTxr = explosionRadiusTxr;
            m_numberOfBombsTxr = numberOfBombsTxr;

        }
        public void UpdateMe(int numberOfSpeedPickups, int numberOfExplosionRadiusPickups, int numberOfBombIncreasePickups)
        {
            m_numberOfSpeedPickups = numberOfSpeedPickups;
            m_numberOfExplosionPickups = numberOfExplosionRadiusPickups;
            m_numberOfBombIncreasePickups = numberOfBombIncreasePickups;
        }
        public void DrawMe(SpriteBatch sb, SpriteFont font)
        {
            int Ycorrection = 28;
            int Xcorrection = 10;
            // Speed
            sb.Draw(m_speedTxr, new Vector2(m_position.X, m_position.Y), Color.White);
            sb.DrawString(font, m_numberOfSpeedPickups.ToString(), new Vector2(m_position.X + m_speedTxr.Width - Xcorrection,
                m_position.Y + Ycorrection), Color.White);

            //Radius
            sb.Draw(m_explosionRadiusTxr, new Vector2(m_position.X, m_position.Y + m_speedTxr.Height), Color.White);
            sb.DrawString(font, m_numberOfExplosionPickups.ToString(), new Vector2(m_position.X + m_speedTxr.Width - Xcorrection,
                m_position.Y + m_speedTxr.Height + Ycorrection), Color.White);


            //Bomb Increase
            sb.Draw(m_numberOfBombsTxr, new Vector2(m_position.X, m_position.Y + 2*(m_speedTxr.Height)), Color.White);
            sb.DrawString(font, m_numberOfBombIncreasePickups.ToString(), new Vector2(m_position.X + m_speedTxr.Width - Xcorrection,
                m_position.Y + 2*(m_speedTxr.Height) + Ycorrection), Color.White);
        }
    }

    enum StateOfButton
    {
        Neutral,
        Hovered,
        Pressed
    }
    /// <summary>
    /// The Button class is more complicated than the HealthUI.
    /// It also inherits from StaticGraphic as it needs to draw in
    /// a similar manner.
    /// However it has a lot more bells and whistles to multiple 
    /// parts of it's class -- adding functionality to the actual buttons
    /// in a very modular way, required it's on class.
    /// </summary>
    class Button : StaticGraphic
    {
        private Texture2D m_txrPressed;
        private Rectangle m_rect;
        public event Action OnClick; // Event that will notify when Button is clicked
        private SoundEffect m_hoverSfx, m_pressedSfx;
        private bool m_HoverSoundHasPlayed, m_PressedSoundHasPlayed;
        private StateOfButton m_state;
        private string m_text;
        private SpriteFont m_font;

        /// <summary>
        /// The constructor takes in the two textures, and two relevant soundEffects, font,
        /// dispolayed text and finally its position.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="txr"></param>
        /// <param name="txrPressed"></param>
        /// <param name="pos"></param>
        /// <param name="hoverSfx"></param>
        /// <param name="pressedSfx"></param>
        public Button(string text, SpriteFont font, Texture2D txr, Texture2D txrPressed, Point pos, SoundEffect hoverSfx, SoundEffect pressedSfx)
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
            m_text = text;
            m_font = font;
        }
        /// <summary>
        /// The main update method switches states to act accordingly to input.
        /// </summary>
        /// <param name="mousepos"></param>
        /// <param name="currMouse"></param>
        /// <param name="oldMouse"></param>
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
        /// <summary>
        /// Check if the mouse is on the button and change state to
        /// "Hovered" if that is the case.
        /// </summary>
        /// <param name="mousepos"></param>
        public void DoNeutral(Point mousepos)
        {
            if (m_rect.Contains(mousepos))
                m_state = StateOfButton.Hovered;
        }
        /// <summary>
        /// If the mouse leaves the button, change back to neutral.
        /// Play the "hover" sound, if it hasn't already played.
        /// Change state to "pressed" when clicked on.
        /// </summary>
        /// <param name="mousepos"></param>
        /// <param name="currMouse"></param>
        /// <param name="oldMouse"></param>
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
        /// <summary>
        /// Play "pressed" sound effect if it hasn't already played.
        /// If mouse leaves the confines of the button, change state back to "neutral"
        /// Once player releases the click, Invoke the "OnClick" action.
        /// </summary>
        /// <param name="mousepos"></param>
        /// <param name="currMouse"></param>
        /// <param name="oldMouse"></param>
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

        /// <summary>
        /// The draw method makes slight tweaks to the colour/position of the button.
        /// It also uses the given Font and String given to it in the constructor
        /// to measure the String and centre it.
        /// The downside to this, currently, is that if a very long string were to
        /// be added to the button, then it would exceed the confines of the button,
        /// rather than word wrap.
        /// </summary>
        /// <param name="sb"></param>
        public override void DrawMe(SpriteBatch sb)
        {
            // Set up string measurements to draw text on the Button
            Vector2 textlength = m_font.MeasureString(m_text);
            var halfButtonX = m_position.X + m_txr.Width / 2;
            var halfButtonY = m_position.Y + m_txr.Height / 2;

            switch (m_state)
            {
                case StateOfButton.Neutral:
                    sb.Draw(m_txr, new Vector2(m_position.X, m_position.Y), Color.LightGreen);
                    sb.DrawString(m_font, m_text, new Vector2(halfButtonX - (int)textlength.X / 2, halfButtonY - (int)textlength.Y / 2), Color.Black);
                    break;
                case StateOfButton.Hovered:
                    sb.Draw(m_txr, new Vector2(m_position.X, m_position.Y), Color.PaleGreen);
                    sb.DrawString(m_font, m_text, new Vector2(halfButtonX - (int)textlength.X / 2, halfButtonY - (int)textlength.Y / 2), Color.Black);
                    break;
                case StateOfButton.Pressed:
                    sb.Draw(m_txrPressed, new Vector2(m_position.X, m_position.Y + 5), Color.Orange);
                    sb.DrawString(m_font, m_text, new Vector2(halfButtonX - (int)textlength.X / 2, halfButtonY - (int)textlength.Y / 2 + 5), Color.Black);
                    break;
            }
        }
    }
}
