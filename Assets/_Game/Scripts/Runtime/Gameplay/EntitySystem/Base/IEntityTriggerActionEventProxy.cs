using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class SetStateData
    {
        public Transform[] spawnVFXPoints;

        public SetStateData(Transform[] spawnVFXPoints = null)
        {
            this.spawnVFXPoints = spawnVFXPoints;
        }
    }

    public interface IEntityTriggerActionEventProxy
    {
        void TriggerEvent(AnimationType animationType, Action<SetStateData> stateAction = null, Action<SetStateData> endAction = null, bool isRefresh = false);
    }

    public class DummyEntityTriggerActionEventProxy : IEntityTriggerActionEventProxy
    {
        public void TriggerEvent(AnimationType animationType, Action<SetStateData> stateAction = null, Action<SetStateData> endAction = null, bool isRefresh = false)
        {
            stateAction?.Invoke(new SetStateData());
            endAction?.Invoke(new SetStateData());
        }
    }
}