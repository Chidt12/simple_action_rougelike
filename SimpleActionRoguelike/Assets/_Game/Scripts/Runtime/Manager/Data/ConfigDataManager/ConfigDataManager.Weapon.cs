using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Definition;

namespace Runtime.Manager.Data
{
    public partial class ConfigDataManager
    {
        public async UniTask<EquipmentMechanicDataConfig> LoadWeaponConfigItem(WeaponType weaponType)
        {
            var weaponConfig = await Load<WeaponDataConfig>(string.Format(AddressableKeys.WEAPON_DATA_CONFIG_ASSET_FORMAT, weaponType));
            return weaponConfig;
        }

        public async UniTask<EquipmentMechanicDataConfigItem> LoadWeaponMechanicConfigItem(WeaponType weaponType, RarityType rarityType)
        {
            var weaponConfig = await Load<WeaponDataConfig>(string.Format(AddressableKeys.WEAPON_DATA_CONFIG_ASSET_FORMAT, weaponType));
            var weaponConfigItem = weaponConfig.GetEquipmentMechanicDataConfigItem(rarityType);
            return weaponConfigItem;
        }

        public async UniTask<IWeaponDataConfigItem> LoadWeaponDataConfigItem(WeaponType weaponType)
        {
            var weaponConfig = await Load<WeaponDataConfig>(string.Format(AddressableKeys.WEAPON_DATA_CONFIG_ASSET_FORMAT, weaponType));
            var weaponDataConfigItem = weaponConfig.GetWeaponDataConfigItem();
            return weaponDataConfigItem;
        }
    }
}
