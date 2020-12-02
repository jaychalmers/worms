using Microsoft.Xna.Framework;

namespace Worms
{
    interface ICollidable
    {
        bool WillCollide(Point destination);
    }
}
