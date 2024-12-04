using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace BomberManClone
{
    public enum GameState
    {
        Start,
        InPlay,
        GameOver
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        // Adding Render Target to scale the screen
        private RenderTarget2D _renderTarget;
        // RNG
        public static readonly Random RNG = new Random();

#if DEBUG
        public static SpriteFont debugFont;
#endif

        #region VARIABLES

        // Convenient
        int screenWidth, screenHeight;
        Point screenCentre;


        // Tiles/Map
        List<Texture2D> tiles;
        int[,] testfloor, testfloor2, finishedLevel;
        Map currentMap;

        //Keyboard
        KeyboardState kb_curr, kb_old;

        // Mouse
        Point mousepos;
        MouseState currMouse, oldMouse;

        //Players
        PC player1;
        Texture2D toombstoneTxr;

        //Bomb
        List<Bomb> bombs;

        // PowerUps
        List<PowerUp> powerUps;
        Texture2D speedPUText, extraBombPUText, explosionRadiusPUText, randomPUText; 

        // Crates
        List<Crate> crates;
        List<Point> crateSpawnPoints;

        //GameState
        GameState currentGameState;

        // Buttons
        List<Button> buttons;
        Texture2D buttonTxr, buttonTxrPressed;
        Point startButtonPos, exitButtonPos;

        // Health UI
        Texture2D healthTxr, healthTxrEmpty;
        List<HealthUI> healthDisplay;
        Point player1HealthDisplay, player2HealthDisplay, player3HealthDisplay, player4HealthDisplay;

        // Sounds
        SoundEffect buttonHoverSfx, buttonPressedSfx, maximiseSfx, minimiseSfx, footstepSfx, PUspawnSfx, deathSfx;
        SoundEffectInstance buttonHoverInstance, buttonPressedInstance, maximiseInstance, minimiseInstance;

        #endregion



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Window size and window title settings
            Window.Title = "Supermarket Reloaded";
            _graphics.PreferredBackBufferWidth = 1088;
            _graphics.PreferredBackBufferHeight = 1088;

            screenWidth = _graphics.PreferredBackBufferWidth;
            screenHeight = _graphics.PreferredBackBufferHeight;
            screenCentre = new Point(screenWidth/2, screenHeight/2);
        }

        protected override void Initialize()
        {
            // List initialisations
            tiles = new List<Texture2D>();
            bombs = new List<Bomb>();
            powerUps = new List<PowerUp>();
            crates = new List<Crate>();
            crateSpawnPoints = new List<Point>();
            buttons = new List<Button>();
            healthDisplay = new List<HealthUI>();

            // Setting points
            player1HealthDisplay = new Point(0, 0); // top left
            player2HealthDisplay = new Point(_graphics.PreferredBackBufferWidth-64, 12*64); // bottom right
            player3HealthDisplay = new Point(_graphics.PreferredBackBufferWidth-64, 0); // top right
            player4HealthDisplay = new Point(0, 12*64);

            // Button Positions
            startButtonPos = new Point(screenCentre.X - 80, screenCentre.Y - 22);
            exitButtonPos = new Point(startButtonPos.X, screenCentre.Y + 45);

#if DEBUG
            debugFont = Content.Load<SpriteFont>("Fonts\\Arial07");
#endif
            currentGameState = GameState.Start;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _renderTarget = new RenderTarget2D(_spriteBatch.GraphicsDevice, 1088, 1088);


            // Bombs
            //bombs.Add(new Bomb (Content.Load<Texture2D>("Objects\\plate"), new Point(4, 1)));

            // PowerUps
            speedPUText = Content.Load<Texture2D>("Objects\\powerup_01");
            extraBombPUText = Content.Load<Texture2D>("Objects\\powerup_02");
            explosionRadiusPUText = Content.Load<Texture2D>("Objects\\powerup_03");
            // powerUps.Add(new PowerUp(explosionRadiusPUText, new Point(4, 5), PowerUpType.Speed));

            // Crates
            // crates.Add(new Crate(Content.Load<Texture2D>("Objects\\crate_01"), new Point(4, 1)));


            // Health UI
            healthTxr = Content.Load<Texture2D>("UI\\playerFace");
            healthTxrEmpty = Content.Load<Texture2D>("UI\\playerFace_empty");
            healthDisplay.Add(new HealthUI(healthTxr, healthTxrEmpty, player1HealthDisplay));   // player 1
            healthDisplay.Add(new HealthUI(healthTxr, healthTxrEmpty, player2HealthDisplay));   // player 2
            healthDisplay.Add(new HealthUI(healthTxr, healthTxrEmpty, player3HealthDisplay));   // player 3 
            healthDisplay.Add(new HealthUI(healthTxr, healthTxrEmpty, player4HealthDisplay));   // player 4 

            // SoundEffects
            buttonHoverSfx = Content.Load<SoundEffect>("Sounds\\UI1");
            buttonPressedSfx = Content.Load<SoundEffect>("Sounds\\UI2");
            maximiseSfx = Content.Load<SoundEffect>("Sounds\\maximise");
            minimiseSfx = Content.Load<SoundEffect>("Sounds\\minimise");
            footstepSfx = Content.Load<SoundEffect>("Sounds\\footstep");
            PUspawnSfx = Content.Load<SoundEffect>("Sounds\\PUSpawn");
            deathSfx = Content.Load<SoundEffect>("Sounds\\death");



            buttonHoverInstance = buttonHoverSfx.CreateInstance();
            buttonPressedInstance = buttonPressedSfx.CreateInstance();
            maximiseInstance = maximiseSfx.CreateInstance();
            minimiseInstance = minimiseSfx.CreateInstance();

            // Players
            toombstoneTxr = Content.Load<Texture2D>("Objects\\toombstone");
            player1 = new PC(new Point(2, 1), Content.Load<Texture2D>("Characters\\player_spritesheetHighlighted"), 3, 4, footstepSfx, toombstoneTxr, deathSfx);

            //Buttons
            buttonTxr = Content.Load<Texture2D>("UI\\button");
            buttonTxrPressed = Content.Load<Texture2D>("UI\\button_pressed");

            buttons.Add(new Button(buttonTxr, buttonTxrPressed, startButtonPos, buttonHoverSfx, maximiseSfx));
            buttons.Add(new Button(buttonTxr, buttonTxrPressed, exitButtonPos, buttonHoverSfx, minimiseSfx));




            // Setting up Tile Textures
            tiles.Add(Content.Load<Texture2D>("Tiles\\occupiedCell"));  // 0
            tiles.Add(Content.Load<Texture2D>("Tiles\\ground_01")); // 1
            tiles.Add(Content.Load<Texture2D>("Tiles\\wall_03"));  // 2
            tiles.Add(Content.Load<Texture2D>("Tiles\\wall_04")); // 3
            tiles.Add(Content.Load<Texture2D>("Tiles\\wall_04")); // 4
            tiles.Add(Content.Load<Texture2D>("Tiles\\wall_04"));// 5
            tiles.Add(Content.Load<Texture2D>("Tiles\\wall_04"));// 6

            tiles.Add(Content.Load<Texture2D>("Tiles\\void"));  // 7
            tiles.Add(Content.Load<Texture2D>("Tiles\\wall_04"));  // 8
            tiles.Add(Content.Load<Texture2D>("Tiles\\ground_01"));  // 9
            tiles.Add(Content.Load<Texture2D>("Tiles\\occupiedCell"));  // 10
            tiles.Add(Content.Load<Texture2D>("Tiles\\empty"));  // 11
            tiles.Add(Content.Load<Texture2D>("Tiles\\ground_01")); // 12
            tiles.Add(Content.Load<Texture2D>("Tiles\\void"));  // 13





            // Tile Layout
            testfloor = new int[17, 17]
            {
                { 5, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 6},
                { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
                { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4},
                { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
                { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4},
                { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
                { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4},
                { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
                { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4},
                { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
                { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4},
                { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
                { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1 ,2, 1, 4},
                { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 ,1, 1, 4},
                { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1 ,2, 1, 4},
                { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 ,1, 1, 4},
                { 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8}

            };
            finishedLevel = new int[17, 17]
            {
                {11, 5, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 6, 11},
                {11, 3, 12, 12, 1, 1, 1, 1, 1, 1, 1, 1, 1, 12, 12, 4, 11},
                {11, 3, 12, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 12, 4, 11},
                {11, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 11},
                {11, 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4, 11},
                {11, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 11},
                {11, 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4, 11},
                {11, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 11},
                {11, 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4, 11},
                {11, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 11},
                {11, 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4, 11},
                {11, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 11},
                {11, 3, 12, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 12, 4, 11},
                {11, 3, 12, 12, 1, 1, 1, 1, 1, 1, 1, 1, 1, 12, 12, 4, 11},
                {11, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 11},
                {11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11},
                {11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11}
                
            };
            testfloor2 = new int[15, 15]
            {
                { 5, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 6},
                { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
                { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4},
                { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
                { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4},
                { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
                { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4},
                { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
                { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4},
                { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
                { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4},
                { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
                { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1 ,2, 1, 4},
                { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 ,1, 1, 4},
                { 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8}

            };

            // Use this Map
            currentMap = new Map(finishedLevel);
            currentMap.SpawnCrates(Content.Load<Texture2D>("Objects\\crate_01"), crates);
        }
        public bool PlaceBomb(Point pos, GameTime gt, int explosionRadius)
        {
            Bomb newBomb = new Bomb(Content.Load<Texture2D>("Objects\\plate"), pos, explosionRadius);
            bombs.Add(newBomb);

            // Subscribe to the OnExplode event
            newBomb.OnExplode += player1.IncreaseBombCount;

            // Iterate bombs to check for explosions and remove dead bombs
            for (int i = 0; i < bombs.Count; i++)
            {
                bombs[i].UpdateMe(gt, currentMap);
                if (bombs[i].State == BombStates.Dead)
                {
                    bombs.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        public void SpawnPowerUp(Point pos, GameTime gt)
        {
            var randomPowerup = RNG.Next(0, 3);
            switch (randomPowerup)
            {
                case 0:
                    randomPUText = speedPUText;
                    break;
                case 1:
                    randomPUText = extraBombPUText;
                    break;
                case 2:
                    randomPUText = explosionRadiusPUText;
                    break;
            }
            PowerUp newPowerUp = new PowerUp(randomPUText, pos, (PowerUpType) randomPowerup, PUspawnSfx);
            powerUps.Add(newPowerUp);
        }
        public void ChangeState(GameState state)
        {
            currentGameState = state;

        }

        protected override void Update(GameTime gt)
        {
            kb_curr = Keyboard.GetState();
            //Track Mouse Position
            mousepos = Mouse.GetState().Position;
            currMouse = Mouse.GetState();

            switch (currentGameState)
            {
                case GameState.Start:
                    StartUpdate();
                    break;
                case GameState.InPlay:
                    InPlayUpdate(gt);
                    break;
                case GameState.GameOver:
                    GameOverUpdate();
                    break;
            }

            // close game
            if (kb_curr.IsKeyDown(Keys.Escape))
                this.Exit();
            // Test for player reset method //////////////////////////
            if (kb_curr.IsKeyDown(Keys.Up))
                player1.Reset();

                base.Update(gt);
            kb_old = kb_curr;
            oldMouse = currMouse;
        }


        protected override void Draw(GameTime gt)
        {
            // Applying scale effect to the screen
            GraphicsDevice.SetRenderTarget(_renderTarget);


            GraphicsDevice.Clear(Color.Tan);

            _spriteBatch.Begin();

            switch (currentGameState)
            {
                case GameState.Start:
                    StartDraw(gt);
                    break;
                case GameState.InPlay:
                    InPlayDraw(gt);
                    break;
                case GameState.GameOver:
                    GameOverDraw(gt);
                    break;
            }

            _spriteBatch.End();
            #region Screen Scaling
            // Completing Scale effect on the screen
            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_renderTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            _spriteBatch.End();
            #endregion
            base.Draw(gt);
        }
        #region STATE UPDATES
        void StartUpdate()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[0].UpdateMe(mousepos, currMouse, oldMouse); // Start Button
                //buttons[0].OnClick += () => ChangeStateToPlay();
                //buttons[0].OnClick += (_, _) => ChangeState(GameState.InPlay);
                buttons[0].OnClick += delegate { ChangeState(GameState.InPlay); };
                buttons[1].UpdateMe(mousepos, currMouse, oldMouse); // Exit Button
                buttons[1].OnClick += () => Exit();
            }

        }
        void InPlayUpdate(GameTime gt)
        {
            #region Update States
            // Player
            var canPlaceBomb = player1.UpdateMe(gt, currentMap, kb_curr, kb_old);
            if (canPlaceBomb)
            {
                PlaceBomb(player1.Position.ToPoint(), gt, player1.ExplosionRadius);
            }
            // Bombs
            for (int i = 0; i < bombs.Count; i++)
            {
                bombs[i].UpdateMe(gt, currentMap);
                if (bombs[i].State == BombStates.Dead)
                    bombs.RemoveAt(i);
            }

            // Map/Tiles
            currentMap.Update(gt);

            // PowerUps
            for (int i = 0; i < powerUps.Count; i++)
            {
                powerUps[i].UpdateMe(gt, currentMap);
                if (powerUps[i].State == PowerUpState.Dead)
                    powerUps.RemoveAt(i);


            }

            //////////////////// Player - PowerUps Interraction //////////////////// 

            if (currentMap.IsPowerUpOnCell(player1.Position.ToPoint()))
            {
                for (int i = 0; i < powerUps.Count; i++)
                {
                    if (player1.State==PlayerState.InPlay)
                    {

                        if (powerUps[i].Position == player1.Position.ToPoint())
                        {
                            maximiseSfx.Play();
                            switch (powerUps[i].Type)
                            {
                                case PowerUpType.Speed:
                                    player1.SpeedPowerUp();
                                    break;
                                case PowerUpType.MoreBombs:
                                    player1.MoreBombsPowerUp();
                                    break;
                                case PowerUpType.ExplosionRadius:
                                    player1.ExplosionRadiusPowerUp();
                                    break;
                            }
                            powerUps.RemoveAt(i);
                        }
                    }

                }
            }



            // Crates
            for (int i = 0; i < crates.Count; i++)
            {
                crates[i].UpdateMe(currentMap);
                var spawnPowerUp = crates[i].UpdateMe(currentMap);
                if (spawnPowerUp == CrateState.SpawnPickup)
                    SpawnPowerUp(crates[i].Position, gt);
                if (crates[i].State == CrateState.Dead || crates[i].State == CrateState.SpawnPickup)
                {
                    crates.RemoveAt(i);
                }
            }
            #endregion
        }
        void GameOverUpdate()
        {

        }
        #endregion
        #region STATE DRAWS
        void StartDraw(GameTime gt)
        {
            // Map
            currentMap.DrawMe(_spriteBatch, tiles);

            // Buttons
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[0].DrawMe(_spriteBatch, "Start", debugFont);
                buttons[1].DrawMe(_spriteBatch, "Exit", debugFont);
            }
        }
        void InPlayDraw(GameTime gt)
        {
            // Map
            currentMap.DrawMe(_spriteBatch, tiles);
            // Player
            player1.DrawMe(_spriteBatch, gt, tiles[0].Width, tiles[1].Height, Color.White);

            // Bombs
            for (int i = 0; i < bombs.Count; i++)
            {
                bombs[i].DrawMe(_spriteBatch);
            }

            // PowerUps
            for (int i = 0; i < powerUps.Count; i++)
            {
                powerUps[i].DrawMe(_spriteBatch);
            }

            // Crates
            foreach (var crate in crates)
            {
                crate.DrawMe(_spriteBatch);
            }

            // Health UI
            foreach (var health in healthDisplay)
            {
                health.DrawMe(_spriteBatch, player1.Shields);
            }

#if DEBUG
            _spriteBatch.DrawString(debugFont, _graphics.PreferredBackBufferWidth + "x " + _graphics.PreferredBackBufferHeight
                + "\nfps: " + (int)(1 / gt.ElapsedGameTime.TotalSeconds) + "ish" +
                "\nplayer1 Shields: " + player1.Shields,
                new Vector2(10, 10), Color.Black);
#endif
           
        }
        void GameOverDraw(GameTime gt)
        {

        }
        #endregion
    }
}
