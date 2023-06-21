using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using System.Linq;
using UnityEngine;

namespace Runtime.ConfigModel
{
    public abstract class ArtifactDataConfigItem : BaseWithPointConfigItem
    {
        public int level;
        public abstract ArtifactType BuffInGameType { get; }
    }

    public abstract class ArtifactDataConfig : ScriptableObject
    {
        public abstract ArtifactDataConfigItem GetBuffItem(int level);
        public abstract UniTask<string> GetDescription(IEntityData entityData, int level);
    }

    public abstract class ArtifactDataConfig<T> : ArtifactDataConfig  where T : ArtifactDataConfigItem, new()
    {
        public T[] items;

        public override ArtifactDataConfigItem GetBuffItem(int level)
        {
            return items.FirstOrDefault(x => x.level == level);
        }

        public async override UniTask<string> GetDescription(IEntityData entityData, int level)
        {
            var item = GetBuffItem(level) as T;
            if(level > 0)
            {
                var previousItem = GetBuffItem(level - 1) as T;
                return await GetDescription(entityData, item, previousItem);
            }
            return await GetDescription(entityData, item, null);
        }

        protected abstract UniTask<string> GetDescription(IEntityData entityData, T itemData, T previousItemData);
    }
}
