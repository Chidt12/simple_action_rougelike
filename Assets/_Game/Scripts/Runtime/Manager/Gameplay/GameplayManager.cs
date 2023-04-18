using Cysharp.Threading.Tasks;
using Runtime.Core.Singleton;
using Runtime.Gameplay.EntitySystem;
using UnityEngine;

namespace Runtime.Manager.Gameplay
{
    public class GameplayManager : MonoSingleton<GameplayManager>
    {
        [SerializeField]
        private EntityHolder heroHolder;
        [SerializeField]
        private EntityHolder enemyHolder;


        private void Start()
        {
            InitGameAsync().Forget();
        }

        private async UniTaskVoid InitGameAsync()
        {
            EntitiesManager.Instance.Initialize();

            var heroModel = new EntityModel();

            var enemyModel = new EntityModel();
            enemyModel.Init(Definition.EntityType.Enemy, 2);
            if(enemyHolder)
                await enemyHolder.BuildAsync(enemyModel);

            heroModel.Init(Definition.EntityType.Hero, 1);
            if(heroHolder)
                await heroHolder.BuildAsync(heroModel);

            EntitiesManager.Instance.CollectAllCurrentEntities();
        }

    }

}