using Cysharp.Threading.Tasks;
using Runtime.Definition;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityGetStatusBehavior : EntityBehavior<IEntityStatusData>
    {
        [SerializeField]
        private Transform _topPosition;
        [SerializeField]
        private Transform _middlePosition;
        [SerializeField]
        private Transform _bottomPosition;

        private List<IStatus> _statuses;
        private Dictionary<StatusType, StatusVFX> _statusEffectVFXsDictionary;
        private CancellationTokenSource _cancellationTokenSource;

        protected override UniTask<bool> BuildDataAsync(IEntityStatusData data)
        {
            if (data == null)
            {
                return UniTask.FromResult(false);
            }


            _cancellationTokenSource = new CancellationTokenSource();
            _statuses = new();
            _statusEffectVFXsDictionary = new();

            return UniTask.FromResult(true);
        }
    }
}
