using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityAnimation
    {
        bool IsMainPart(AnimationType animationType);
        void Init(IEntityControlData controlData);
        void Play(AnimationType animationType);
        void ChangeColor(Color changedColor);
        void Continue();
        void Pause();
        void SetTriggeredEvent(AnimationType animationType, Action<SetStateData> stateAction, Action<SetStateData> endAction);
        void Dispose();
        Transform[] GetVFXSpawnPoints(AnimationType animationType);
    }
}