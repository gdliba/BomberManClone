using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace BomberManClone
{
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

        // Tiles/Map
        List<Texture2D> tiles;
        int[,] testfloor, testfloor2;
        Map currentMap;

        //Keyboard
        KeyboardState kb_curr, kb_old;

        //Players
        PC player1;

        //Bomb
        List<Bomb> bombs;

        // PowerUps
        List<PowerUp> powerUps;
        Texture2D speedPUText, extraBombPUText, explosionRadiusPUText, randomPUText; 

        // Crates
        List<Crate> crates;

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
        }

        protected override void Initialize()
        {
            // List initialisations
            tiles = new List<Texture2D>();
            bombs = new List<Bomb>();
            powerUps = new List<PowerUp>();
            crates = new List<Crate>();


#if DEBUG
            debugFont = Content.Load<SpriteFont>("Fonts\\Arial07");
#endif
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _renderTarget = new RenderTarget2D(_spriteBatch.GraphicsDevice, 1088, 1088);

            // Players
            player1 = new PC(new Point(1, 1), Content.Load<Texture2D>("Characters\\player_spritesheetHighlighted"), 3, 4);

            // Bombs
            //bombs.Add(new Bomb (Content.Load<Texture2D>("Objects\\plate"), new Point(4, 1)));

            // PowerUps
            speedPUText = Content.Load<Texture2D>("Objects\\powerup_01");
            extraBombPUText = Content.Load<Texture2D>("Objects\\powerup_02");
            explosionRadiusPUText = Content.Load<Texture2D>("Objects\\powerup_03");
            powerUps.Add(new PowerUp(explosionRadiusPUText, new Point(4, 5), PowerUpType.Speed));

            // Crates
            crates.Add(new Crate(Content.Load<Texture2D>("Objects\\crate_01"), new Point(4, 1)));


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
            currentMap = new Map(testfloor);

        }
        public bool PlaceBomb(Point pos, GameTime gt)
        {
            Bomb newBomb = new Bomb(Content.Load<Texture2D>("Objects\\plate"), pos);
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
        public bool SpawnPowerUp(Point pos, GameTime gt)
        {
            var randomPowerup = RNG.Next(0, 3);
            if (randomPowerup == 0)
                randomPUText = speedPUText;
            else if (randomPowerup == 1)
                randomPUText = extraBombPUText;
            else if (randomPowerup == 2)
                randomPUText = explosionRadiusPUText;
            PowerUp newPowerUp = new PowerUp(randomPUText, pos, PowerUpType.Speed);
            powerUps.Add(newPowerUp);


            return true;
        }

        protected override void Update(GameTime gt)
        {
            kb_curr = Keyboard.GetState();

            #region Update States
            // Player
            var canPlaceBomb = player1.UpdateMe(gt, currentMap, kb_curr, kb_old);
            if (canPlaceBomb)
            {
                PlaceBomb(player1.Position.ToPoint(), gt);
            }

            // Bombs
            for (int i = 0; i < bombs.Count; i++)
            {
                bombs[i].UpdateMe(gt,currentMap);
                if (bombs[i].State==BombStates.Dead)
                    bombs.RemoveAt(i);
            }

            // Map/Tiles
            currentMap.Update(gt);

            // PowerUps
            for (int i = 0; i < powerUps.Count; i++)
            {
                powerUps[i].UpdateMe(gt, currentMap);
            }

            // Crates
            for (int i = 0; i < crates.Count; i++)
            {
                crates[i].UpdateMe(currentMap);
                var spawnPowerUp = crates[i].UpdateMe(currentMap);
                    if (spawnPowerUp != null)
                        SpawnPowerUp(crates[i].Position, gt);
                if (crates[i].State == CrateState.Dead)
                {
                    crates.RemoveAt(i);
                }
            }

            #endregion

            // close game
            if (kb_curr.IsKeyDown(Keys.Escape))
                this.Exit();

            base.Update(gt);
            kb_old = kb_curr;
        }

        protected override void Draw(GameTime gt)
        {

            // Applying scale effect to the screen
            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.Clear(Color.Tan);

            _spriteBatch.Begin();
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

#if DEBUG
            _spriteBatch.DrawString(debugFont, _graphics.PreferredBackBufferWidth + "x " + _graphics.PreferredBackBufferHeight
                + "\nfps: " + (int)(1 / gt.ElapsedGameTime.TotalSeconds) + "ish" +
                "\nplayer1 Shields: " + player1.Shields,
                new Vector2(10, 10), Color.Black);
#endif
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
    }
}
