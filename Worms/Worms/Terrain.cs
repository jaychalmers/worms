using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Worms
{
    class Terrain : ICollidable
    {
        private RLEColumn[] _columns;
        private Texture2D _texture;

        private readonly int _floor;
        private readonly int _maxHeight;
        private const int IslandMarginPixels = 20;
        private const int ClimbModeChangeFactor = 30;
        private const int SlopeFactor = 2;

        public Terrain(GraphicsDevice graphicsDevice)
        {
            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new Color[] { Color.RosyBrown });

            _columns = new RLEColumn[graphicsDevice.Viewport.Width];
            _floor = graphicsDevice.Viewport.Height;
            _maxHeight = (_floor / 2) + 30;

            PopulateColumns();
            //Well();
        }

        public bool WillCollide(Point point)
        {
            if (point.X < 0 || point.X >= _columns.Length)
            {
                //out of bounds
                return false;
            }
            foreach(RLERange range in _columns[point.X].Ranges)
            {
                if (point.Y <= range.Lower && point.Y >= range.Upper)
                {
                    return true;
                }
            }
            return false;
        }

        private void FlatCols(GraphicsDevice graphicsDevice)
        {
            for (int i = 0; i < _columns.Length; i++)
            {
                _columns[i] = new RLEColumn(graphicsDevice.Viewport.Height, graphicsDevice.Viewport.Height - 30);
            }
        }

        private void Well()
        {
            int edge = _columns.Length / 4;
            for(int i = 0; i < _columns.Length; i++)
            {
                if (i < edge || i > _columns.Length - edge)
                {
                    _columns[i] = new RLEColumn(_floor, _floor - 250);
                }
                else
                {
                    _columns[i] = new RLEColumn(_floor, _floor - 50);
                }
            }
        }

        private void PopulateColumns()
        {
            Random r = new Random();
            int nextHeight = _floor;
            int columnPointer = 0;

            //Initial rise
            TerrainGenClimbMode climbMode = TerrainGenClimbMode.Rise;
            while (columnPointer < IslandMarginPixels)
            {
                _columns[columnPointer] = new RLEColumn(_floor, nextHeight);
                nextHeight = GetNextHeight(nextHeight, climbMode);
                columnPointer++;
            }

            //Random body
            int nextChange = columnPointer + r.Next(ClimbModeChangeFactor);
            while (columnPointer < _columns.Length - IslandMarginPixels)
            {
                _columns[columnPointer] = new RLEColumn(_floor, nextHeight);
                if (columnPointer == nextChange)
                {
                    climbMode = GetNextClimbMode(climbMode, nextHeight);
                    nextChange = nextChange + 10 + r.Next(ClimbModeChangeFactor);
                }
                nextHeight = GetNextHeight(nextHeight, climbMode);
                columnPointer++;
            }
            //Slope out
            climbMode = TerrainGenClimbMode.Fall;
            while (columnPointer < _columns.Length)
            {
                _columns[columnPointer] = new RLEColumn(_floor, nextHeight);
                nextHeight = GetNextHeight(nextHeight, climbMode);
                columnPointer++;
            }
        }

        private TerrainGenClimbMode GetNextClimbMode(TerrainGenClimbMode currentMode, int currentHeight)
        {
            if (currentHeight > _floor - 30)
            {
                return TerrainGenClimbMode.Rise;
            }

            if (currentHeight < _maxHeight + 30)
            {
                return TerrainGenClimbMode.Fall;
            }
            /*
            List<TerrainGenClimbMode> modes = new List<TerrainGenClimbMode>()
            {
                TerrainGenClimbMode.Fall,
                TerrainGenClimbMode.Level,
                TerrainGenClimbMode.Rise
            };
            modes.Remove(currentMode);
            return modes[new Random().Next(2)];*/
            return (TerrainGenClimbMode)new Random().Next(3);
        }

        private int GetNextHeight(int currentHeight, TerrainGenClimbMode mode)
        {
            Random rand = new Random();
            if (mode == TerrainGenClimbMode.Fall)
            {
                if (currentHeight < _floor)
                {
                    return currentHeight + rand.Next(SlopeFactor) + 1;
                }
            }
            else if (mode == TerrainGenClimbMode.Rise)
            {
                if (currentHeight > _maxHeight)
                {
                    return currentHeight - rand.Next(SlopeFactor) - 1;
                }
            }
            return currentHeight;
        }

        internal void Draw(SpriteBatch batch)
        {
            for(int i = 0; i < _columns.Length; i++)
            {
                foreach(RLERange range in _columns[i].Ranges)
                {
                    int height = range.Lower - range.Upper;
                    int yOrigin = range.Lower - height;
                    Rectangle rect = new Rectangle(i, yOrigin, 1, height);
                    batch.Draw(_texture, rect, Color.White);
                }
            }
        }

        private class RLEColumn
        {
            internal List<RLERange> Ranges;

            /// <summary>Generates an RLEColumn with a single range</summary>
            internal RLEColumn(int floor, int ceiling)
            {
                Ranges = new List<RLERange>
                {
                    { new RLERange()
                        {
                            Lower = floor,
                            Upper = ceiling
                        }
                    }
                };
            }
        }

        private struct RLERange
        {
            internal int Lower;
            internal int Upper;
        }

        private enum TerrainGenClimbMode
        {
            Rise,
            Fall,
            Level
        }
    }
}
