using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Pool;
using Runtime.Definition;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class CreateBuffFlagArtifactSystem : RuneArtifactSystem<CreateBuffFlagArtifactDataConfigItem>
    {
        private bool _isSpawningFlag;
        private EffectArea _effectArea;
        private CancellationTokenSource _cancellationTokenSource;
        private List<IEntityModifiedStatData> _targets;
        private Dictionary<int, GameObject> _buffVfxs;

        public override ArtifactType ArtifactType => ArtifactType.CreateBuffFlag;

        public override bool CanTrigger()
        {
            return !_isSpawningFlag;
        }

        public override UniTask Init(IEntityData entityData)
        {
            _targets = new();
            _buffVfxs = new();
            _cancellationTokenSource = new();
            return base.Init(entityData);
        }

        public override void Dispose()
        {
            _cancellationTokenSource.Dispose();
            base.Dispose();
        }

        public override void Trigger()
        {
            SpawningFlagAsync().Forget();
        }

        private async UniTaskVoid SpawningFlagAsync()
        {
            if (_effectArea)
            {
                PoolManager.Instance.Return(_effectArea.gameObject);
                _effectArea = null;

                foreach (var target in _targets)
                {
                    foreach (var item in ownerData.buffStats)
                        target.DebuffStat(item.statType, item.value, item.statModifyType);

                    if (_buffVfxs.ContainsKey(target.EntityUID))
                    {
                        var buffVfx = _buffVfxs[target.EntityUID];
                        PoolManager.Instance.Return(buffVfx);
                        _buffVfxs.Remove(target.EntityUID);
                    }
                }

                _targets.Clear();
            }

            _isSpawningFlag = true;
            var flagPrefab = await PoolManager.Instance.Rent(ownerData.flagPrefabName, token: _cancellationTokenSource.Token);
            flagPrefab.transform.position = ownerEntityData.Position;
            _effectArea = flagPrefab.GetComponent<EffectArea>();
            _effectArea.Init(EntityType.Hero, ownerData.rangeWidth, ownerData.rangeHeight, OnEntityEntered, OnEntityExited);

            _isSpawningFlag = false;
        }

        private void OnEntityEntered(IEntityModifiedStatData obj)
        {
            if (!obj.IsDead)
            {
                _targets.Add(obj);
                _effectArea.ToggleEnableVisual(true);
                foreach (var item in ownerData.buffStats)
                    obj.BuffStat(item.statType, item.value, item.statModifyType);

                AddBuffVfxAsync(obj).Forget();
            }
        }

        private async UniTaskVoid AddBuffVfxAsync(IEntityModifiedStatData obj)
        {
            var vfx = await PoolManager.Instance.Rent(ownerData.buffVfxPrefabName, token: _cancellationTokenSource.Token);
            if (_targets.Contains(obj))
            {
                vfx.transform.SetParent(obj.EntityTransform);
                vfx.transform.localPosition = Vector2.zero;
                _buffVfxs.TryAdd(obj.EntityUID, vfx);
            }
            else
            {
                PoolManager.Instance.Return(vfx);
            }
        }

        private void OnEntityExited(IEntityModifiedStatData obj)
        {
            var indexTarget = Array.IndexOf(_targets.ToArray(), obj);
            if(indexTarget != -1)
            {
                _targets.RemoveAt(indexTarget);
                foreach (var item in ownerData.buffStats)
                    obj.DebuffStat(item.statType, item.value, item.statModifyType);

                if (_buffVfxs.ContainsKey(obj.EntityUID))
                {
                    var buffVfx = _buffVfxs[obj.EntityUID];
                    PoolManager.Instance.Return(buffVfx);
                    _buffVfxs.Remove(obj.EntityUID);
                }
            }
            _targets.Remove(obj);

            if(_targets.Count == 0)
                _effectArea.ToggleEnableVisual(false);
        }
    }
}