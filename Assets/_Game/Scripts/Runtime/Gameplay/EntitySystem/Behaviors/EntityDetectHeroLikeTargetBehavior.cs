using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityDetectHeroLikeTargetBehavior : EntityBehavior<IEntityControlData>
    {
        #region Members

        private IEntityControlData _controlData;

        #endregion Members

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data)
        {
            
        }


    }
}
