using CsvReader;
using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class RotateOrbsBuffInGameDataConfigItem : BuffInGameDataConfigItem
    {
        public string orbPrefabName;
        public int numberOfOrbs;
        public float flyRange;
        public float rotateSpeed;
        public float orbDamageBonus;
        [CsvColumnFormat(ColumnFormat = "orb_{0}")]
        public DamageFactor[] orbDamageFactors;

        public override BuffInGameType BuffInGameType => BuffInGameType.RotateOrbs;
    }

    public class RotateOrbsBuffInGameDataConfig : BuffInGameDataConfig<RotateOrbsBuffInGameDataConfigItem>
    {
        protected override UniTask<string> GetDescription(IEntityData entityData, RotateOrbsBuffInGameDataConfigItem itemData, RotateOrbsBuffInGameDataConfigItem previousItemData)
        {
            if (itemData != null)
            {
                var format = "Create {0} that rotate around hero";
                var description = string.Format(format, itemData.numberOfOrbs);
                return UniTask.FromResult(description);
            }
            return UniTask.FromResult(string.Empty);
        }
    }
}