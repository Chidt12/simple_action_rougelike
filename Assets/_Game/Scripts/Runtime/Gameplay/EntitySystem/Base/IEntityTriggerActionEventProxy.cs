using Cysharp.Threading.Tasks;
using System;
using System.Threading;
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
        void TriggerEvent(AnimationType animationType, CancellationToken cancellationToken, Action<SetStateData> stateAction = null, Action<SetStateData> endAction = null, bool isRefresh = false);
    }
}