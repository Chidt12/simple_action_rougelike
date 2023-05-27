using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Singleton;
using Runtime.Definition;
using Runtime.Manager.Data;
using System;
using System.Collections.Generic;

namespace Runtime.Gameplay
{
    [Serializable]
    public struct EquipmentEquippedData
    {
        #region Members

        public EquipmentType EquipmentType;
        public RarityType RarityType;
        public int EquipmentId;
        public int Level;
        public int Star;

        #endregion Members

        #region Struct Methods

        public EquipmentEquippedData(EquipmentType equipmentType, RarityType rarityType, int equipmentId, int level, int star)
        {
            EquipmentType = equipmentType;
            RarityType = rarityType;
            EquipmentId = equipmentId;
            Level = level;
            Star = star;
        }

        #endregion Struct Methods
    }

    public abstract class GameplayDataDispatcher : MonoSingleton<GameplayDataDispatcher>
    {
        public abstract int HeroLevel { get; }

        public abstract Dictionary<EquipmentType, EquipmentEquippedData> SelectedEquipments { get; }

        public abstract UniTask<HeroStatsInfo> GetHeroStatsInfo(CharacterLevelStats heroLevelStats);
    }
}
