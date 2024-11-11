using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BomberManClone
{
    class PC : GameActor
    {
        public PC(Point startPos, Texture2D txr, int frameCount, int fps)
            :base(startPos, txr, frameCount, fps)
        {

        }
        public void updateme(GameTime gameTime, Map currentMap,
            KeyboardState kb_curr, KeyboardState kb_old)
        {
            m_northCell = new Vector2(m_position.X, m_position.Y - 1);
            m_southCell = new Vector2(m_position.X, m_position.Y + 1);
            m_eastCell = new Vector2(m_position.X + 1, m_position.Y);
            m_westCell = new Vector2(m_position.X - 1, m_position.Y);

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var obstacles =currentMap.getObstacles();

            //#region Movement
            //if (kb_curr.IsKeyDown(Keys.W))
            //{
            //    m_facing = Direction.North;
            //    if (currentMap.isWalkableForPlayer(m_northCell))
            //        moveme(Direction.North, deltaTime);
            //}
            //if (kb_curr.IsKeyDown(Keys.S))
            //{
            //    m_facing = Direction.South;
            //    if (currentMap.isWalkableForPlayer(m_southCell))
            //        moveme(Direction.South, deltaTime);
            //}
            //if (kb_curr.IsKeyDown(Keys.A))
            //{
            //    m_facing = Direction.West;
            //    if (currentMap.isWalkableForPlayer(m_westCell))
            //        moveme(Direction.West, deltaTime);
            //}
            //if (kb_curr.IsKeyDown(Keys.D))
            //{
            //    m_facing = Direction.East;
            //    if (currentMap.isWalkableForPlayer(m_eastCell))
            //        moveme(Direction.East, deltaTime);
            //}
            //#endregion
            for (int i = 0; i < obstacles.Count; i++)
            {
                if (kb_curr.IsKeyDown(Keys.W) &! m_sourceRect.Intersects(obstacles[i]))
                {
                    m_facing = Direction.North;
                        moveme(Direction.North, deltaTime);
                }
                if (kb_curr.IsKeyDown(Keys.S) & !m_sourceRect.Intersects(obstacles[i]))
                {
                    m_facing = Direction.South;
                        moveme(Direction.South, deltaTime);
                }
                if (kb_curr.IsKeyDown(Keys.A) & !m_sourceRect.Intersects(obstacles[i]))
                {
                    m_facing = Direction.West;
                        moveme(Direction.West, deltaTime);
                }
                if (kb_curr.IsKeyDown(Keys.D) & !m_sourceRect.Intersects(obstacles[i]))
                {
                    m_facing = Direction.East;
                        moveme(Direction.East, deltaTime);
                }
            }
        }
    }
}
