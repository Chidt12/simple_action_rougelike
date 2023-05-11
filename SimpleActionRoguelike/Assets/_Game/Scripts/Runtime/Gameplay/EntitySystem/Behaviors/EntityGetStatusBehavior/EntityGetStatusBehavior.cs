using Cysharp.Threading.Tasks;
using Runtime.Core.Pool;
using Runtime.Definition;
using Runtime.Helper;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Linq;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public class EntityGetStatusBehavior : EntityBehavior<IEntityStatusData>, IDisposeEntityBehavior
    {
        [SerializeField]
        private Transform _topPosition;
        [SerializeField]
        private Transform _middlePosition;
        [SerializeField]
        private Transform _bottomPosition;

        private IEntityStatusData _statusData;
        private CancellationTokenSource _cancellationTokenSource;
        private Dictionary<StatusType, StatusVFX> _statusVFXsDictionary;

        protected override UniTask<bool> BuildDataAsync(IEntityStatusData data)
        {
            if (data == null)
            {
                return UniTask.FromResult(false);
            }

            _cancellationTokenSource = new();
            _statusVFXsDictionary = new();
            _statusData = data;

            _statusData.UpdateCurrentStatus += UpdateCurrentStatus;

            return UniTask.FromResult(true);
        }

        private void UpdateCurrentStatus()
        {
            var allStatusTypes = _statusData.CurrentStatuses.Select(x => x.StatusType).Distinct().ToList();
            if(allStatusTypes.Count == 0)
            {
                RemoveAllVFX();
            }
            else
            {
                var removedStatusTypes = _statusVFXsDictionary.Keys.Where(x => !allStatusTypes.Contains(x));
                foreach (var statusType in removedStatusTypes)
                {
                    if (_statusVFXsDictionary.ContainsKey(statusType))
                    {
                        var status = _statusVFXsDictionary[statusType];
                        _statusVFXsDictionary.Remove(statusType);
                        status.Dispose();
                    }
                }

                foreach (var statusType in allStatusTypes)
                {
                    if (HasStatusVFX(statusType))
                    {
                        if (!_statusVFXsDictionary.ContainsKey(statusType))
                            CreateStatusEffectVFX(statusType).Forget();
                    }
                }
            }
        }

        private async UniTaskVoid CreateStatusEffectVFX(StatusType statusType)
        {
            _statusVFXsDictionary.Add(statusType, null);

            var statusVFXGameObject = await PoolManager.Instance.Rent(GetStatusEffectPrefabName(statusType), token: _cancellationTokenSource.Token);
            var statusVFX = statusVFXGameObject.GetOrAddComponent<StatusVFX>();
            statusVFXGameObject.transform.SetParent(GetStatusEffectPosition(statusType));
            statusVFXGameObject.transform.localPosition = Vector2.zero;
            _statusVFXsDictionary[statusType]  = statusVFX;
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
            _statusData.RemoveAllStatus();

            RemoveAllVFX();
        }

        private void RemoveAllVFX()
        {
            var statusEffectsVFX = transform.GetComponentsInChildren<StatusVFX>();
            foreach (var statusEffectVFX in statusEffectsVFX)
                statusEffectVFX.Dispose();

            _statusVFXsDictionary.Clear();
        }

        private string GetStatusEffectPrefabName(StatusType statusType)
        {
            return $"{statusType.ToString().ToSnakeCase()}_status_vfx";
        }
        

        private bool HasStatusVFX(StatusType statusType)
        {
            switch (statusType)
            {
                case StatusType.Stun:
                    return true;
                default:
                    return false;
            }
        }

        private Transform GetStatusEffectPosition(StatusType statusType)
        {
            switch (statusType)
            {
                case StatusType.Stun:
                case StatusType.Taunt:
                case StatusType.Terror:
                    return _topPosition;
                case StatusType.Poison:
                case StatusType.Chill:
                case StatusType.Bleed:
                    return _middlePosition;
                case StatusType.Freeze:
                    return _bottomPosition;
                default:
                    return _bottomPosition;
            }
        }
    }
}
