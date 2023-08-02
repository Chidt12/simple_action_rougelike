using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Localization;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class CreateBuffFlagArtifactDataConfigItem : RuneArtifactDataConfigItem
    {
        public override ArtifactType ArtifactType => ArtifactType.CreateBuffFlag;
        public float rangeWidth;
        public float rangeHeight;
        public EquipmentStat[] buffStats;
        public string flagPrefabName;
        public string buffVfxPrefabName;
    }

    public class CreateBuffFlagArtifactDataConfig : ArtifactDataConfig<CreateBuffFlagArtifactDataConfigItem>
    {
        protected async override UniTask<(string, string)> GetDescription(IEntityData entityData, CreateBuffFlagArtifactDataConfigItem itemData, CreateBuffFlagArtifactDataConfigItem previousItemData)
        {
            var currentDescription = await LocalizeManager.GetLocalizeAsync(LocalizeTable.ARTIFACT, LocalizeKeys.GetArtifactDescription(itemData.ArtifactType));
            var previousDescription = previousItemData != null ?
                await LocalizeManager.GetLocalizeAsync(LocalizeTable.ARTIFACT, LocalizeKeys.GetArtifactDescription(previousItemData.ArtifactType))
                : string.Empty;
            return (currentDescription, previousDescription);
        }
    }
}
