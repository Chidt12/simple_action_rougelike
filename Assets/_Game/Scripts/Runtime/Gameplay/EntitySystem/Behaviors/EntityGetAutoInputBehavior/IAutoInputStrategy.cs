using Pathfinding;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IAutoInputStrategy : IDisposable, IAstarAI
    {
        void Update();
    }
}
