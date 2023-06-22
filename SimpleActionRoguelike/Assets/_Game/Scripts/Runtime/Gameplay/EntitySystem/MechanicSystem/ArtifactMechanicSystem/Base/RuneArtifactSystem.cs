using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Pool;
using Runtime.Definition;
using Runtime.Manager;
using Runtime.Manager.Gameplay;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class RuneArtifactSystem<T> : ArtifactSystem<T> where T : RuneArtifactDataConfigItem
    {
        private CancellationTokenSource _cancellationTokenSource;

        public async override UniTask Init(IEntityData entityData)
        {
            await base.Init(entityData);
            _cancellationTokenSource = new();
            StartRuneCooldownAsync().Forget();
        }

        protected virtual async UniTaskVoid StartRuneCooldownAsync()
        {
            float currentCountTime = 0;
            while (true)
            {
                await UniTask.Yield(_cancellationTokenSource.Token);
                currentCountTime += Time.deltaTime;

                if (GameManager.Instance.CurrentGameStateType != GameStateType.GameplayRunning)
                    continue;

                if(currentCountTime >= ownerData.runeInterval)
                {
                    var rune = await PoolManager.Instance.Rent("rune_artifact");

                    rune.transform.position = MapManager.Instance.GetRandomWalkablePoint();

                    var runeArtifact = rune.GetComponent<RuneArtifact>();
                    // Find some place to spawn.
                    await runeArtifact.InitAsync(ownerData.runeLifeTime, ArtifactType, _cancellationTokenSource.Token);
                    currentCountTime = 0;
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _cancellationTokenSource?.Cancel();
        }
    }
}