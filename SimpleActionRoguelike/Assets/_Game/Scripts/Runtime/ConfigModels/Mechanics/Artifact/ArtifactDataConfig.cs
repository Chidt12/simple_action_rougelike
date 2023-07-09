using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using System.Linq;
using UnityEngine;

namespace Runtime.ConfigModel
{
    public abstract class RuneArtifactDataConfigItem: ArtifactDataConfigItem
    {
        public float runeLifeTime;
        public float runeInterval;
    }

    public abstract class ArtifactDataConfigItem : BaseWithPointConfigItem
    {
        public int dataId;
        public int level;
        public abstract ArtifactType ArtifactType { get; }
    }

    public abstract class ArtifactDataConfig : ScriptableObject
    {
        public abstract ArtifactDataConfigItem GetArtifactItem(int level, int dataId);
        public abstract UniTask<string> GetDescription(IEntityData entityData, int level, int dataId);
    }

    public abstract class ArtifactDataConfig<T> : ArtifactDataConfig  where T : ArtifactDataConfigItem, new()
    {
        public T[] items;

        public override ArtifactDataConfigItem GetArtifactItem(int level, int dataId)
        {
            return items.FirstOrDefault(x => x.level == level && x.dataId == dataId);
        }

        public async override UniTask<string> GetDescription(IEntityData entityData, int level, int dataId)
        {
            var item = GetArtifactItem(level, dataId) as T;
            if(level > 0)
            {
                var previousItem = GetArtifactItem(level - 1, dataId) as T;
                return await GetDescription(entityData, item, previousItem);
            }
            return await GetDescription(entityData, item, null);
        }

        protected abstract UniTask<string> GetDescription(IEntityData entityData, T itemData, T previousItemData);
    }
}
