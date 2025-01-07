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
        // Font
        SpriteFont UIfont, bigUIfont;

        // Tiles/Map
        List<Texture2D> tiles;
        int[,] finishedLevel;
        Map currentMap;

        // Keyboard
        KeyboardState kb_curr, kb_old;

        // Mouse
        Point mousepos;
        MouseState currMouse, oldMouse;

        //Players
        List<PC> players;
        Texture2D toombstoneTxr, playerTxr;

        //Bomb
        List<Bomb> bombs;

        // PowerUps
        List<PowerUp> powerUps;

        // Crates
        List<Crate> crates;

        // GameState
        GameState currentGameState;

        // Buttons
        Dictionary<string, Button> buttons;
        Texture2D buttonTxr, buttonTxrPressed;

        // Health UI
        Texture2D healthTxr, healthTxrEmpty;
        List<HealthUI> healthDisplay;

        //PowerUpUI
        List<PowerUpUI> powerUpUIs;

        // Sounds
        Dictionary<string, SoundEffect> soundEffects;

        // GamePadStates
        List<GamePadState> gamePadStates;
        List<GamePadState> oldGamePadStates;
        int numberOfPlayers;

        // Gameover screen overlay
        StaticGraphic gameOverScreenTint;

        // Timers and Countdowns
        float gameOverCounter, gameOverCounterReset;
        #endregion

        public Game1()
        {
            Globals.Graphics = _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Window size and window title settings
            Window.Title = "Supermarket Reloaded";
            Globals.ChangeResolution(1088, 1088);
            Window.AllowUserResizing = true;
        }
        protected override void Initialize()
        {
            #region List and Dictionary Intitialisations
            // List initialisations
            tiles = new List<Texture2D>();
            bombs = new List<Bomb>();
            powerUps = new List<PowerUp>();
            crates = new List<Crate>();
            soundEffects = new Dictionary<string, SoundEffect>();
            buttons = new Dictionary<string, Button>();
            healthDisplay = new List<HealthUI>();
            powerUpUIs = new List<PowerUpUI>();
            gamePadStates = new List<GamePadState>();
            oldGamePadStates = new List<GamePadState>();
            players = new List<PC>();
            #endregion
            // GameState
            currentGameState = GameState.Start;
            //Number of Players
            numberOfPlayers = 2;
            // Timers and Countdowns
            gameOverCounter = 3;
            gameOverCounterReset = 3;

            Globals.Initialise();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _renderTarget = new RenderTarget2D(_spriteBatch.GraphicsDevice, 1088, 1088);

            // Font
#if DEBUG
            debugFont = Content.Load<SpriteFont>("Fonts\\Arial07");
#endif
            UIfont = Content.Load<SpriteFont>("Fonts\\UIfont");
            bigUIfont = Content.Load<SpriteFont>("Fonts\\BigUIfont");

            // Health UI
            healthTxr = Content.Load<Texture2D>("UI\\playerFace");
            healthTxrEmpty = Content.Load<Texture2D>("UI\\playerFace_empty");
            for (int i = 0; i < Globals.HealthDisplayPoints.Count; i++)
            {
                healthDisplay.Add(new HealthUI(healthTxr, healthTxrEmpty, Globals.HealthDisplayPoints[i]));
                powerUpUIs.Add(new PowerUpUI(Content.Load<Texture2D>("Objects\\powerup_01"), Content.Load<Texture2D>("Objects\\powerup_03"), Content.Load<Texture2D>("Objects\\powerup_02"), Globals.PowerUpDisplayPoints[i]));


            };

            #region SoundEffects
            // SoundEffects
            var buttonHoverSfx = Content.Load<SoundEffect>("Sounds\\UI1");
            soundEffects.Add("buttonHover", buttonHoverSfx);
            var buttonPressedSfx = Content.Load<SoundEffect>("Sounds\\UI2");
            soundEffects.Add("buttonPressed", buttonPressedSfx);
            var maximiseSfx = Content.Load<SoundEffect>("Sounds\\maximise");
            soundEffects.Add("maximise", maximiseSfx);
            var minimiseSfx = Content.Load<SoundEffect>("Sounds\\minimise");
            soundEffects.Add("minimise", minimiseSfx);
            var footstepSfx = Content.Load<SoundEffect>("Sounds\\footstep");
            soundEffects.Add("footstep", footstepSfx);
            var PUspawnSfx = Content.Load<SoundEffect>("Sounds\\PUSpawn");
            soundEffects.Add("powerupSpawn", PUspawnSfx);
            var deathSfx = Content.Load<SoundEffect>("Sounds\\death");
            soundEffects.Add("death", deathSfx);
            var fuzeSfx = Content.Load<SoundEffect>("Sounds\\sizzle");
            soundEffects.Add("fuze", fuzeSfx);
            #endregion

            // Players
            toombstoneTxr = Content.Load<Texture2D>("Objects\\toombstone");
            playerTxr = Content.Load<Texture2D>("Characters\\player_spritesheetHighlighted");

            #region Buttons
            //Buttons
            buttonTxr = Content.Load<Texture2D>("UI\\button");
            buttonTxrPressed = Content.Load<Texture2D>("UI\\button_pressed");

            var startButton = new Button("Start", UIfont,buttonTxr, buttonTxrPressed, Globals.ButtonPositions["startButton"], buttonHoverSfx, maximiseSfx);
            startButton.OnClick += delegate 
            {
                gamePadStates.Clear();
                players.Clear();
                oldGamePadStates.Clear();
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    players.Add(new PC(Globals.PlayerSpawnPoints[i], playerTxr, 3, 4, footstepSfx, toombstoneTxr, deathSfx, Globals.PlayerTints[i]));
                    gamePadStates.Add(new GamePadState());
                    oldGamePadStates.Add(new GamePadState());
                }
                ChangeState(GameState.InPlay); 
            };
            buttons.Add("Start", startButton);

            var exitButton = new Button("Exit", UIfont, buttonTxr, buttonTxrPressed, Globals.ButtonPositions["exitButton"], buttonHoverSfx, minimiseSfx);
            exitButton.OnClick += ()=> Exit();
            buttons.Add("Exit", exitButton);

            var twoPlayersButton = new Button("2 Players", UIfont, buttonTxr, buttonTxrPressed, Globals.ButtonPositions["twoPlayersButton"], buttonHoverSfx, maximiseSfx);
            twoPlayersButton.OnClick += () => numberOfPlayers = 2;
            buttons.Add("2 Players", twoPlayersButton);

            var threePlayersButton = new Button("3 Players", UIfont, buttonTxr, buttonTxrPressed, Globals.ButtonPositions["threePlayersButton"], buttonHoverSfx, maximiseSfx);
            threePlayersButton.OnClick += () => numberOfPlayers = 3;
            buttons.Add("3 Players", threePlayersButton);

            var fourPlayersButton = new Button("4 Players", UIfont, buttonTxr, buttonTxrPressed, Globals.ButtonPositions["fourPlayersButton"], buttonHoverSfx, maximiseSfx);
            fourPlayersButton.OnClick += () => numberOfPlayers = 4;
            buttons.Add("4 Players", fourPlayersButton);

            var restartButton = new Button("Restart", UIfont, buttonTxr, buttonTxrPressed, Globals.ButtonPositions["startButton"], buttonHoverSfx, maximiseSfx);
            restartButton.OnClick += delegate
            {
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    players[i].Reset();
                }
                RestartGame();
            };
            buttons.Add("Restart", restartButton);

            var mainMenuButton = new Button("Main Menu", UIfont, buttonTxr, buttonTxrPressed, Globals.ButtonPositions["exitButton"], buttonHoverSfx, minimiseSfx);
            mainMenuButton.OnClick += delegate
            { 
                ChangeState(GameState.Start);
            };
            buttons.Add("Main Menu", mainMenuButton);
            #endregion

            // game over screen tint
            gameOverScreenTint = new StaticGraphic(Content.Load<Texture2D>("pixel"), new Point(0,0));

            // Setting up Tile Textures
            tiles.Add(Content.Load<Texture2D>("Tiles\\ground_01"));     // 0
            tiles.Add(Content.Load<Texture2D>("Tiles\\ground_01"));     // 1
            tiles.Add(Content.Load<Texture2D>("Tiles\\wall_03"));       // 2
            tiles.Add(Content.Load<Texture2D>("Tiles\\wall_04"));       // 3
            tiles.Add(Content.Load<Texture2D>("Tiles\\wall_04"));       // 4
            tiles.Add(Content.Load<Texture2D>("Tiles\\wall_04"));       // 5
            tiles.Add(Content.Load<Texture2D>("Tiles\\wall_04"));       // 6

            tiles.Add(Content.Load<Texture2D>("Tiles\\ground_01"));     // 7
            tiles.Add(Content.Load<Texture2D>("Tiles\\wall_04"));       // 8
            tiles.Add(Content.Load<Texture2D>("Tiles\\ground_01"));     // 9
            tiles.Add(Content.Load<Texture2D>("Tiles\\occupiedCell"));  // 10
            tiles.Add(Content.Load<Texture2D>("Tiles\\empty"));         // 11
            tiles.Add(Content.Load<Texture2D>("Tiles\\ground_01"));     // 12
            tiles.Add(Content.Load<Texture2D>("Tiles\\ground_01"));     // 13
            tiles.Add(Content.Load<Texture2D>("Tiles\\ground_01"));     // 14

            // Tile Layout
            // This is an unused Map. I kept is as a template, as it is easier to use than
            // the next one.
            //testfloor = new int[17, 17]
            //{
            //    { 5, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 6},
            //    { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
            //    { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4},
            //    { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
            //    { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4},
            //    { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
            //    { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4},
            //    { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
            //    { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4},
            //    { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
            //    { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4},
            //    { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4},
            //    { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1 ,2, 1, 4},
            //    { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 ,1, 1, 4},
            //    { 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1 ,2, 1, 4},
            //    { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 ,1, 1, 4},
            //    { 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8}

            //};
            finishedLevel = new int[17, 17]
            {
                {11, 5, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 6, 11},
                {11, 3, 12, 12, 1, 1, 1, 1, 1, 1, 1, 1, 1, 12, 12, 4, 11},
                {11, 3, 12, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 12, 4, 11},
                {11, 3, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 1, 4, 11},
                {11, 3, 1, 2, 1, 2, 1, 2, 2, 2, 1, 2, 1, 2, 1, 4, 11},
                {11, 3, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 4, 11},
                {11, 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4, 11},
                {11, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 4, 11},
                {11, 3, 1, 2, 1, 2, 1, 2, 2, 2, 1, 2, 1, 2, 1, 4, 11},
                {11, 3, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 4, 11},
                {11, 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 4, 11},
                {11, 3, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 1, 4, 11},
                {11, 3, 12, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 12, 4, 11},
                {11, 3, 12, 12, 1, 1, 1, 1, 1, 1, 1, 1, 1, 12, 12, 4, 11},
                {11, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 11},
                {11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11},
                {11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11}
                
            };
            // Use this Map
            currentMap = new Map(finishedLevel);
        }
        /// <summary>
        /// Method in charge of placing a bomg when the player is allowed to.
        /// The method is also used to let the player know that the bomb they placed has exploaded
        /// and that they can now place another.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="gt"></param>
        /// <param name="explosionRadius"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool PlaceBomb(Point pos, GameTime gt, int explosionRadius, int i)
        {
            var newBomb = new Bomb(Content.Load<Texture2D>("Objects\\sodaBomb"), pos, explosionRadius, soundEffects["fuze"]);
            bombs.Add(newBomb);

            // Subscribe to the OnExplode event
            newBomb.OnExplode += players[i].IncreaseBombCount;

            // Iterate bombs to check for explosions and remove dead bombs
            for (int j = 0; j < bombs.Count; j++)
            {
                bombs[j].UpdateMe(gt, currentMap);
                if (bombs[j].State == BombStates.Dead)
                {
                    bombs.RemoveAt(j);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Method in charge of spawning the powerups.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="gt"></param>
        public void SpawnPowerUp(Point pos, GameTime gt)
        {
            var randomPowerup = RNG.Next((int) PowerUpType.Count);
            PowerUp newPowerUp = new PowerUp(Content.Load<Texture2D>("Objects\\powerup_0" + (randomPowerup + 1)), pos, (PowerUpType) randomPowerup, soundEffects["powerupSpawn"]);
            powerUps.Add(newPowerUp);
        }
        /// <summary>
        /// Modular way of changing GameState.
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(GameState state)
        {
            crates.Clear();
            currentGameState = state;
            currentMap = new Map(finishedLevel);
            if (state == GameState.InPlay)
                currentMap.SpawnCrates(Content.Load<Texture2D>("Objects\\crate_01"), crates);
        }
        /// <summary>
        /// Restart method for the game. This way the game can be replayed
        /// without needing to close it and reopen it again.
        /// </summary>
        public void RestartGame()
        {
            ChangeState(GameState.InPlay);
            gameOverCounter = gameOverCounterReset;
            crates.Clear();
            powerUps.Clear();
            bombs.Clear();
            currentMap = new Map(finishedLevel);
            currentMap.SpawnCrates(Content.Load<Texture2D>("Objects\\crate_01"), crates);
        }
        /// <summary>
        /// Main update method.
        /// Updates all relevant states and switches to its own relevant states
        /// depending on GameState.
        /// </summary>
        /// <param name="gt"></param>
        protected override void Update(GameTime gt)
        {
            // Assign the gamePadStates to the players
            for (int i = 0; i < gamePadStates.Count; i++)
            {
                gamePadStates[i] = GamePad.GetState((PlayerIndex) i);
            }

            // health update
            for (int i = 0; i < gamePadStates.Count; i++)
            {
                healthDisplay[i].UpdateMe(players[i].Health);
            }

            // PowerUpUI update
            for (int i = 0; i < gamePadStates.Count; i++)
            {
                powerUpUIs[i].UpdateMe(players[i].NumberOfSpeedPus,
                    players[i].NumberOfRadiusPUs, players[i].NumberOfBombIncreasePus);
            }

            // Trak keyboard inputs
            kb_curr = Keyboard.GetState();
            // Track Mouse Position
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
                    GameOverUpdate(gt);
                    break;
            }

            // close game
            //if (kb_curr.IsKeyDown(Keys.Escape))
            //    this.Exit();

            // Test for player reset method
            //for (int i = 0; i < numberOfPlayers; i++)
            //{
            //    if (kb_curr.IsKeyDown(Keys.Up))
            //        players[i].Reset();
            //}

            kb_old = kb_curr;
            oldMouse = currMouse;
            // set the old gamepadstate to the current one
            for (int i = 0; i < gamePadStates.Count; i++)
            {
                oldGamePadStates[i] = gamePadStates[i];
            }
            base.Update(gt);
        }
        /// <summary>
        /// Main draw method. In charge of all draws.
        /// Switches between its own draw methods depending on GameState.
        /// Scales screen to the render target.
        /// </summary>
        /// <param name="gt"></param>
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
            _spriteBatch.Begin();
            _spriteBatch.Draw(_renderTarget, GraphicsDevice.Viewport.Bounds, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
            _spriteBatch.End();
            #endregion
            base.Draw(gt);
        }
        #region STATE UPDATES
        /// <summary>
        /// Start Screen update method
        /// </summary>
        void StartUpdate()
        {
            // Update Buttons
            foreach (var buttonName in Globals.StartScreenButtons)
            {
                buttons[buttonName].UpdateMe(mousepos, currMouse, oldMouse);
            }
        }
        /// <summary>
        /// Method in charge of the Player - Powerup Interaction.
        /// It is it's own method for organisational purposes.
        /// </summary>
        void PlayerPowerupInteraction()
        {
            for (int i = 0; i < numberOfPlayers; i++)                                   // loop through the players
            {
                if (currentMap.IsPowerUpOnCell(players[i].Position.ToPoint()))          // if they are standing on a powerup
                {
                    if (players[i].State == PlayerState.InPlay)                           // if the player is in play
                    {
                        for (int j = 0; j < powerUps.Count; j++)                        // loop through the powerups
                        {
                            if (powerUps[j].Position == players[i].Position.ToPoint())  // check which powerup the player is standing on
                            {
                                soundEffects["maximise"].Play();
                                switch (powerUps[j].Type)                               // act accordingly
                                {
                                    case PowerUpType.Speed:
                                        players[i].SpeedPowerUp();
                                        break;
                                    case PowerUpType.MoreBombs:
                                        players[i].MoreBombsPowerUp();
                                        break;
                                    case PowerUpType.ExplosionRadius:
                                        players[i].ExplosionRadiusPowerUp();
                                        break;
                                }
                                powerUps.RemoveAt(j);                                   // remove the powerup
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Update Method for "In Play" GameState.
        /// </summary>
        /// <param name="gt"></param>
        void InPlayUpdate(GameTime gt)
        {
                // GAME OVER CONDITION
            int livingPlayers = 0;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].State != PlayerState.Dead)
                    livingPlayers++;
            }
            if (livingPlayers <= 1)
            {
                gameOverCounter -= (float)gt.ElapsedGameTime.TotalSeconds;
                if (gameOverCounter < 0)
                {
                    gameOverCounter = gameOverCounterReset;
                    ChangeState(GameState.GameOver);
                }
            }

            // Assign Gamepads to players and initialise them
            for (int i = 0; i < numberOfPlayers; i++)
            {

                gamePadStates[i] = GamePad.GetState((PlayerIndex)i);
            }

            #region Update Methods
            // Player
            for (int i = 0; i < numberOfPlayers; i++)
            {
                var canPlaceBomb = players[i].UpdateMe(gt, currentMap, kb_curr, kb_old, gamePadStates[i], oldGamePadStates[i]);
                if (canPlaceBomb)
                {
                    PlaceBomb(players[i].Position.ToPoint(), gt, players[i].ExplosionRadius, i);
                }
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

            // Player - PowerUps Interraction 
            PlayerPowerupInteraction();

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
        /// <summary>
        /// Method in charge of the "GameOver" GameState.
        /// </summary>
        /// <param name="gt"></param>
        void GameOverUpdate(GameTime gt)
        {
            // Map/Tiles
            currentMap.Update(gt);

            // Buttons
            foreach (var buttonNames in Globals.GameOverScreenButtons)
            {
                buttons[buttonNames].UpdateMe(mousepos, currMouse, oldMouse);
            }

            // Player upate
            for (int i = 0; i < numberOfPlayers; i++)
            {
                if (players[i].State != PlayerState.Dead)
                    players[i].UpdateMe(gt, currentMap, kb_curr, kb_old, gamePadStates[i], oldGamePadStates[i]);
            }

        }
        #endregion
        #region STATE DRAWS
        /// <summary>
        /// Draw Method for Start Screen.
        /// </summary>
        /// <param name="gt"></param>
        void StartDraw(GameTime gt)
        {
            // Map
            currentMap.DrawMe(_spriteBatch, tiles);
            // Screen tint
            gameOverScreenTint.DrawMeAsRect(_spriteBatch, new Rectangle(0, 0, Globals.ScreenWidth, Globals.ScreenHeight), Color.White * .75f);

            // Text
            Rectangle tempRect;
            Vector2 textlength = UIfont.MeasureString("Select Number Of Players, Then Press Start.\nPlayers must use the Dpad to move and X to place Bombs." +
                "\nWhen a player is hit, they become a ghost.\nWhen in Ghost state, press X to respawn." + "\nBreak crates using the bombs to find Poweups." +
                "\nBlue ones Increase Speed, Orange Increase Explosion Radius" +
                "\nand Grey Increase the number of bombs you can place at a time.");
            tempRect = new Rectangle(Globals.ButtonPositions["startButton"].X - 210, Globals.ButtonPositions["startButton"].Y - 210, (int)textlength.X+20, (int)textlength.Y+20);
            gameOverScreenTint.DrawMeAsRect(_spriteBatch, tempRect, Color.Black * .75f);
            _spriteBatch.DrawString(UIfont, 
                "Select Number Of Players, Then Press Start.\nPlayers must use the Dpad to move and X to place Bombs." +
                "\nWhen a player is hit, they become a ghost.\nWhen in Ghost state, press X to respawn." +
                "\nBreak crates using the bombs to find Poweups." +
                "\nBlue ones Increase Speed, Orange Increase Explosion Radius" +
                "\nand Grey Increase the number of bombs you can place at a time.", new Vector2(Globals.ButtonPositions["startButton"].X - 200, Globals.ButtonPositions["startButton"].Y-200), Color.White);
            
            // Buttons
            foreach (var buttonName in Globals.StartScreenButtons)
            {
                buttons[buttonName].DrawMe(_spriteBatch);
            }
        }
        /// <summary>
        /// Draw Method for "InPlay" GameState.
        /// </summary>
        /// <param name="gt"></param>
        void InPlayDraw(GameTime gt)
        {
            // Map
            currentMap.DrawMe(_spriteBatch, tiles);
            // Player
            foreach (var player in players)
            {
                player.DrawMe(_spriteBatch, gt, tiles[0].Width, tiles[1].Height);
            }

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
                health.DrawMe(_spriteBatch);
            }
            // PowerUp UI
            foreach (var powerup in powerUpUIs)
            {
                powerup.DrawMe(_spriteBatch, UIfont);
            }

#if DEBUG
            _spriteBatch.DrawString(debugFont, _graphics.PreferredBackBufferWidth + "x " + _graphics.PreferredBackBufferHeight
                + "\nfps: " + (int)(1 / gt.ElapsedGameTime.TotalSeconds) + "ish" ,
                new Vector2(10, 10), Color.Black);
#endif
           
        }
        /// <summary>
        /// Draw Method for "GameOver" GameState.
        /// </summary>
        /// <param name="gt"></param>
        void GameOverDraw(GameTime gt)
        {
            // Map
            currentMap.DrawMe(_spriteBatch, tiles);

            // screen tint
            gameOverScreenTint.DrawMeAsRect(_spriteBatch, new Rectangle(0, 0, Globals.ScreenWidth, Globals.ScreenHeight), Color.White * .75f);

            for (int i = 0; i < numberOfPlayers; i++)
            {
                players[i].DrawMe(_spriteBatch, gt, tiles[0].Width, tiles[1].Height);
            }

            // Game Over Text
            // If Tied
            int livingPlayers = 0;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].State != PlayerState.Dead)
                    livingPlayers++;
            }
            if (livingPlayers == 0)
            {
                Vector2 textlength = bigUIfont.MeasureString("TIED");

                _spriteBatch.DrawString(bigUIfont, "TIED", new Vector2(Globals.screenCentre.X - textlength.X / 2, Globals.screenCentre.Y - 200), Color.Black);
            }
            // else if there is a clear winner
            else
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].State != PlayerState.Dead)
                    {
                        Vector2 textlength = bigUIfont.MeasureString("PLAYER " + (i + 1) + " WINS");
                        _spriteBatch.DrawString(bigUIfont, "PLAYER " + (i + 1) + " WINS", new Vector2(Globals.screenCentre.X - textlength.X / 2, Globals.screenCentre.Y - 200), Color.Black);
                    }
                }
            }

            // Buttons
            foreach (var buttonName in Globals.GameOverScreenButtons)
            {
                buttons[buttonName].DrawMe(_spriteBatch);
            }
        }
        #endregion
    }
}
