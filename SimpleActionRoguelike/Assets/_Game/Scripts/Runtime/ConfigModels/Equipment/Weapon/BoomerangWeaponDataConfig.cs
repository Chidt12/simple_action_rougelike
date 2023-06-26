using CsvReader;
using Cysharp.Threading.Tasks;
using Runtime.Definition;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class BoomerangEquipmentMechanicDataConfigItem : EquipmentMechanicDataConfigItem
    {
        public int bonusProjectiles;
        public StatusIdentity buffStatusIdentity;
        public int buffFlySpeed;
        public bool goThrough;
    }

    [Serializable]
    public class BoomerangWeaponDataConfigItem : WeaponDataConfigItem<BoomerangEquipmentMechanicDataConfigItem>
    {
        public string projectileId;
        public float projectileSpeed;

        public override WeaponType WeaponType => WeaponType.Boomerang;
    }

    public class BoomerangWeaponDataConfig : WeaponDataConfig<BoomerangWeaponDataConfigItem, BoomerangEquipmentMechanicDataConfigItem>
    {
        public override WeaponType WeaponType => WeaponType.Boomerang;

        protected override BoomerangEquipmentMechanicDataConfigItem Add(BoomerangEquipmentMechanicDataConfigItem item1, BoomerangEquipmentMechanicDataConfigItem item2)
        {
            item1.bonusProjectiles += item2.bonusProjectiles;
            item1.buffStatusIdentity = item2.buffStatusIdentity.statusType != StatusType.None ? item2.buffStatusIdentity : item1.buffStatusIdentity;
            item1.buffFlySpeed = item2.buffFlySpeed;
            item1.goThrough = item1.goThrough || item2.goThrough;
            return item1;
        }

        protected override UniTask<string> GetDescription(RarityType rarityType, BoomerangWeaponDataConfigItem itemData, BoomerangEquipmentMechanicDataConfigItem mechanicData, BoomerangEquipmentMechanicDataConfigItem previousMechanicData)
        {
            string increaseWaveFormat = "You can use {0} more each time";
            string goThroughFormat = "The boomerang go through obstacles";
            string buffSpeedFormat = "The boomerang speed go faster";
            string statusFormat = "have chance to {0} target";

            switch (rarityType)
            {
                case RarityType.Common:
                    return UniTask.FromResult(string.Format(increaseWaveFormat, mechanicData.bonusProjectiles));
                case RarityType.Rare:
                    return UniTask.FromResult(string.Format(increaseWaveFormat, mechanicData.bonusProjectiles));
                case RarityType.Epic:
                    return UniTask.FromResult(string.Format(increaseWaveFormat, mechanicData.bonusProjectiles));
                case RarityType.Unique:
                    return UniTask.FromResult(goThroughFormat);
                case RarityType.Legendary:
                    return UniTask.FromResult(string.Format(statusFormat, mechanicData.buffStatusIdentity.statusType));
                case RarityType.Ultimate:
                    return UniTask.FromResult(buffSpeedFormat);
                default:
                    return UniTask.FromResult(string.Empty);
            }
        }
    }
}
