using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Worms
{
    class Worm
    {
        private const int MovementSpeed = 2;
        private const int FallSpeed = 5;
        private const int ClimbHeight = 500;

        private const int Height = 20;
        private const int Width = 10;

        public Vector2 Pos { get; private set; }
        private Texture2D _texture;

        private WormState _wormState;

        public Worm(GraphicsDevice graphicsDevice, Vector2 pos)
        {
            Pos = pos;

            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new Color[] { Color.White });

            _wormState = WormState.Falling;
        }

        internal void Update(IReadOnlyCollection<ICollidable> collidables)
        {
            if (_wormState == WormState.Falling)
            {
                Fall(collidables);
            }
            if (_wormState == WormState.Waiting)
            {
                ProcessMovement(collidables);
            }
        }

        private void Fall(IReadOnlyCollection<ICollidable> collidables)
        {
            foreach(ICollidable col in collidables)
            {
                //scan for collision
                for(int i = 0; i <= FallSpeed; i++)
                {
                    int projectedBottomOfSprite = (int)Pos.Y + i + Height/2; //TODO: Replace with reference to sprite when we have it
                    if (col.WillCollide(new Point((int)Pos.X, projectedBottomOfSprite)))
                    {
                        Pos = new Vector2(Pos.X, Pos.Y + i);
                        _wormState = WormState.Waiting;
                        return;
                    }
                }
            }

            Pos = new Vector2(Pos.X, Pos.Y + FallSpeed);
        }

        private void ProcessMovement(IReadOnlyCollection<ICollidable> collidables)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                Move(MovementDirection.Left, collidables);
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                Move(MovementDirection.Right, collidables);
            }
            CheckForFall(collidables);
        }

        private void CheckForFall(IReadOnlyCollection<ICollidable> collidables)
        {
            Point ground = new Point((int)Pos.X, (int)Pos.Y - Height / 2);
            foreach(ICollidable col in collidables)
            {
                if (col.WillCollide(ground))
                {
                    return;
                }
            }
            _wormState = WormState.Falling;
        }

        private void Move(MovementDirection direction, IReadOnlyCollection<ICollidable> collidables)
        {
            foreach (ICollidable col in collidables)
            {
                //scan for collision
                for (int i = 0; i <= MovementSpeed; i++)
                {
                    int projectedEdgeOfSprite = (int)Pos.X + (Width/2+i)*(int)direction; //TODO: Replace with reference to sprite when we have it
                    int bottomOfSprite = (int)Pos.Y + Height / 2 - 1;
                    Point collisionPoint = new Point(projectedEdgeOfSprite, bottomOfSprite);
                    if (col.WillCollide(collisionPoint))
                    {
                        Point climbPoint = new Point(collisionPoint.X - i, collisionPoint.Y);
                        if (AttemptToClimb(climbPoint, collidables) == false)
                        {
                            //Have hit wall we cant climb. Move to wall.
                            Pos = new Vector2(Pos.X + i * (int)direction, Pos.Y);
                            return;
                        }
                        return;
                    }
                }
            }
            //Can move freely.
            Pos = new Vector2(Pos.X + MovementSpeed * (int)direction, Pos.Y);
        }

        private bool AttemptToClimb(Point collisionPoint, IReadOnlyCollection<ICollidable> collidables)
        {
            foreach(ICollidable col in collidables)
            {
                for(int i = 1; i <= ClimbHeight; i++)
                {
                    Point climbDestination = new Point(collisionPoint.X, collisionPoint.Y - i - Height/2 - 1);
                    if (col.WillCollide(climbDestination) == false)
                    {
                        Pos = new Vector2(climbDestination.X, climbDestination.Y);
                        return true;
                    }
                }
            }
            return false;
        }

        public void Draw(SpriteBatch batch)
        {
            Rectangle rect = new Rectangle(
                new Point((int)Pos.X - (Width/2), (int)Pos.Y - (Height/2)),
                new Point(Width,Height)
            );
            batch.Draw(_texture, rect, Color.White);
        }

        private enum MovementDirection
        {
            Left = -1,
            Right = 1
        }
    }
}
