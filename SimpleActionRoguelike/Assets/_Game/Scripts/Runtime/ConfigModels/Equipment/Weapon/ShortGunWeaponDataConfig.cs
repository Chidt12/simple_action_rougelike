using Cysharp.Threading.Tasks;
using Runtime.Definition;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class ShortGunEquipmentMechanicDataConfigItem : EquipmentMechanicDataConfigItem 
    {
        public int bonusProjectileVertical;
        public int bonusProjectileHorizontal;
        public bool goThrough;
    }

    [Serializable]
    public class ShortGunWeaponDataConfigItem : WeaponDataConfigItem<ShortGunEquipmentMechanicDataConfigItem>
    {
        public string projectileId;
        public float projectileSpeed;
        public override WeaponType WeaponType => WeaponType.ShortGun;
    }

    public class ShortGunWeaponDataConfig : WeaponDataConfig<ShortGunWeaponDataConfigItem, ShortGunEquipmentMechanicDataConfigItem>
    {
        public override WeaponType WeaponType => WeaponType.ShortGun;

        protected override ShortGunEquipmentMechanicDataConfigItem Add(ShortGunEquipmentMechanicDataConfigItem item1, ShortGunEquipmentMechanicDataConfigItem item2)
        {
            item1.bonusProjectileHorizontal += item2.bonusProjectileHorizontal;
            item1.bonusProjectileVertical += item2.bonusProjectileVertical;
            item1.goThrough = item1.goThrough || item2.goThrough;
            return item1;
        }    

        protected override UniTask<string> GetDescription(RarityType rarityType, ShortGunWeaponDataConfigItem itemData, ShortGunEquipmentMechanicDataConfigItem mechanicData, ShortGunEquipmentMechanicDataConfigItem previousMechanicData)
        {
            string increaseWaveFormat = "The shortgun fire {0} more wave each shot";
            string increaseProjectilesEachWaveFormat = "The shortgun fire {0} more each wave";
            string goThroughFormat = "The shortgun's projectiles go through obstacles";
            switch (rarityType)
            {
                case RarityType.Common:
                    return UniTask.FromResult(string.Format(increaseWaveFormat, mechanicData.bonusProjectileVertical)); // + 1;
                case RarityType.Rare:
                    return UniTask.FromResult(string.Format(increaseWaveFormat, mechanicData.bonusProjectileVertical)); // + 1;
                case RarityType.Epic:
                    return UniTask.FromResult(string.Format(increaseWaveFormat, mechanicData.bonusProjectileVertical)); // + 1;
                case RarityType.Unique:
                    return UniTask.FromResult(goThroughFormat);
                case RarityType.Legendary:
                    return UniTask.FromResult(string.Format(increaseProjectilesEachWaveFormat, mechanicData.bonusProjectileHorizontal)); // + 2;
                case RarityType.Ultimate:
                    return UniTask.FromResult(string.Format(increaseProjectilesEachWaveFormat, mechanicData.bonusProjectileHorizontal)); // + 2;
                default:
                    return UniTask.FromResult(string.Empty);
            }
        }
    }
}
