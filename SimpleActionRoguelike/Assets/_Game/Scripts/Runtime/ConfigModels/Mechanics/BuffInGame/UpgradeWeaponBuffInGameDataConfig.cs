using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Manager.Data;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class UpgradeWeaponBuffInGameDataConfigItem : BuffInGameDataConfigItem
    {
        public RarityType rarityType;

        public override BuffInGameType BuffInGameType => BuffInGameType.UpgradeWeapon;
    }

    public class UpgradeWeaponBuffInGameDataConfig : BuffInGameDataConfig<UpgradeWeaponBuffInGameDataConfigItem>
    {
        protected override async UniTask<string> GetDescription(IEntityData entityData, UpgradeWeaponBuffInGameDataConfigItem itemData, UpgradeWeaponBuffInGameDataConfigItem previousItemData)
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
