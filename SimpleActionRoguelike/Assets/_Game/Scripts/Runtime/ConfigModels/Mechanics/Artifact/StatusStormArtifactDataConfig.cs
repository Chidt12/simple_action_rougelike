using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Localization;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class StatusStormArtifactDataConfigItem : RuneArtifactDataConfigItem
    {
        public override ArtifactType ArtifactType => ArtifactType.StatusStorm;
        public StatusIdentity triggeredStatus;
        public float damageBonus;
        public DamageFactor[] damageFactors;
        public float range;
        public float delayBetweenLightning;
        public float numberOfLightning;
        public string ligntningPrefab;
    }

    public class StatusStormArtifactDataConfig : ArtifactDataConfig<StatusStormArtifactDataConfigItem>
    {
        protected override async UniTask<(string, string)> GetDescription(IEntityData entityData, StatusStormArtifactDataConfigItem itemData, StatusStormArtifactDataConfigItem previousItemData)
        {
            var currentDescription = await LocalizeManager.GetLocalizeAsync(LocalizeTable.ARTIFACT, LocalizeKeys.GetArtifactDescription(itemData.ArtifactType));
            var previousDescription = await LocalizeManager.GetLocalizeAsync(LocalizeTable.ARTIFACT, LocalizeKeys.GetArtifactDescription(previousItemData.ArtifactType));
            return (currentDescription, previousDescription);
        }
    }
}