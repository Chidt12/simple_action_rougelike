using Cysharp.Threading.Tasks;
using Runtime.Definition;
using System.Linq;
using UnityEngine;

namespace Runtime.ConfigModel
{
    public abstract class ShopInGameDataConfigItem : BaseWithPointConfigItem
    {
        public int dataId;
        public abstract ShopInGameItemType ShopInGameType { get; }
    }

    public abstract class ShopInGameDataConfig : ScriptableObject
    {
        public abstract ShopInGameDataConfigItem GetItem(int dataId);
        public abstract UniTask<(string, string)> GetDescription(int dataId);
    }

    public abstract class ShopInGameDataConfig<T> : ShopInGameDataConfig where T : ShopInGameDataConfigItem, new()
    {
        public T[] items;

        public override ShopInGameDataConfigItem GetItem(int dataId)
        {
            return items.FirstOrDefault(x => x.dataId == dataId);
        }

        public async override UniTask<(string, string)> GetDescription(int dataId)
        {
            var item = GetItem(dataId) as T;
            return await GetDescription(item);
        }

        protected abstract UniTask<(string, string)> GetDescription(T itemData);
    }
}