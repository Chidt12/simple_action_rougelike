using Pathfinding;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IAutoInputStrategy : IDisposable
    {
        void Update();
    }
}
