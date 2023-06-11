using Runtime.Helper;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class RotatePivot : MonoBehaviour, IEntityAnimation
    {
        private const float ROTATE_SPEED = 1200;
        private IEntityControlData _controlData;
        private bool _inited;

        public void Init(IEntityControlData controlData)
        {
            _inited = true;
            _controlData = controlData;
        }

        private void Update()
        {
            if (!_inited)
                return;

            var toRotation = _controlData.FaceDirection.ToQuaternion(0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, ROTATE_SPEED * Time.deltaTime);
        }

        public void ChangeColor(Color changedColor) {}

        public void Continue() {}

        public void Dispose() {}

        public Transform[] GetVFXSpawnPoints(AnimationType animationType) => null;

        public bool IsMainPart(AnimationType animationType) => false;

        public void Pause() {}

        public void Play(AnimationType animationType) {}

        public void SetTriggeredEvent(AnimationType animationType, Action<SetStateData> stateAction, Action<SetStateData> endAction) {}
    }

}