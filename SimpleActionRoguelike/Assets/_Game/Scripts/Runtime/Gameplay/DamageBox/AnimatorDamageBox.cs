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

        public void Init(
            IEntityData creatorData, EffectSource effectSource, EffectProperty effectProperty, float damageBonus, DamageFactor[] damageFactors, StatusIdentity statusIdentity,
            Action<IEntityData> onTriggeredEntered = null, Action<IEntityData> onTriggeredExit = null)
        {
            animatorHolder.Play("attack");
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
            if (selfDestroy)
            {
                PoolManager.Instance.Return(gameObject);
            }
        }

        private void OnTurnOnDamageBox(IEntityData creatorData, EffectSource effectSource, EffectProperty effectProperty, float damageBonus, DamageFactor[] damageFactors, StatusIdentity statusIdentity,
            Action<IEntityData> onTriggeredEntered = null, Action<IEntityData> onTriggeredExit = null)
        {
            damageBox.gameObject.SetActive(true);
            damageBox.StartDamage(creatorData, effectSource, effectProperty, damageBonus, damageFactors, statusIdentity, onTriggeredEntered, onTriggeredExit);
        }

        private void OnTurnOffDamageBox()
        {
            damageBox.gameObject.SetActive(false);
        }
    }
}