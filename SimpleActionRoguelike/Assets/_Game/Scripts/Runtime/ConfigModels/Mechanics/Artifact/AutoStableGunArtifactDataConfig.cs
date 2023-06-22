using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class AutoStableGunArtifactDataConfigItem : RuneArtifactDataConfigItem
    {
        public override ArtifactType ArtifactType => ArtifactType.AutoStableGun;
        public float interval;
        public float lifeTime;
        public float damageBonus;
        public DamageFactor[] damageFactors;
        public float range;
        public string projectileId;
        public float projectileSpeed;
        public string gunPrefabName;
        
    }

    public class AutoStableGunArtifactDataConfig : ArtifactDataConfig<AutoStableGunArtifactDataConfigItem>
    {
        protected override UniTask<string> GetDescription(IEntityData entityData, AutoStableGunArtifactDataConfigItem itemData, AutoStableGunArtifactDataConfigItem previousItemData)
        {
            return UniTask.FromResult(string.Empty);
        }
    }
}