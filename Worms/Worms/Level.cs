using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Worms
{
    class Level
    {
        private readonly GraphicsDevice _graphicsDevice;

        private Vector2 CENTER => new Vector2(
            _graphicsDevice.Viewport.Width / 2,
            _graphicsDevice.Viewport.Height / 2);

        private Worm[] _worms;
        private int _currentWorm = 0;

        private Terrain _terrain;
        private List<ICollidable> _collidables;

        private GameState _state;

        public Level(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _worms = new Worm[] { new Worm(graphicsDevice, CENTER) };
            _state = GameState.PlayerControl;
            _terrain = new Terrain(graphicsDevice);
            _collidables = new List<ICollidable> { _terrain };
        }

        internal void Update()
        {
            if (_state == GameState.PlayerControl)
            {
                _worms[_currentWorm].Update(_collidables);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(Worm w in _worms)
            {
                w.Draw(spriteBatch);
            }
            _terrain.Draw(spriteBatch);
        }
    }
}
