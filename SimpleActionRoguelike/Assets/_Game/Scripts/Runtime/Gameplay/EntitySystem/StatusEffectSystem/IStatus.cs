using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IStatus
    {
        public StatusModel OwnerModel { get; }
        public StatusType StatusType { get; }
        public IEntityData Creator { get; }
        public IEntityStatusData Owner { get; }
        public void Init(StatusModel statusModel, IEntityData creator, IEntityStatusData owner, StatusMetaData metaData);
        public abstract void Dispose();
    }

    public struct StatusMetaData
    {
        #region Members

        public Vector2 statusEffectDirection;
        public Vector2 statusEffectAttractedPoint;

        #endregion Members

        #region Struct Methods

        public StatusMetaData(DamageMetaData damageMetaData)
        {
            statusEffectDirection = damageMetaData.damageDirection;
            statusEffectAttractedPoint = damageMetaData.attractedPoint;
        }

        public StatusMetaData(Vector2 statusEffectDirection, Vector2 statusEffectAttractedPoint)
        {
            this.statusEffectDirection = statusEffectDirection;
            this.statusEffectAttractedPoint = statusEffectAttractedPoint;
        }

        #endregion Struct Methods
    }
}
