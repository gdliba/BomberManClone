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
        List<PC> players;
        Texture2D toombstoneTxr, playerTxr;

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
        Point startButtonPos, exitButtonPos, twoPlayerButtonPos, threePlayerButtonPos, fourPlayerButtonPos;

        // Health UI
        Texture2D healthTxr, healthTxrEmpty;
        List<HealthUI> healthDisplay;
        Point player1HealthDisplay, player2HealthDisplay, player3HealthDisplay, player4HealthDisplay;

        // Sounds
        SoundEffect buttonHoverSfx, buttonPressedSfx, maximiseSfx, minimiseSfx, footstepSfx, PUspawnSfx, deathSfx, fuzeSfx;
        SoundEffectInstance buttonHoverInstance, buttonPressedInstance, maximiseInstance, minimiseInstance;

        // GamePadStates
        List<GamePadState> gamePadStates;
        List<GamePadState> oldGamePadStates;
        int numberOfPlayers;

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
            gamePadStates = new List<GamePadState>();
            oldGamePadStates = new List<GamePadState>();
            players = new List<PC>();

            // Setting points
            player1HealthDisplay = new Point(0, 0); // top left
            player2HealthDisplay = new Point(screenWidth-64, 12*64); // bottom right
            player3HealthDisplay = new Point(screenWidth-64, 0); // top right
            player4HealthDisplay = new Point(0, 12*64);

            // Button Positions
            startButtonPos = new Point(screenCentre.X - 80, screenCentre.Y - 22);
            exitButtonPos = new Point(startButtonPos.X, screenCentre.Y + 45);
            twoPlayerButtonPos = new Point(screenCentre.X - 400, startButtonPos.Y);
            threePlayerButtonPos = new Point(twoPlayerButtonPos.X, twoPlayerButtonPos.Y + 60);
            fourPlayerButtonPos = new Point(twoPlayerButtonPos.X, threePlayerButtonPos.Y + 60);

#if DEBUG
            debugFont = Content.Load<SpriteFont>("Fonts\\Arial07");
#endif
            currentGameState = GameState.Start;
            numberOfPlayers = 1;
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
            fuzeSfx = Content.Load<SoundEffect>("Sounds\\sizzle");




            buttonHoverInstance = buttonHoverSfx.CreateInstance();
            buttonPressedInstance = buttonPressedSfx.CreateInstance();
            maximiseInstance = maximiseSfx.CreateInstance();
            minimiseInstance = minimiseSfx.CreateInstance();

            // Players
            toombstoneTxr = Content.Load<Texture2D>("Objects\\toombstone");
            playerTxr = Content.Load<Texture2D>("Characters\\player_spritesheetHighlighted");
            //player1 = new PC(new Point(2, 1), Content.Load<Texture2D>("Characters\\player_spritesheetHighlighted"), 3, 4, footstepSfx, toombstoneTxr, deathSfx);

            //Buttons
            buttonTxr = Content.Load<Texture2D>("UI\\button");
            buttonTxrPressed = Content.Load<Texture2D>("UI\\button_pressed");

            var startButton = new Button(buttonTxr, buttonTxrPressed, startButtonPos, buttonHoverSfx, maximiseSfx);
            startButton.OnClick += delegate 
            {
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    players.Add(new PC(Globals.PlayerSpawnPoints[i], playerTxr, 3, 4, footstepSfx, toombstoneTxr, deathSfx, Globals.PlayerTints[i]));
                    gamePadStates.Add(new GamePadState());
                }
                ChangeState(GameState.InPlay); 
            };
            buttons.Add(startButton);

            var exitButton = new Button(buttonTxr, buttonTxrPressed, exitButtonPos, buttonHoverSfx, minimiseSfx);
            exitButton.OnClick += ()=> Exit();
            buttons.Add(exitButton);

            var twoPlayersButton = new Button(buttonTxr, buttonTxrPressed, twoPlayerButtonPos, buttonHoverSfx, maximiseSfx);
            twoPlayersButton.OnClick += () => numberOfPlayers = 2;
            buttons.Add(twoPlayersButton);

            var threePlayersButton = new Button(buttonTxr, buttonTxrPressed, threePlayerButtonPos, buttonHoverSfx, maximiseSfx);
            threePlayersButton.OnClick += () => numberOfPlayers = 3;
            buttons.Add(threePlayersButton);

            var fourPlayersButton = new Button(buttonTxr, buttonTxrPressed, fourPlayerButtonPos, buttonHoverSfx, maximiseSfx);
            fourPlayersButton.OnClick += () => numberOfPlayers = 4;
            buttons.Add(fourPlayersButton);





            // Setting up Tile Textures
            tiles.Add(Content.Load<Texture2D>("Tiles\\ground_01"));  // 0
            tiles.Add(Content.Load<Texture2D>("Tiles\\ground_01")); // 1
            tiles.Add(Content.Load<Texture2D>("Tiles\\shelf_01"));  // 2
            tiles.Add(Content.Load<Texture2D>("Tiles\\wall_04")); // 3
            tiles.Add(Content.Load<Texture2D>("Tiles\\wall_04")); // 4
            tiles.Add(Content.Load<Texture2D>("Tiles\\wall_04"));// 5
            tiles.Add(Content.Load<Texture2D>("Tiles\\wall_04"));// 6

            tiles.Add(Content.Load<Texture2D>("Tiles\\ground_01"));  // 7
            tiles.Add(Content.Load<Texture2D>("Tiles\\wall_04"));  // 8
            tiles.Add(Content.Load<Texture2D>("Tiles\\ground_01"));  // 9
            tiles.Add(Content.Load<Texture2D>("Tiles\\occupiedCell"));  // 10
            tiles.Add(Content.Load<Texture2D>("Tiles\\empty"));  // 11
            tiles.Add(Content.Load<Texture2D>("Tiles\\ground_01")); // 12
            tiles.Add(Content.Load<Texture2D>("Tiles\\ground_01"));  // 13





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
            //currentMap.SpawnCrates(Content.Load<Texture2D>("Objects\\crate_01"), crates);
        }
        public bool PlaceBomb(Point pos, GameTime gt, int explosionRadius, int i)
        {
            var newBomb = new Bomb(Content.Load<Texture2D>("Objects\\sodaBomb"), pos, explosionRadius, fuzeSfx);
            bombs.Add(newBomb);

            // Subscribe to the OnExplode event
            newBomb.OnExplode += players[i].IncreaseBombCount;

            // Iterate bombs to check for explosions and remove dead bombs
            for (int j = 0; j < bombs.Count; j++)
            {
                bombs[j].UpdateMe(gt, currentMap);
                if (bombs[j].State == BombStates.Dead)
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
            currentMap = new Map(finishedLevel);
            currentMap.SpawnCrates(Content.Load<Texture2D>("Objects\\crate_01"), crates);

        }

        protected override void Update(GameTime gt)
        {
            for (int i = 0; i < gamePadStates.Count; i++)
            {

                gamePadStates[i] = GamePad.GetState((PlayerIndex)i);
            }

            // health update
            for (int i = 0; i < gamePadStates.Count; i++)
            {
                healthDisplay[i].UpdateMe(players[i].Health);
            }

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
            for (int i = 0; i < numberOfPlayers; i++)
            {
                if (kb_curr.IsKeyDown(Keys.Up))
                    players[i].Reset();
            }

                base.Update(gt);
            kb_old = kb_curr;
            oldMouse = currMouse;
            // set the old gamepadstate to the current one
            for (int i = 0; i < gamePadStates.Count; i++)
            {
                oldGamePadStates.Add(new GamePadState());
                oldGamePadStates[i] = gamePadStates[i];
            }
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
                buttons[i].UpdateMe(mousepos, currMouse, oldMouse); // Start Button

                //buttons[0].UpdateMe(mousepos, currMouse, oldMouse); // Start Button
                ////buttons[0].OnClick += () => ChangeStateToPlay();
                ////buttons[0].OnClick += (_, _) => ChangeState(GameState.InPlay);
                //buttons[0].OnClick += delegate { ChangeState(GameState.InPlay); };
                //buttons[1].UpdateMe(mousepos, currMouse, oldMouse); // Exit Button
                //buttons[1].OnClick += () => Exit();
            }

        }
        void InPlayUpdate(GameTime gt)
        {
            int livingPlayers = 0;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].State != PlayerState.Dead)
                    livingPlayers++;
            }
            if (livingPlayers <= 1)
            {

                ChangeState(GameState.InPlay);
            }

            for (int i = 0; i < numberOfPlayers; i++)
            {

                gamePadStates[i] = GamePad.GetState((PlayerIndex)i);
            }

            #region Update States
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

            //////////////////// Player - PowerUps Interraction //////////////////// 
            for (int i = 0; i < numberOfPlayers; i++)                                   // loop through the players
            {
                if (currentMap.IsPowerUpOnCell(players[i].Position.ToPoint()))          // if they are standing on a powerup
                {
                    if (players[i].State==PlayerState.InPlay)                           // if the player is in play
                    {
                        for (int j = 0; j < powerUps.Count; j++)                        // loop through the powerups
                        {
                            if (powerUps[j].Position == players[i].Position.ToPoint())  // check which powerup the player is standing on
                            {
                                maximiseSfx.Play();
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
                buttons[2].DrawMe(_spriteBatch, "2 Players", debugFont);
                buttons[3].DrawMe(_spriteBatch, "3 Players", debugFont);
                buttons[4].DrawMe(_spriteBatch, "4 Players", debugFont);

            }
        }
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

#if DEBUG
            _spriteBatch.DrawString(debugFont, _graphics.PreferredBackBufferWidth + "x " + _graphics.PreferredBackBufferHeight
                + "\nfps: " + (int)(1 / gt.ElapsedGameTime.TotalSeconds) + "ish" ,
                new Vector2(10, 10), Color.Black);
#endif
           
        }
        void GameOverDraw(GameTime gt)
        {

        }
        #endregion
    }
}
