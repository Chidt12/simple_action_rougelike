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
        private List<EntityModel> _entitiesModels;
        private MapEditorEntity[] _mapEditorEntities;

        public EntityModel HeroModel { get; private set; }
        public List<EntityModel> EntitiesModels => _entitiesModels;
        public int DefeatedEnemiesCount => _defeatedEnemiesCount;

        public void Initialize()
        {
            _defeatedEnemiesCount = 0;
            _entityUId = 0;
            HeroModel = null;
            _entitiesModels = new();
            _mapEditorEntities = FindObjectsOfType<MapEditorEntity>(true);
        }

        public void CollectAllCurrentEntities()
        {
            var entityHolders = FindObjectsOfType<EntityHolder>();
            foreach (var holder in entityHolders)
            {
                if(_entitiesModels.Any(x => x.EntityUID != holder.EntityModel.EntityUID))
                    _entitiesModels.Add(holder.EntityModel);

                if (holder.EntityModel.EntityType == Definition.EntityType.Hero)
                {
                    HeroModel = holder.EntityModel;
                    SimpleMessenger.Publish(new SpawnedHeroMessage());
                }
            }
        }
    }
}
