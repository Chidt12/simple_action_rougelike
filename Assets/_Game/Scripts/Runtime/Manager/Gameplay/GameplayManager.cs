using Cysharp.Threading.Tasks;
using Runtime.Core.Singleton;
using Runtime.Gameplay.EntitySystem;
using UnityEngine;

namespace Runtime.Manager.Gameplay
{
    public class GameplayManager : MonoSingleton<GameplayManager>
    {
        [SerializeField]
        private EntityHolder entityHolder;


        private void Start()
        {
            var entityModel = new EntityModel();
            entityModel.Init();
            entityHolder.BuildAsync(entityModel).Forget();
        }

    }

}