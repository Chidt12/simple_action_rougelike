using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Core.Message;
using Runtime.Core.Pool;
using Runtime.Core.Singleton;
using Runtime.Definition;
using Runtime.Helper;
using Runtime.Manager.Gameplay;
using Runtime.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public enum HandleCharacterDiedResultType
    {
        None,
        DeletedAllEnemyOnMap,
        HeroDied,
    }

    public partial class EntitiesManager : MonoSingleton<EntitiesManager>
    {
        private const string SPAWN_ENEMY_VFX_NAME = "spawn_enemy_vfx";
        private const float DISPLAY_SPAWN_WARNING_TIME = 0.5f;

        private MapEditorEntity[] _mapEditorEntities;
        private int _entityUId;
        private int _defeatedEnemiesCount;
        private List<IEntityData> _enemiesData;
        private int _currentWarningSpawnedEnemyCount;
        private int _currentSpawningEnemyCount;

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationTokenSource _spawnWaveCancellationTokenSource;
        private bool _finishedSpawnInConfig = false;
        private bool _finishedSpawnInStageData = false;

        public IEntityData HeroData { get; private set; }
        public List<IEntityData> EnemiesData => _enemiesData;
        public bool HaveNoEnemiesLeft => EnemiesData.Count <= 0 && _currentWarningSpawnedEnemyCount <= 0 && _currentSpawningEnemyCount <= 0 && CurrentActionContainEnemySpawn <= 0;
        public int DefeatedEnemiesCount => _defeatedEnemiesCount;
        public int CurrentActionContainEnemySpawn { get; set; }

        #region Class Methods

        public void Initialize()
        {
            _cancellationTokenSource = new();
            _defeatedEnemiesCount = 0;
            _entityUId = 0;
            _currentSpawningEnemyCount = 0;
            _currentWarningSpawnedEnemyCount = 0;
            _enemiesData = new();

            CurrentActionContainEnemySpawn = 0;
            HeroData = null;
        }

        public void Dipose()
        {
            _cancellationTokenSource.Cancel();
        }

        public void SpawnStageWave(StageLoadConfigItem stageLoadConfigItem, int waveIndex, Action finishSpawnCallback)
        {
            _finishedSpawnInStageData = true;
            _finishedSpawnInConfig = true;
            _spawnWaveCancellationTokenSource = new CancellationTokenSource();
            _mapEditorEntities = FindObjectsOfType<MapEditorEntity>(true);
            var stageMapEditorEntities = _mapEditorEntities == null ? new() : _mapEditorEntities.Where(x => x.WaveActive == waveIndex)
                                                           .OrderBy(x => x.DelaySpawnInWave)
                                                           .ToList();

            if (stageMapEditorEntities.Count > 0)
            {
                _finishedSpawnInStageData = false;
                SpawnStageWaveByStageDataAsync(stageMapEditorEntities, finishSpawnCallback).Forget();
            }

            var waveConfigItem = stageLoadConfigItem.waveConfigs.FirstOrDefault(x => x.waveIndex == waveIndex);

            if (waveConfigItem.entites != null && waveConfigItem.entites.Length > 0)
            {
                var spawnEntityConfigItems = waveConfigItem.entites.OrderBy(x => x.delaySpawnTime).ToList();
                _finishedSpawnInConfig = false;
                SpawnStageWaveByConfigAsync(spawnEntityConfigItems, finishSpawnCallback).Forget();
            }
        }

        private async UniTask SpawnStageWaveByConfigAsync(List<EntityStageLoadConfigItem> spawnEntityConfigItems, Action finishSpawnCallback)
        {
            float countTimeDelay = 0;
            foreach (var spawnEntityConfigItem in spawnEntityConfigItems)
            {
                var delayTime = spawnEntityConfigItem.delaySpawnTime - countTimeDelay;
                await UniTask.Delay(TimeSpan.FromSeconds(delayTime), cancellationToken: _spawnWaveCancellationTokenSource.Token);
                countTimeDelay += delayTime;
                await SpawnEntityAsync(spawnEntityConfigItem);
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, this.GetCancellationTokenOnDestroy());
            }
            _finishedSpawnInConfig = true;
            if (_finishedSpawnInStageData && _finishedSpawnInConfig)
                finishSpawnCallback?.Invoke();
        }

        private async UniTask SpawnStageWaveByStageDataAsync(List<MapEditorEntity> stageMapEditorEntities, Action finishSpawnCallback)
        {
            float countTimeDelay = 0;
            foreach (var stageMapEditorEntity in stageMapEditorEntities)
            {
                var delayTime = stageMapEditorEntity.DelaySpawnInWave - countTimeDelay;
                await UniTask.Delay(TimeSpan.FromSeconds(delayTime), cancellationToken: _spawnWaveCancellationTokenSource.Token);
                countTimeDelay += delayTime;
                await SpawnStageWaveByStageDataItemAsync(stageMapEditorEntity);
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, _spawnWaveCancellationTokenSource.Token);
            }

            _finishedSpawnInStageData = true;
            if (_finishedSpawnInStageData && _finishedSpawnInConfig)
                finishSpawnCallback?.Invoke();
        }

        private async UniTask SpawnStageWaveByStageDataItemAsync(MapEditorEntity stageMapEditorEntity)
        {
            if (stageMapEditorEntity.EntityType.IsEnemy())
                LoadEntityOnMapWithWarningAsync(stageMapEditorEntity).Forget();
            await LoadEntityOnMapAsync(stageMapEditorEntity.gameObject, stageMapEditorEntity.EntityType, stageMapEditorEntity.EntityId, stageMapEditorEntity.Level);
        }

        private async UniTaskVoid LoadEntityOnMapWithWarningAsync(MapEditorEntity stageMapEditorEntity)
        {
            var warningVFX = await CreateSpawnWarningVFXAsync(stageMapEditorEntity.EntityType, DISPLAY_SPAWN_WARNING_TIME, stageMapEditorEntity.transform.position, _spawnWaveCancellationTokenSource.Token);
            await LoadEntityOnMapAsync(stageMapEditorEntity.gameObject, stageMapEditorEntity.EntityType, stageMapEditorEntity.EntityId, stageMapEditorEntity.Level);
            RemoveSpawnVFX(warningVFX);
        }

        private async UniTask SpawnEntityAsync(EntityStageLoadConfigItem entityConfig)
        {
            if (entityConfig.followHero && HeroData != null)
            {
                var spawnedCenterPosition = HeroData.Position;
                var spawnedCenterOffsetDistance = entityConfig.distanceFromHero;
                var spawnedEntityInfo = entityConfig.entityConfigItem;
                await CreateEntitiesAsync(spawnedCenterPosition, spawnedCenterOffsetDistance, true, true, _spawnWaveCancellationTokenSource.Token, spawnedEntityInfo);
            }
            else
            {
                var spawnPointIndex = entityConfig.spawnPointIndex;
                if (entityConfig.spawnPointIndex >= MapManager.Instance.SpawnPoints.Count())
                    spawnPointIndex = 0;

                var spawnPosition = MapManager.Instance.SpawnPoints[spawnPointIndex].Position;
                await CreateEntitiesAsync(spawnPosition, 0.3f, true, true, _spawnWaveCancellationTokenSource.Token, entityConfig.entityConfigItem);
            }
        }

        #region Create Entities

        public async UniTask<List<Vector2>> CreateEntitiesAsync(Vector3 spawnedCenterPosition, float spawnedCenterDistancedOffset, bool displayWarning, bool fromSystem, CancellationToken cancellationToken, params SpawnedEntityInfo[] spawnedEntitiesInfo)
        {
            var spawnedPositions = new List<Vector2>();
            var numberOfDirectionsAwayCenterPosition = 32;
            var numberOfSpawnedEntities = spawnedEntitiesInfo.Sum(x => x.entityNumber);
            var validPositions = new List<Vector2>();
            var spreadNodesCount = Mathf.FloorToInt(numberOfSpawnedEntities / numberOfDirectionsAwayCenterPosition);
            var checkedPositions = MapManager.Instance.GetWalkablePositionsAroundPosition(spawnedCenterPosition, spawnedCenterDistancedOffset, spreadNodesCount, numberOfDirectionsAwayCenterPosition);
            var overlapCircleCheckRadius = MapManager.Instance.SlotSize;

            foreach (var checkedPosition in checkedPositions)
            {
                var collider = Physics2D.OverlapCircle(checkedPosition, overlapCircleCheckRadius, Layers.OBSTACLE_LAYER_MASK);
                if (collider == null)
                    validPositions.Add(checkedPosition);
            }

            if (validPositions.Count > 0)
            {
                validPositions.Shuffle();
                var index = 0;

                foreach (var spawnedEntityInfo in spawnedEntitiesInfo)
                {
                    for (int i = 0; i < spawnedEntityInfo.entityNumber; i++)
                    {
                        if (index >= validPositions.Count)
                        {
                            validPositions.Shuffle();
                            index = 0;
                        }

                        var spawnPosition = validPositions[index++];
                        spawnedPositions.Add(spawnPosition);
                        await LoadEntity(spawnedEntityInfo, spawnPosition, displayWarning, fromSystem, cancellationToken);
                        await UniTask.Yield(cancellationToken);
                    }
                }
            }
            else
            {
                foreach (var spawnedEntityInfo in spawnedEntitiesInfo)
                {
                    for (int i = 0; i < spawnedEntityInfo.entityNumber; i++)
                    {
                        var spawnPosition = spawnedCenterPosition;
                        spawnedPositions.Add(spawnPosition);
                        await LoadEntity(spawnedEntityInfo, spawnPosition, displayWarning, fromSystem, cancellationToken);
                        await UniTask.Yield(cancellationToken);
                    }
                }
            }

            return spawnedPositions;
        }

        public async UniTask<GameObject> CreateEntityAsync(SpawnedEntityInfo spawnedEntityInfo, Vector2 spawnPosition, CancellationToken cancellationToken = default)
        {
            GameObject entity = null;
            switch (spawnedEntityInfo.entityType)
            {
                case EntityType.Hero:
                    entity = await CreateHeroAsync(int.Parse(spawnedEntityInfo.entityId), spawnedEntityInfo.entityLevel, spawnPosition, cancellationToken);
                    break;
                case EntityType.Enemy:
                    _currentSpawningEnemyCount++;
                    entity = await CreateEnemyAsync(int.Parse(spawnedEntityInfo.entityId), spawnedEntityInfo.entityLevel, spawnPosition, cancellationToken);
                    _currentSpawningEnemyCount--;
                    break;
                case EntityType.Boss:
                    _currentSpawningEnemyCount++;
                    entity = await CreateBossAsync(int.Parse(spawnedEntityInfo.entityId), spawnedEntityInfo.entityLevel, spawnPosition, cancellationToken);
                    _currentSpawningEnemyCount--;
                    break;
                case EntityType.Asset:
                    entity = await CreateAssetAsync(int.Parse(spawnedEntityInfo.entityId), spawnedEntityInfo.entityLevel, spawnPosition, cancellationToken);
                    break;
                default:
                    break;
            }
            return entity;
        }

        public async UniTask<GameObject> LoadEntityOnMapAsync(GameObject entityGameObject, EntityType entityType, int entityId, int entityLevel)
        {
            GameObject entity = null;
            switch (entityType)
            {
                case EntityType.Hero:
                    entity = await LoadHeroOnMapAsync(entityGameObject, entityId, entityLevel);
                    break;
                case EntityType.Enemy:
                    _currentSpawningEnemyCount++;
                    entity = await LoadEnemyOnMapAsync(entityGameObject, entityId, entityLevel);
                    _currentSpawningEnemyCount--;
                    break;
                case EntityType.Boss:
                    _currentSpawningEnemyCount++;
                    entity = await LoadBossOnMapAsync(entityGameObject, entityId, entityLevel);
                    _currentSpawningEnemyCount--;
                    break;
                case EntityType.Asset:
                    entity = await LoadAssetOnMapAsync(entityGameObject, entityId, entityLevel);
                    break;
                default:
                    break;
            }
            return entity;
        }

        public async UniTask<GameObject> CreateProjectileAsync(string projectileId, IEntityData creatorData, Vector2 spawnPosition, CancellationToken cancellationToken = default)
        {
            var projectileGameObject = await PoolManager.Instance.Rent(projectileId, token: cancellationToken);
            await projectileGameObject.GetComponent<IProjectile>().BuildAsync(creatorData, spawnPosition);
            return projectileGameObject;
        }

        public virtual async UniTask<HandleCharacterDiedResultType> HandleCharacterDied(EntityNotifyDiedMessage entityDiedMessage)
        {
            if (entityDiedMessage.IsEnemyDied)
            {
                _defeatedEnemiesCount++;
                var enemyData = entityDiedMessage.EntityData;

                if (entityDiedMessage.DeathIdentity.deathType != DeathType.None)
                    await ExecuteDeathStrategyAsync(entityDiedMessage.EntityData, entityDiedMessage.DeathIdentity, this.GetCancellationTokenOnDestroy());

                EnemiesData.Remove(enemyData);
                SimpleMessenger.Publish(new EntityDiedMessage(enemyData));

                if (HaveNoEnemiesLeft && !entityDiedMessage.DeathIdentity.deathType.IsSpawnedEnemy())
                {
                    return HandleCharacterDiedResultType.DeletedAllEnemyOnMap;
                }
            }
            else if (entityDiedMessage.IsHeroDied)
            {
                SimpleMessenger.Publish(new EntityDiedMessage(entityDiedMessage.EntityData));
                return HandleCharacterDiedResultType.HeroDied;
            }
            return HandleCharacterDiedResultType.None;
        }

        private async UniTask ExecuteDeathStrategyAsync(IEntityData deathEntityData, DeathDataIdentity deathDataIdentity, CancellationToken cancellationToken)
        {
            var deathStrategy = DeathStrategyFactory.GetDeathStrategy(deathDataIdentity.deathType);
            await deathStrategy.Execute(deathEntityData, deathDataIdentity, cancellationToken);
        }

        protected async UniTask CreateEntitySpawnVfxAsync(EntityType entityType, Vector2 vfxPosition, CancellationToken cancellationToken)
        {
        }

        #region Create Entity With warning

        private async UniTask LoadEntity(SpawnedEntityInfo spawnedEntityInfo, Vector2 spawnPosition, bool displayWarning, bool fromSystem, CancellationToken cancellationToken)
        {
            if (displayWarning && spawnedEntityInfo.entityType.IsEnemy())
                CreateEntityWithWarningAsync(spawnedEntityInfo, spawnPosition, fromSystem, cancellationToken).Forget();
            else
                await CreateEntityAsync(spawnedEntityInfo, spawnPosition, cancellationToken);
        }

        private async UniTaskVoid CreateEntityWithWarningAsync(SpawnedEntityInfo spawnedEntityInfo, Vector2 spawnPosition, bool fromSystem, CancellationToken cancellationToken)
        {
            var warningVFX = await CreateSpawnWarningVFXAsync(spawnedEntityInfo.entityType, DISPLAY_SPAWN_WARNING_TIME, spawnPosition, cancellationToken);
            var entityGameObject = await CreateEntityAsync(spawnedEntityInfo, spawnPosition, cancellationToken);
            RemoveSpawnVFX(warningVFX);
            entityGameObject.transform.SetParent(transform);
        }

        public async UniTask<GameObject> CreateSpawnWarningVFXAsync(EntityType entityType, float displayTime, Vector2 spawnPosition, CancellationToken cancellationToken)
        {
            _currentWarningSpawnedEnemyCount++;
            float vfxScaleValue = 1;
            var warningVFX = await PoolManager.Instance.Rent(SPAWN_ENEMY_VFX_NAME, token: cancellationToken);
            warningVFX.transform.position = spawnPosition;
            warningVFX.transform.localScale = new Vector2(vfxScaleValue, vfxScaleValue);
            await UniTask.Delay(TimeSpan.FromSeconds(displayTime), cancellationToken: cancellationToken);
            return warningVFX;
        }

        public void RemoveSpawnVFX(GameObject spawnVFX)
        {
            _currentWarningSpawnedEnemyCount--;
            PoolManager.Instance.Return(spawnVFX);
        }

        #endregion Create Entities

        #endregion Create Entities


        #endregion Class Methods
    }
}
