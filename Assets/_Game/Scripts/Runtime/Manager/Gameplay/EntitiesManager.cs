using Runtime.Core.Message;
using Runtime.Core.Singleton;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntitiesManager : MonoSingleton<EntitiesManager>
    {
        private uint _entityUId;
        private int _defeatedEnemiesCount;
        private List<IEntityData> _entitiesData;

        public IEntityData HeroData { get; private set; }
        public List<IEntityData> EntitiesData => _entitiesData;
        public int DefeatedEnemiesCount => _defeatedEnemiesCount;

        public void Initialize()
        {
            _defeatedEnemiesCount = 0;
            _entityUId = 0;
            HeroData = null;
            _entitiesData = new();
        }

        public void CollectAllCurrentEntities()
        {
            var entityHolders = FindObjectsOfType<EntityHolder>();
            foreach (var holder in entityHolders)
            {
                if(_entitiesData.Any(x => x.EntityUID != holder.EntityData.EntityUID))
                    _entitiesData.Add(holder.EntityData);

                if (holder.EntityData.EntityType == Definition.EntityType.Hero)
                {
                    HeroData = holder.EntityData;
                    SimpleMessenger.Publish(new SpawnedHeroMessage());
                }
            }
        }
    }
}
