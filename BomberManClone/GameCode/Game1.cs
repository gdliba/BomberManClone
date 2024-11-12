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
        int[,] testfloor;
        Map currentMap;

        //Keyboard
        KeyboardState kb_curr, kb_old;

        //Players
        PC player1;

        //Bomb
        List<Bomb> bombs;

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


#if DEBUG
            debugFont = Content.Load<SpriteFont>("Fonts\\Arial07");
#endif
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _renderTarget = new RenderTarget2D(_spriteBatch.GraphicsDevice, 480, 320);

            // Players
            player1 = new PC(new Point(1, 1), Content.Load<Texture2D>("Characters\\pc"), 3, 8);

            // Bombs
            //bombs.Add(new Bomb (Content.Load<Texture2D>("Objects\\plate"), new Point(4, 1)));


            // Setting up Tile Textures
            tiles.Add(Content.Load<Texture2D>("Tiles\\void"));  // 0
            tiles.Add(Content.Load<Texture2D>("Tiles\\floor")); // 1
            tiles.Add(Content.Load<Texture2D>("Tiles\\wall"));  // 2
            tiles.Add(Content.Load<Texture2D>("Tiles\\wallL")); // 3
            tiles.Add(Content.Load<Texture2D>("Tiles\\wallR")); // 4
            tiles.Add(Content.Load<Texture2D>("Tiles\\wallTL"));// 5
            tiles.Add(Content.Load<Texture2D>("Tiles\\wallTR"));// 6

            tiles.Add(Content.Load<Texture2D>("Tiles\\void"));  // 7


            // Tile Layout
            testfloor = new int[17, 17]
            {
                { 5, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 6},
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
                { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2}

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

        protected override void Update(GameTime gt)
        {
            kb_curr = Keyboard.GetState();

            #region Update States
            // Player
            var canPlaceBomb = player1.InPlay(gt, currentMap, kb_curr, kb_old);
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
            #endregion

            // close game
            if (kb_curr.IsKeyDown(Keys.Escape))
                this.Exit();

            base.Update(gt);
            kb_old = kb_curr;
        }

        protected override void Draw(GameTime gt)
        {
            GraphicsDevice.Clear(Color.Black);

            // Applying scale effect to the screen
            GraphicsDevice.SetRenderTarget(_renderTarget);

            _spriteBatch.Begin();
            // Map
            currentMap.DrawMe(_spriteBatch, tiles);
            // Player
            player1.DrawMe(_spriteBatch, gt, tiles[0].Width, tiles[1].Height);

            // Bombs
            for (int i = 0; i < bombs.Count; i++)
            {
                bombs[i].DrawMe(_spriteBatch);
            }
#if DEBUG
            _spriteBatch.DrawString(debugFont, _graphics.PreferredBackBufferWidth + "x " + _graphics.PreferredBackBufferHeight
                + "\nfps: " + (int)(1 / gt.ElapsedGameTime.TotalSeconds) + "ish",
                new Vector2(10, 10), Color.White*.5f);
#endif
            _spriteBatch.End();
            #region Screen Scaling
            // Completing Scale effect on the screen
            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_renderTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 4, SpriteEffects.None, 1);
            _spriteBatch.End();
            #endregion


            base.Draw(gt);
        }
    }
}
