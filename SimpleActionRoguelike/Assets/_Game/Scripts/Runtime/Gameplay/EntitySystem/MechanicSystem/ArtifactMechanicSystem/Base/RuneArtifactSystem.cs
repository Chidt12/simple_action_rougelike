using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Pool;
using Runtime.Definition;
using Runtime.Manager;
using Runtime.Manager.Gameplay;
using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class RuneArtifactSystem<T> : ArtifactSystem<T>, ICooldown where T : RuneArtifactDataConfigItem
    {
        protected float currentCountTime;
        protected CancellationTokenSource cancellationTokenSource;

        public float CurrentCountTime => currentCountTime;
        public float CountTime => ownerData.runeInterval;

        public Action<bool> OnCountTimeChanged { get; set; }

        public async override UniTask Init(IEntityControlData entityData)
        {
            await base.Init(entityData);
            currentCountTime = 0;
            OnCountTimeChanged?.Invoke(true);
            cancellationTokenSource = new();
            StartRuneCooldownAsync().Forget();
        }

        protected async UniTaskVoid StartRuneCooldownAsync()
        {
            currentCountTime = 0;
            while (true)
            {
                await UniTask.Yield(cancellationTokenSource.Token);
                if (GameManager.Instance.CurrentGameStateType != GameStateType.GameplayRunning)
                    continue;

                currentCountTime += Time.deltaTime;

                OnUpdateAsync();

                if(currentCountTime >= ownerData.runeInterval)
                {
                    OnCountTimeChanged?.Invoke(true);

                    var rune = await PoolManager.Instance.Rent("rune_artifact");

                    rune.transform.position = MapManager.Instance.GetRandomWalkablePoint();

                    var runeArtifact = rune.GetComponent<RuneArtifact>();
                    // Find some place to spawn.
                    await runeArtifact.InitAsync(ownerData.runeLifeTime, ArtifactType, DataId, cancellationTokenSource.Token);
                    currentCountTime = 0;
                }
                else
                {
                    OnCountTimeChanged?.Invoke(false);
                }
            }
        }

        protected virtual void OnUpdateAsync() { }

        public override UniTask ResetNewStage()
        {
            currentCountTime = 0;
            OnCountTimeChanged?.Invoke(true);
            return base.ResetNewStage();
        }

        public override void Dispose()
        {
            base.Dispose();
            cancellationTokenSource?.Cancel();
        }
    }
}