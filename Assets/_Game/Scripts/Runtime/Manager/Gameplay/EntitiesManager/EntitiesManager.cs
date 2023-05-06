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
using Sirenix.OdinInspector;
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

    public class EntitiesManager : MonoSingleton<EntitiesManager>
    {
        private const string SPAWN_ENEMY_VFX_NAME = "spawn_enemy_vfx";
        private const float DISPLAY_SPAWN_WARNING_TIME = 0.5f;

        [ReadOnly]
        [SerializeField]
        private MapEditorEntity[] _mapEditorEntities;

        private int _entityUId;
        private int _defeatedEnemiesCount;
        private List<IEntityData> _enemiesData;
        private int _currentWarningSpawnedEnemyCount;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationTokenSource _spawnWaveCancellationTokenSource;
        private bool _finishedSpawnInConfig = false;
        private bool _finishedSpawnInStageData = false;

        public IEntityData HeroData { get; private set; }
        public List<IEntityData> EnemiesData => _enemiesData;
        public bool HaveNoEnemiesLeft => EnemiesData.Count <= 0 && _currentWarningSpawnedEnemyCount <= 0;
        public int DefeatedEnemiesCount => _defeatedEnemiesCount;

        #region Class Methods

        public void Initialize()
        {
            _cancellationTokenSource = new();
            _defeatedEnemiesCount = 0;
            _entityUId = 0;
            HeroData = null;
            _enemiesData = new();
            _mapEditorEntities = FindObjectsOfType<MapEditorEntity>(true);
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
            var stageMapEditorEntities = _mapEditorEntities == null ? new() : _mapEditorEntities.Where(x => x.WaveActive == waveIndex)
                                                           .OrderBy(x => x.DelaySpawnInWave)
                                                           .ToList();

            if (stageMapEditorEntities.Count > 0)
            {
                _finishedSpawnInStageData = false;
                SpawnStageWaveByStageDataAsync(stageMapEditorEntities, finishSpawnCallback).Forget();
            }

            if (stageLoadConfigItem.entites != null && stageLoadConfigItem.entites.Length > 0)
            {
                var spawnEntityConfigItems = stageLoadConfigItem.entites.Where(x => x.waveIndex == waveIndex).OrderBy(x => x.delaySpawnTime).ToList();
                if (spawnEntityConfigItems.Count > 0)
                {
                    _finishedSpawnInConfig = false;
                    SpawnStageWaveByConfigAsync(spawnEntityConfigItems, finishSpawnCallback).Forget();
                }
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
                SpawnStageWaveByStageDataItemWithWarningAsync(stageMapEditorEntity).Forget();
            await LoadDataNotCreateEntityAsync(stageMapEditorEntity.gameObject, stageMapEditorEntity.EntityType, stageMapEditorEntity.EntityId, stageMapEditorEntity.Level);
        }

        private async UniTaskVoid SpawnStageWaveByStageDataItemWithWarningAsync(MapEditorEntity stageMapEditorEntity)
        {
            var warningVFX = await CreateSpawnWarningVFXAsync(stageMapEditorEntity.EntityType, DISPLAY_SPAWN_WARNING_TIME, stageMapEditorEntity.transform.position, _spawnWaveCancellationTokenSource.Token);
            await LoadDataNotCreateEntityAsync(stageMapEditorEntity.gameObject, stageMapEditorEntity.EntityType, stageMapEditorEntity.EntityId, stageMapEditorEntity.Level);
            RemoveSpawnVFX(warningVFX);
        }

        private async UniTask SpawnEntityAsync(EntityStageLoadConfigItem entityConfig)
        {
            if (entityConfig.followHero && HeroData != null)
            {
                var spawnedCenterPosition = HeroData.Position;
                var spawnedCenterOffsetDistance = entityConfig.distanceFromHero;
                var spawnedEntityInfo = entityConfig.entityConfigItem;
                await CreateEntitiesAsync(spawnedCenterPosition, spawnedCenterOffsetDistance, true,
                                                                   _spawnWaveCancellationTokenSource.Token, spawnedEntityInfo);
            }
            else
            {
                var spawnPointIndex = entityConfig.spawnPointIndex;
                if (entityConfig.spawnPointIndex >= MapManager.Instance.SpawnPoints.Count())
                    spawnPointIndex = 0;

                var spawnPosition = MapManager.Instance.SpawnPoints[spawnPointIndex].Position;
                await CreateEntitiesAsync(spawnPosition, true, _spawnWaveCancellationTokenSource.Token, entityConfig.entityConfigItem);
            }
        }

        #region Create Entities

        public async UniTask CreateEntitiesAsync(Vector3 spawnPosition, bool displayWarning, CancellationToken cancellationToken, SpawnedEntityInfo spawnedEntityInfo)
        {
            for (int i = 0; i < spawnedEntityInfo.entityNumber; i++)
            {
                await CreateEntityWithWarning(spawnedEntityInfo, spawnPosition, displayWarning, cancellationToken);
                await UniTask.Yield(cancellationToken);
            }
        }

        public async UniTask CreateEntitiesAsync(Vector3 spawnedCenterPosition, float spawnedCenterDistancedOffset, bool displayWarning, CancellationToken cancellationToken, params SpawnedEntityInfo[] spawnedEntitiesInfo)
        {
            var numberOfDirectionsAwayCenterPosition = 16;
            var numberOfSpawnedEntities = spawnedEntitiesInfo.Sum(x => x.entityNumber);
            var validPositions = new List<Vector2>();
            var spreadNodesCount = Mathf.FloorToInt(numberOfSpawnedEntities / numberOfDirectionsAwayCenterPosition);
            var checkedPositions = MapManager.Instance.GetWalkablePositionsAroundPosition(spawnedCenterPosition, spawnedCenterDistancedOffset, spreadNodesCount, numberOfDirectionsAwayCenterPosition);
            var overlapCircleCheckRadius = MapManager.Instance.SlotSize + MapManager.Instance.SlotHalfSize;

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
                        await CreateEntityWithWarning(spawnedEntityInfo, spawnPosition, displayWarning, cancellationToken);
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
                        var entityGameObject = await CreateEntityAsync(spawnedEntityInfo.entityId,
                                                                       spawnedEntityInfo.entityLevel,
                                                                       spawnedEntityInfo.entityType,
                                                                       spawnPosition);
                        entityGameObject.transform.SetParent(transform);
                        await UniTask.Yield(cancellationToken);
                    }
                }
            }
        }

        public async UniTask<GameObject> CreateEntityAsync(string entityId, int entityLevel, EntityType entityType, Vector2 spawnPosition, CancellationToken cancellationToken = default)
        {
            switch (entityType)
            {
                case EntityType.Hero:
                    return await CreateHeroAsync(int.Parse(entityId), entityLevel, spawnPosition, cancellationToken);
                case EntityType.Enemy:
                    return await CreateEnemyAsync(int.Parse(entityId), entityLevel, spawnPosition, cancellationToken);
                default:
                    return null;
            }
        }

        public async UniTask<GameObject> LoadDataNotCreateEntityAsync(GameObject entityGameObject, EntityType entityType, int entityId, int entityLevel)
        {
            switch (entityType)
            {
                case EntityType.Hero:
                    return await LoadDataNotCreateHeroAsync(entityGameObject, entityId, entityLevel);

                case EntityType.Enemy:
                    return await LoadDataNotCreateEnemyAsync(entityGameObject, entityId, entityLevel);
                default:
                    return null;
            }
        }

        public async UniTask<GameObject> CreateProjectileAsync(string projectileId, IEntityData creatorData, Vector2 spawnPosition, CancellationToken cancellationToken = default)
        {
            var projectileGameObject = await PoolManager.Instance.Rent(projectileId, token: cancellationToken);
            await projectileGameObject.GetComponent<IProjectile>().BuildAsync(creatorData, spawnPosition);
            return projectileGameObject;
        }

        public virtual HandleCharacterDiedResultType HandleCharacterDied(EntityDiedMessage entityDiedMessage)
        {
            if (entityDiedMessage.IsEnemyDied)
            {
                _defeatedEnemiesCount++;
                var enemyData = entityDiedMessage.EntityData;
                EnemiesData.Remove(enemyData);
                if (HaveNoEnemiesLeft && !entityDiedMessage.SpawnedEnemyAfterDeath)
                {
                    CreateEntityDestroyVfxAsync(entityDiedMessage.EntityData.EntityType, entityDiedMessage.EntityData.Position, this.GetCancellationTokenOnDestroy()).Forget();
                    return HandleCharacterDiedResultType.DeletedAllEnemyOnMap;
                }
            }
            else if (entityDiedMessage.IsHeroDied)
            {
                return HandleCharacterDiedResultType.HeroDied;
            }

            CreateEntityDestroyVfxAsync(entityDiedMessage.EntityData.EntityType, entityDiedMessage.EntityData.Position, this.GetCancellationTokenOnDestroy()).Forget();
            return HandleCharacterDiedResultType.None;
        }

        private async UniTask<GameObject> CreateHeroAsync(int entityId, int entityLevel, Vector2 spawnPosition, CancellationToken cancellationToken = default)
        {
            var heroData = await GameplayDataManager.Instance.GetHeroDataAsync(entityId);
            var heroModel = new HeroModel();
            heroModel.Init(EntityType.Hero, _entityUId++, entityId);
            heroModel.InitStats(heroData.Item1);
            heroModel.InitWeapon(heroData.Item2);
            var heroGameObject = await PoolManager.Instance.Rent(entityId.ToString(), token: cancellationToken);
            heroGameObject.transform.position = spawnPosition;
            await heroGameObject.GetComponent<IEntityHolder>().BuildAsync(heroModel);
            SimpleMessenger.Publish(new HeroSpawnedMessage(heroModel, heroGameObject.transform));
            CreateEntitySpawnVfxAsync(heroModel.EntityType, spawnPosition, cancellationToken).Forget();
            HeroData = heroModel;
            return heroGameObject;
        }

        private async UniTask<GameObject> LoadDataNotCreateHeroAsync(GameObject entityGameObject, int entityId, int entityLevel, CancellationToken cancellationToken = default)
        {
            var heroData = await GameplayDataManager.Instance.GetHeroDataAsync(entityId);
            var heroModel = new HeroModel();
            heroModel.Init(EntityType.Hero, _entityUId++, entityId);
            heroModel.InitStats(heroData.Item1);
            heroModel.InitWeapon(heroData.Item2);
            entityGameObject.SetActive(true);
            entityGameObject.transform.position = entityGameObject.transform.position;
            await entityGameObject.GetComponent<IEntityHolder>().BuildAsync(heroModel);
            SimpleMessenger.Publish(new HeroSpawnedMessage(heroModel, entityGameObject.transform));
            HeroData = heroModel;
            return entityGameObject;
        }

        private async UniTask<GameObject> CreateEnemyAsync(int entityId, int entityLevel, Vector2 spawnPosition, CancellationToken cancellationToken = default)
        {
            var enemyData = await GameplayDataManager.Instance.GetEnemyDataAsync(entityId, entityLevel);
            var enemyModel = new EnemyModel();
            enemyModel.Init(EntityType.Enemy, _entityUId++, entityId);
            enemyModel.InitStats(enemyData.Item1);
            enemyModel.InitSkills(enemyData.Item2);
            var enemyGameObject = await PoolManager.Instance.Rent(entityId.ToString(), token: cancellationToken);
            enemyGameObject.transform.position = spawnPosition;
            await enemyGameObject.GetComponent<IEntityHolder>().BuildAsync(enemyModel);
            SimpleMessenger.Publish(new EntitySpawnedMessage(enemyModel, enemyGameObject.transform));
            CreateEntitySpawnVfxAsync(enemyModel.EntityType, spawnPosition, cancellationToken).Forget();
            EnemiesData.Add(enemyModel);
            return enemyGameObject;
        }

        private async UniTask<GameObject> LoadDataNotCreateEnemyAsync(GameObject entityGameObject, int entityId, int entityLevel, CancellationToken cancellationToken = default)
        {
            var enemyData = await GameplayDataManager.Instance.GetEnemyDataAsync(entityId, entityLevel);
            var enemyModel = new EnemyModel();
            enemyModel.Init(EntityType.Enemy, _entityUId++, entityId);
            enemyModel.InitStats(enemyData.Item1);
            enemyModel.InitSkills(enemyData.Item2);
            entityGameObject.SetActive(true);
            entityGameObject.transform.position = entityGameObject.transform.position;
            await entityGameObject.GetComponent<IEntityHolder>().BuildAsync(enemyModel);
            SimpleMessenger.Publish(new EntitySpawnedMessage(enemyModel, entityGameObject.transform));
            EnemiesData.Add(enemyModel);
            return entityGameObject;
        }

        protected async UniTask CreateEntitySpawnVfxAsync(EntityType entityType, Vector2 vfxPosition, CancellationToken cancellationToken)
        {
        }

        protected async UniTask CreateEntityDestroyVfxAsync(EntityType entityType, Vector2 vfxPosition, CancellationToken cancellationToken)
        {
        }

        #region Create Entity With warning

        private async UniTask CreateEntityWithWarning(SpawnedEntityInfo spawnedEntityInfo, Vector2 spawnPosition, bool displayWarning, CancellationToken cancellationToken)
        {
            if (displayWarning && spawnedEntityInfo.entityType.IsEnemy())
                CreateEntityWithWarning(spawnedEntityInfo, spawnPosition, cancellationToken).Forget();
            else
                await CreateEntityAsync(spawnedEntityInfo.entityId,
                                        spawnedEntityInfo.entityLevel,
                                        spawnedEntityInfo.entityType,
                                        spawnPosition,
                                        cancellationToken);
        }

        private async UniTaskVoid CreateEntityWithWarning(SpawnedEntityInfo spawnedEntityInfo, Vector2 spawnPosition, CancellationToken cancellationToken)
        {
            var warningVFX = await CreateSpawnWarningVFXAsync(spawnedEntityInfo.entityType, DISPLAY_SPAWN_WARNING_TIME, spawnPosition, cancellationToken);

            var entityGameObject = await CreateEntityAsync(spawnedEntityInfo.entityId,
                                                            spawnedEntityInfo.entityLevel,
                                                            spawnedEntityInfo.entityType,
                                                            spawnPosition,
                                                            cancellationToken);
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
