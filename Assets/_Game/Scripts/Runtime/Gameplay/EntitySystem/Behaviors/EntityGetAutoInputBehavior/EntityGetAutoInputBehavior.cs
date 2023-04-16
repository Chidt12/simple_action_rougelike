using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityGetAutoInputBehavior : EntityBehavior
    {
        public override UniTask<bool> BuildAsync(IEntityData data)
        {
            return UniTask.FromResult(false);
        }
    }
}