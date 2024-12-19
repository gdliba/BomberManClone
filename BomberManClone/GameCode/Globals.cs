using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BomberManClone
{
    /// <summary>
    /// This class is here for organisational purposes.
    /// It is a convenient way to store a large amount of variables, keeping Game1 more organised.
    /// </summary>
    public static class Globals
    {
        public static GraphicsDeviceManager Graphics;
        public static int ScreenWidth { get => Graphics.PreferredBackBufferWidth; }
        public static int ScreenHeight { get => Graphics.PreferredBackBufferHeight; }
        public static Point screenCentre { get => new Point(ScreenWidth >> 1, ScreenHeight >> 1); }
        public static readonly float PickUpSpawnChance = .33f;
        public static readonly float CrateSpawnChance = .5f;
        public static List<Point> HealthDisplayPoints;
        public static Dictionary<string, Point> ButtonPositions;
        /// <summary>
        /// Some variables in this class use the screen resolution to calculate points,
        /// This method ensures that those measurements remain relevant if the resolution
        /// were to change at any point.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void ChangeResolution(int width, int height)
        {
            Graphics.PreferredBackBufferWidth = width;
            Graphics.PreferredBackBufferHeight = height;
            Graphics.ApplyChanges();
        }

        /// <summary>
        /// This initialise method needs to be called so that it can update the 
        /// values relevant to the screen resolution.
        /// </summary>
        public static void Initialise()
        {
            HealthDisplayPoints = new List<Point>()
            {
                Point.Zero,
                new Point(ScreenWidth - 64, 12*64),
                new Point(ScreenWidth - 64, 0),
                new Point(0, 12*64)
            };

            ButtonPositions = new Dictionary<string, Point>()
            {
                {"startButton", new Point(screenCentre.X - 80, screenCentre.Y - 22) },
                {"exitButton", new Point(screenCentre.X - 80, screenCentre.Y + 45) },
                {"twoPlayersButton", new Point(screenCentre.X - 400, screenCentre.Y - 22) },
                {"threePlayersButton", new Point(screenCentre.X - 400, screenCentre.Y + 38) },
                {"fourPlayersButton", new Point(screenCentre.X - 400, screenCentre.Y + 98) }

            };
        }
        /// <summary>
        /// A collection of points the players can spawn on.
        /// </summary>
        public static readonly List<Point> PlayerSpawnPoints = new List<Point>()
        {
            new Point(2,1), new Point(14,13), new Point(14,1), new Point(2,13)
        };
        /// <summary>
        /// A collection of tints applied to the players to visually distinguish them.
        /// </summary>
        public static readonly List<Color> PlayerTints = new List<Color>()
        {
            Color.White,
            Color.Goldenrod,
            Color.LightSkyBlue,
            Color.Pink
        };
        /// <summary>
        /// A collection of strings referencing which buttons are to appear in the "Start" Screen.
        /// </summary>
        public static readonly List<string> StartScreenButtons = new List<string>()
        {
            "Start",
            "Exit",
            "2 Players",
            "3 Players",
            "4 Players"
        };
        /// <summary>
        /// A collection of strings referencing which buttons are to appear in the "GameOver" Screen.
        /// </summary>
        public static readonly List<string> GameOverScreenButtons = new List<string>()
        {
            "Restart",
            "Main Menu"

        };
    }
}
