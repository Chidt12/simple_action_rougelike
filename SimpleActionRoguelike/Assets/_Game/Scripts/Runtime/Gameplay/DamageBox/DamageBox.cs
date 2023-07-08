using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Message;
using Runtime.Gameplay.EntitySystem;
using Runtime.Message;
using System;
using UnityEngine;

namespace Runtime.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class DamageBox : MonoBehaviour
    {
        [SerializeField] protected Collider2D collider2D;

        protected EffectSource effectSource;
        protected EffectProperty effectProperty;
        protected float damageBonus;
        protected DamageFactor[] damageFactors;
        protected IEntityData creatorData;
        protected StatusIdentity statusIdentity;

        protected Action<IEntityData> onTriggeredEntered;
        protected Action<IEntityData> onTriggeredExit;
        protected bool isInited;

        public Vector2 CenterBoxPosition => collider2D.bounds.center;

        private void OnDisable()
        {
            isInited = false;
        }

        private void OnEnable()
        {
            collider2D.enabled = false;
        }

        public void StartDamage(
            IEntityData creatorData, EffectSource effectSource, EffectProperty effectProperty, float damageBonus, DamageFactor[] damageFactors, StatusIdentity statusIdentity,
            Action<IEntityData> onTriggeredEntered = null, Action<IEntityData> onTriggeredExit = null)
        {
            isInited = true;
            this.onTriggeredEntered = onTriggeredEntered;
            this.onTriggeredExit = onTriggeredExit;
            collider2D.enabled = true;

            this.creatorData = creatorData;
            this.effectSource = effectSource;
            this.effectProperty = effectProperty;
            this.damageBonus = damageBonus;
            this.damageFactors = damageFactors;
            this.statusIdentity = statusIdentity;
        }

        public void ScaleSize(Vector2 scale)
        {
            transform.localScale = scale;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isInited)
                return;

            var entityHolder = collision.GetComponent<IEntityHolder>();
            if (entityHolder != null && entityHolder.EntityData != null && !entityHolder.EntityData.IsDead && creatorData.EntityType.CanCauseDamage(entityHolder.EntityData.EntityType))
            {
                SimpleMessenger.PublishAsync(MessageScope.EntityMessage,
                    new SentDamageMessage(EffectSource.FromArtifact, EffectProperty.Normal,
                    damageBonus, damageFactors, creatorData, entityHolder.EntityData)).Forget();

                var targetStatusData = entityHolder.EntityData as IEntityStatusData;
                if(targetStatusData != null && statusIdentity.statusType != Definition.StatusType.None)
                    SimpleMessenger.PublishAsync(MessageScope.EntityMessage,
                        new SentStatusEffectMessage(creatorData, targetStatusData, statusIdentity)).Forget();

                onTriggeredEntered?.Invoke(entityHolder.EntityData);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!isInited)
                return;

            var entityHolder = collision.GetComponent<IEntityHolder>();
            if (entityHolder != null && entityHolder.EntityData != null && !entityHolder.EntityData.IsDead)
            {
                onTriggeredExit?.Invoke(entityHolder.EntityData);
            }
        }
    }
}