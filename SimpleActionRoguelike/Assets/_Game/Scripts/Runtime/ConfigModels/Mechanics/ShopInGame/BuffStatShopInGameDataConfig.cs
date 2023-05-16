using Cysharp.Threading.Tasks;
using Runtime.Definition;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class BuffStatShopInGameDataConfigItem : ShopInGameDataConfigItem
    {
        public override ShopInGameItemType ShopInGameType => ShopInGameItemType.BuffStat;
        public StatType statType;
        public float statValue;
        public StatModifyType statModifyType;
    }

    public class BuffStatShopInGameDataConfig : ShopInGameDataConfig<BuffStatShopInGameDataConfigItem>
    {
        protected override UniTask<string> GetDescription(BuffStatShopInGameDataConfigItem itemData)
        {
            var isPercentValue = itemData.statType.IsPercentValue() || itemData.statModifyType.IsPercentValue();
            var description = $"{itemData.statType} +{(isPercentValue ? itemData.statValue * 100 : itemData.statValue)} {(isPercentValue?"%":"")}";
            return UniTask.FromResult(description);
        }
    }
}
