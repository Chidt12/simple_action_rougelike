using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class UpgradeWeaponBuffInGameSystem : BuffInGameSystem<UpgradeWeaponBuffInGameDataConfigItem>
    {
        public override BuffInGameType BuffInGameType => BuffInGameType.UpgradeWeapon;

        public async override UniTask Init(IEntityData entityData)
        {
            await base.Init(entityData);
            var weaponData = entityData as IEntityWeaponData;
            if(weaponData != null)
            {
                var equipmentEquip = GameplayDataDispatcher.Instance.SelectedEquipments[EquipmentType.Weapon];
                var weaponType = (WeaponType)equipmentEquip.EquipmentId;
                var weaponConfig = await GameplayDataManager.Instance.GetWeaponDataAsync(weaponType, ownerData.rarityType, equipmentEquip.Level);
                var weaponModel = WeaponModelFactory.GetWeaponModel(weaponType, weaponConfig);
                weaponData.InitWeapon(weaponModel);
            }
        }
    }

}