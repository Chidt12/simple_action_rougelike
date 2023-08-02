using CsvReader;
using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Localization;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class RotateOrbsArtifactDataConfigItem : ArtifactDataConfigItem
    {
        public string orbPrefabName;
        public int numberOfOrbs;
        public float rotateRange;
        public float rotateSpeed;
        public float orbDamageBonus;
        [CsvColumnFormat(ColumnFormat = "orb_{0}")]
        public DamageFactor[] orbDamageFactors;
        public float projectileDamageBonus;
        [CsvColumnFormat(ColumnFormat = "projectile_{0}")]
        public DamageFactor[] projectileDamageFactors;
        public float flySpeed;
        public float flyRange;

        public override ArtifactType ArtifactType => ArtifactType.RotateOrbs;
    }

    public class RotateOrbsArtifactDataConfig : ArtifactDataConfig<RotateOrbsArtifactDataConfigItem>
    {
        protected async override UniTask<(string, string)> GetDescription(IEntityData entityData, RotateOrbsArtifactDataConfigItem itemData, RotateOrbsArtifactDataConfigItem previousItemData)
        {
            var currentDescription = await LocalizeManager.GetLocalizeAsync(LocalizeTable.ARTIFACT, LocalizeKeys.GetArtifactDescription(itemData.ArtifactType));
            var previousDescription = previousItemData != null ?
                await LocalizeManager.GetLocalizeAsync(LocalizeTable.ARTIFACT, LocalizeKeys.GetArtifactDescription(previousItemData.ArtifactType))
                : string.Empty;
            return (currentDescription, previousDescription);
        }
    }
}