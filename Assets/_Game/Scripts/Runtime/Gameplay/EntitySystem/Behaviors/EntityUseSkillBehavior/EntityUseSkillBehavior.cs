using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public class EntityUseSkillBehavior : EntityBehavior<IEntityControlData>
    {
        private IEntityControlData _controlData;
        private ISkillStrategy[] _skillStrategies;

        private int _currentlyUsedSkillIndex;
        private List<SkillModel> _skillModels;
        private CancellationTokenSource[] _skillCooldownCancellationTokenSource;

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data)
        {
            _controlData = data;
            return UniTask.FromResult(true);
        }
    }
}
