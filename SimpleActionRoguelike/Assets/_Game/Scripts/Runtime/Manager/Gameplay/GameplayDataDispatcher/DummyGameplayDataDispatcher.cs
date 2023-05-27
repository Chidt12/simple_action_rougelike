using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Definition;
using Runtime.Manager.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.Manager
{
    public class DummyGameplayDataDispatcher : GameplayDataDispatcher
    {
        [SerializeField]
        private int _heroLevel;
        [SerializeField]
        private WeaponType _weaponType;
        [SerializeField]
        private RarityType _weaponRarity = RarityType.Ultimate;
        [SerializeField]
        private int _weaponLevel = 30;
        [SerializeField]
        private int _weaponStar = 3;
        [SerializeField]
        private EquipmentEquippedData[] _equipments;

        public override int HeroLevel => _heroLevel;

        public override Dictionary<EquipmentType, EquipmentEquippedData> SelectedEquipments
        {
            get
            {
                Dictionary<EquipmentType, EquipmentEquippedData> selectedEquipments = new();
                selectedEquipments.Add(EquipmentType.Weapon, new EquipmentEquippedData(EquipmentType.Weapon, _weaponRarity, (int)_weaponType, _weaponLevel, _weaponStar));

                foreach (var equipment in _equipments)
                    selectedEquipments.Add(equipment.EquipmentType, equipment);

                return selectedEquipments;
            }
        }

        public override UniTask<HeroStatsInfo> GetHeroStatsInfo(CharacterLevelStats heroLevelStats)
        {
            var heroStats = new HeroStatsInfo(heroLevelStats);
            return UniTask.FromResult(heroStats);
        }
    }

}