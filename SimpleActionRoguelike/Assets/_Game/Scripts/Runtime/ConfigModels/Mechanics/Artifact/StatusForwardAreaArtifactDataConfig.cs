using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Localization;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class StatusForwardAreaArtifactDataConfigItem : RuneArtifactDataConfigItem
    {
        public override ArtifactType ArtifactType => ArtifactType.StatusForwardArea;
        public float damageBonus;
        public DamageFactor[] damageFactors;
        public StatusIdentity triggeredStatus;
        public float distanceBetweenObject;
        public float numberOfObjects;
        public string forwardPrefabName;
        public float delayBetweenSpawn;
    }

    public class StatusForwardAreaArtifactDataConfig : ArtifactDataConfig<StatusForwardAreaArtifactDataConfigItem>
    {
        protected override async UniTask<(string, string)> GetDescription(IEntityData entityData, StatusForwardAreaArtifactDataConfigItem itemData, StatusForwardAreaArtifactDataConfigItem previousItemData)
        {
            var currentDescription = await LocalizeManager.GetLocalizeAsync(LocalizeTable.ARTIFACT, LocalizeKeys.GetArtifactDescription(itemData.ArtifactType));
            var previousDescription = previousItemData != null ?
                await LocalizeManager.GetLocalizeAsync(LocalizeTable.ARTIFACT, LocalizeKeys.GetArtifactDescription(previousItemData.ArtifactType))
                : string.Empty;
            return (currentDescription, previousDescription);
        }
    }
}