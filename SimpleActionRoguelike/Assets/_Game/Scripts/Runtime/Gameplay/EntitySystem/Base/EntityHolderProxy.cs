using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityHolderProxy : MonoBehaviour, IEntityHolder
    {
        [SerializeField] private EntityHolder _entityHolder;

        public IEntityData EntityData => _entityHolder.EntityData;

        public bool IsProxy => true;

        public UniTask<bool> BuildAsync(IEntityData entityModel) => UniTask.FromResult(true);

        public void Dispose()
        {
        }
    }

}