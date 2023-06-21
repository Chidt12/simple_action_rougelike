using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Manager.Data;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class UpgradeWeaponArtifactDataConfigItem : ArtifactDataConfigItem
    {
        public RarityType rarityType;

        public override ArtifactType BuffInGameType => ArtifactType.UpgradeWeapon;
    }

    public class UpgradeWeaponArtifactDataConfig : ArtifactDataConfig<UpgradeWeaponArtifactDataConfigItem>
    {
        protected override async UniTask<string> GetDescription(IEntityData entityData, UpgradeWeaponArtifactDataConfigItem itemData, UpgradeWeaponArtifactDataConfigItem previousItemData)
        {
            var weaponData = entityData as IEntityWeaponData;
            if (weaponData != null)
            {
                var weaponDataConfigItem = await DataManager.Config.LoadWeaponConfigItem(weaponData.WeaponModel.WeaponType);
                var description = await weaponDataConfigItem.GetDescription(itemData.rarityType);
                return description;
            }
            return string.Empty;
        }
    }
}
