using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public enum ActionInputType
    {
        Attack1 = 0,
        Attack2 = 1,
        UseSkill1 = 2,
        UseSkill2 = 3,
        UseSkill3 = 4,
    }

    public interface IEntityControlData : IEntityData
    {
        Action<ActionInputType> ActionTriggeredEvent { get; set; }
        Action MovementChangedEvent { get; set; }
        Action DirectionChangedEvent { get; set; }
        public Vector2 MoveDirection { get; }
        public Vector2 FaceDirection { get; }
        public IEntityPositionData Target { get; } // TODO: Put here properly ?
        void SetMoveDirection(Vector2 direction);
        void SetFaceDirection(Vector2 faceDirection);
    }
}
