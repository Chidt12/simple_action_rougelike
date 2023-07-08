using Runtime.ConfigModel;
using Runtime.Core.Pool;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class AnimatorDamageBox : MonoBehaviour
    {
        [SerializeField] protected AnimatorHolder animatorHolder;
        [SerializeField] protected DamageBox damageBox;
        [SerializeField] protected bool selfDestroy;

        protected Action onEndAnim;
        protected Action onTurnOn;
        protected Action onTurnOff;

        public void Init(
            IEntityData creatorData, EffectSource effectSource, EffectProperty effectProperty, float damageBonus, DamageFactor[] damageFactors, StatusIdentity statusIdentity,
            Action<IEntityData> onTriggeredEntered = null, Action<IEntityData> onTriggeredExit = null, Action onEndAnim = null, Action onTurnOn = null, Action onTurnOff = null)
        {
            animatorHolder.Play("attack");
            this.onEndAnim = onEndAnim;
            this.onTurnOn = onTurnOn;
            this.onTurnOff = onTurnOff;
            damageBox.gameObject.SetActive(false);
            animatorHolder.SetEvents(new() 
            {
                () => OnTurnOnDamageBox(creatorData, effectSource,effectProperty, damageBonus, damageFactors, statusIdentity, onTriggeredEntered, onTriggeredExit),
                OnTurnOffDamageBox,
            }, OnEndAnim);
        }

        public void Scale(Vector2 scale)
        {
            transform.localScale = scale;
        }

        private void OnEndAnim()
        {
            onEndAnim?.Invoke();
            if (selfDestroy)
            {
                PoolManager.Instance.Return(gameObject);
            }
        }

        private void OnTurnOnDamageBox(IEntityData creatorData, EffectSource effectSource, EffectProperty effectProperty, float damageBonus, DamageFactor[] damageFactors, StatusIdentity statusIdentity,
            Action<IEntityData> onTriggeredEntered = null, Action<IEntityData> onTriggeredExit = null)
        {
            onTurnOn?.Invoke();
            damageBox.gameObject.SetActive(true);
            damageBox.StartDamage(creatorData, effectSource, effectProperty, damageBonus, damageFactors, statusIdentity, onTriggeredEntered, onTriggeredExit);
        }

        private void OnTurnOffDamageBox()
        {
            onTurnOff?.Invoke();
            damageBox.gameObject.SetActive(false);
        }
    }
}