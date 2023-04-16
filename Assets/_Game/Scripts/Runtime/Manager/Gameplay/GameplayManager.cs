using Cysharp.Threading.Tasks;
using Runtime.Core.Singleton;
using Runtime.Gameplay.EntitySystem;
using UnityEngine;

namespace Runtime.Manager.Gameplay
{
    public class GameplayManager : MonoSingleton<GameplayManager>
    {
        [SerializeField]
        private EntityHolder[] entityHolder;


        private void Start()
        {
            foreach (var item in entityHolder)
            {
                var entityModel = new EntityModel();
                entityModel.Init();
                item.BuildAsync(entityModel).Forget();
            }
        }

    }

}