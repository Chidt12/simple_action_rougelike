using CsvReader;
using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class RotateOrbsArtifactDataConfigItem : ArtifactDataConfigItem
    {
        public string orbPrefabName;
        public int numberOfOrbs;
        public float flyRange;
        public float rotateSpeed;
        public float orbDamageBonus;
        [CsvColumnFormat(ColumnFormat = "orb_{0}")]
        public DamageFactor[] orbDamageFactors;
        public float projectileDamageBonus;
        public DamageFactor[] projectileDamageFactors;
        public float flySpeed;

        public override ArtifactType ArtifactType => ArtifactType.RotateOrbs;
    }

    public class RotateOrbsArtifactDataConfig : ArtifactDataConfig<RotateOrbsArtifactDataConfigItem>
    {
        protected override UniTask<string> GetDescription(IEntityData entityData, RotateOrbsArtifactDataConfigItem itemData, RotateOrbsArtifactDataConfigItem previousItemData)
        {
            if (itemData != null)
            {
                var format = "Create {0} that rotate around hero";
                var description = string.Format(format, itemData.numberOfOrbs);
                return UniTask.FromResult(description);
            }
            return UniTask.FromResult(string.Empty);
        }
    }
}