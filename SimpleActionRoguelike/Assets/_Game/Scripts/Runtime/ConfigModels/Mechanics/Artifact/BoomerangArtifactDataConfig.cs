using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Localization;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class BoomerangArtifactDataConfigItem : RuneArtifactDataConfigItem
    {
        public override ArtifactType ArtifactType => ArtifactType.Boomerang;

        public int numberOfHits;
        public float damageBonus;
        public DamageFactor[] damageFactors;
        public StatusIdentity triggeredStatus;
        public float flySpeed;
        public float flyDistance;
        public string projectileId;
    }

    public class BoomerangArtifactDataConfig : ArtifactDataConfig<BoomerangArtifactDataConfigItem>
    {
        protected async override UniTask<(string, string)> GetDescription(IEntityData entityData, BoomerangArtifactDataConfigItem itemData, BoomerangArtifactDataConfigItem previousItemData)
        {
            var currentDescription = await LocalizeManager.GetLocalizeAsync(LocalizeTable.ARTIFACT, LocalizeKeys.GetArtifactDescription(itemData.ArtifactType));
            var previousDescription = previousItemData != null ?
                 await LocalizeManager.GetLocalizeAsync(LocalizeTable.ARTIFACT, LocalizeKeys.GetArtifactDescription(previousItemData.ArtifactType))
                 : string.Empty;
            return (currentDescription, previousDescription);
        }
    }
}