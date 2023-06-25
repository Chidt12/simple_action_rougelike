using Runtime.Core.Pool;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class AnimatorDamageBox : MonoBehaviour
    {
        [SerializeField] private AnimatorHolder _animatorHolder;
        [SerializeField] private DamageBox _damageBox;
        [SerializeField] private bool _selfDestroy;

        private Action<IEntityData> _onTriggeredEntered;
        private Action<IEntityData> _onTriggeredExit;

        public void Init(Action<IEntityData> onTriggeredEntered, Action<IEntityData> onTriggeredExit = null)
        {
            _onTriggeredEntered = onTriggeredEntered;
            _onTriggeredExit = onTriggeredExit;
            _animatorHolder.Play("attack");
            _damageBox.gameObject.SetActive(false);
            _animatorHolder.SetEvents(new() 
            {
                OnTurnOnDamageBox,
                OnTurnOffDamageBox,
            }, OnEndAnim);
        }

        private void OnEndAnim()
        {
            if (_selfDestroy)
            {
                PoolManager.Instance.Return(gameObject);
            }
        }

        private void OnTurnOnDamageBox()
        {
            _damageBox.gameObject.SetActive(true);
            _damageBox.StartDamage(_onTriggeredEntered, _onTriggeredExit);
        }

        private void OnTurnOffDamageBox()
        {
            _damageBox.gameObject.SetActive(false);
        }
    }
}