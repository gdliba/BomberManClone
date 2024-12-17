using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BomberManClone
{
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

        public static void ChangeResolution(int width, int height)
        {
            Graphics.PreferredBackBufferWidth = width;
            Graphics.PreferredBackBufferHeight = height;
            Graphics.ApplyChanges();
        }


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

        public static readonly List<Point> PlayerSpawnPoints = new List<Point>()
        {
            new Point(2,1), new Point(14,13), new Point(14,1), new Point(2,13)
        };

        public static readonly List<Color> PlayerTints = new List<Color>()
        {
            Color.White,
            Color.Goldenrod,
            Color.LightSkyBlue,
            Color.Pink
        };

        public static readonly List<string> StartScreenButtons = new List<string>()
        {
            "Start",
            "Exit",
            "2 Players",
            "3 Players",
            "4 Players"
        };

        public static readonly List<string> GameOverScreenButtons = new List<string>()
        {
            "Restart",
            "Main Menu"

        };
    }
}
