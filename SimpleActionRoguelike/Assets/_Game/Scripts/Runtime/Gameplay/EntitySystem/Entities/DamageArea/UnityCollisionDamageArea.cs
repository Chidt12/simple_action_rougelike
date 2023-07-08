using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using Runtime.Message;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [RequireComponent(typeof(Collider2D))]
    public class UnityCollisionDamageArea : DamageArea
    {
        [SerializeField] private Collider2D _collider;

        private void OnEnable()
        {
            _collider.enabled = false;
        }

        public async override UniTask BuildAsync(IEntityData creatorData, Vector3 position, DamageAreaData data)
        {
            await base.BuildAsync(creatorData, position, data);
            _collider.enabled = true;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            var entity = collider.GetComponent<IEntityHolder>();
            if (entity != null && !entity.EntityData.IsDead)
            {
                if (data.creatorData.EntityType.CanCauseDamage(entity.EntityData.EntityType) && !damagedTargets.Contains(entity.EntityData))
                {
                    damagedTargets.Add(entity.EntityData);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            var entity = collider.GetComponent<IEntityHolder>();
            if (entity != null)
            {
                if (damagedTargets.Contains(entity.EntityData))
                    damagedTargets.Remove(entity.EntityData);
            }
        }
    }
}