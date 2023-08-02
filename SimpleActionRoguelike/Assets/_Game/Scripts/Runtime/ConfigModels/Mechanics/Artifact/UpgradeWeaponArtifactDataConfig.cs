using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Localization;
using Runtime.Manager.Data;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class UpgradeWeaponArtifactDataConfigItem : ArtifactDataConfigItem
    {
        public RarityType rarityType;

        public override ArtifactType ArtifactType => ArtifactType.UpgradeWeapon;
    }

    public class UpgradeWeaponArtifactDataConfig : ArtifactDataConfig<UpgradeWeaponArtifactDataConfigItem>
    {
        protected override async UniTask<(string, string)> GetDescription(IEntityData entityData, UpgradeWeaponArtifactDataConfigItem itemData, UpgradeWeaponArtifactDataConfigItem previousItemData)
        {
            var currentDescription = await LocalizeManager.GetLocalizeAsync(LocalizeTable.ARTIFACT, LocalizeKeys.GetArtifactDescription(itemData.ArtifactType));
            var previousDescription = previousItemData != null ?
                await LocalizeManager.GetLocalizeAsync(LocalizeTable.ARTIFACT, LocalizeKeys.GetArtifactDescription(previousItemData.ArtifactType))
                : string.Empty;
            return (currentDescription, previousDescription);
        }
    }
}
