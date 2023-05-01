using Runtime.Extensions;
using System.Linq;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class WeaponEntityAnimation : AnimatorEntityAnimation
    {
        [SerializeField] private bool _rotateTowardFaceDirection;
        [SerializeField] private Transform _flipPivotTransform;
        [SerializeField] private Transform _rotateTransform;

        private const float ROTATE_SPEED = 1080;
        private IEntityControlData _controlData;
        private bool _inited;

        #region API Methods

        private void Update()
        {
            if (!_inited)
                return;

            if(currentAnimationType == AnimationType.None || currentAnimationType == AnimationType.Idle || currentAnimationType == AnimationType.Run)
            {
                if (_rotateTowardFaceDirection)
                {
                    var toRotation = _controlData.FaceDirection.ToQuaternion(0);
                    _rotateTransform.rotation = Quaternion.RotateTowards(_rotateTransform.rotation, toRotation, ROTATE_SPEED * Time.deltaTime);
                    var degree = Quaternion.Angle(_rotateTransform.rotation, Quaternion.identity);
                    if (degree > 90 || degree < -90)
                        _flipPivotTransform.localScale = new Vector3(1, -1, 1);
                    else
                        _flipPivotTransform.localScale = new Vector3(1, 1, 1);
                }
            }
        }

        #endregion API Methods

        public override void Init(IEntityControlData controlData)
        {
            base.Init(controlData);
            _inited = true;
            _controlData = controlData;
        }

        public override void Dispose() 
        {
            _inited = false;
        }
    }
}
