using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BomberManClone
{
    public static class Globals
    {
        public static readonly float PickUpSpawnChance = .33f;
        public static readonly float CrateSpawnChance = .5f;
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
