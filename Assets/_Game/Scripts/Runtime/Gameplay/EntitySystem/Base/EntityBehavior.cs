using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class EntityBehavior : MonoBehaviour, IEntityBehavior
    {
        public abstract UniTask<bool> BuildAsync(IEntityData data);
    }

    public abstract class EntityBehavior<T1> : EntityBehavior where T1 : IEntityData
    {
        public override UniTask<bool> BuildAsync(IEntityData data)
        {
            return BuildDataAsync((T1)data);
        }

        protected abstract UniTask<bool> BuildDataAsync(T1 data);
    }

    public abstract class EntityBehavior<T1, T2> : EntityBehavior where T1 : IEntityData 
                                                                  where T2 : IEntityData
    {
        public override UniTask<bool> BuildAsync(IEntityData data)
        {
            return BuildDataAsync((T1)data, (T2)data);
        }

        protected abstract UniTask<bool> BuildDataAsync(T1 data, T2 data2);
    }

    public abstract class EntityBehavior<T1, T2, T3> : EntityBehavior where T1 : IEntityData
                                                                      where T2 : IEntityData
                                                                      where T3 : IEntityData
    {
        public override UniTask<bool> BuildAsync(IEntityData data)
        {
            return BuildDataAsync((T1)data, (T2)data, (T3)data);
        }

        protected abstract UniTask<bool> BuildDataAsync(T1 data, T2 data2, T3 data3);
    }

    public abstract class EntityBehavior<T1, T2, T3, T4> : EntityBehavior where T1 : IEntityData
                                                                          where T2 : IEntityData
                                                                          where T3 : IEntityData
                                                                          where T4 : IEntityData
    {
        public override UniTask<bool> BuildAsync(IEntityData data)
        {
            return BuildDataAsync((T1)data, (T2)data, (T3)data, (T4)data);
        }

        protected abstract UniTask<bool> BuildDataAsync(T1 data, T2 data2, T3 data3, T4 data4);
    }

}
