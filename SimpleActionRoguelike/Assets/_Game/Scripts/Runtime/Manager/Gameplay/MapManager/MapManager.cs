using PathCreation;
using Pathfinding;
using Runtime.Core.Singleton;
using Runtime.Gameplay.Manager;
using Runtime.Helper;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace Runtime.Manager.Gameplay
{
    public class MapManager : MonoSingleton<MapManager>, IDisposable
    {
        #region Members

        private int _randomOffsetPathThresholdSlotsCount = 2;
        private int _randomMoveSearchMinSlotsCount = 3;
        private int _randomMoveSearchMaxSlotsCount = 10;
        private float _randomMoveSearchMinOffsetDegrees = 20;
        private float _randomMoveSearchMaxOffsetDegrees = 30;

        #endregion Members

        #region Properties

        private PathCreator[] _pathCreators;
        private MapSpawnPoint[] _spawnPoints;
        public PathCreator[] PathCreators => _pathCreators;
        public MapSpawnPoint[] SpawnPoints => _spawnPoints;

        public GridGraph ActiveGraph
        {
            get
            {
                var gridGraph = AstarPath.active.data.gridGraph;
                return gridGraph;
            }
        }

        public float SlotSize { get { return ActiveGraph.nodeSize; } }
        public float SlotHalfSize { get { return ActiveGraph.nodeSize * 0.5f; } }
        public float Width { get { return ActiveGraph.width; } }
        public float Height { get { return ActiveGraph.depth; } }
        public Vector3 Center { get { return ActiveGraph.center; } }

        #endregion Properties

        #region Class Methods

        public void Dispose() { }


        public void LoadLevelMap(MapLevel mapLevel)
        {
            _spawnPoints = mapLevel.mapSpawnPoints;
            _pathCreators = mapLevel.pathCreators;

            if (AstarPath.active != null)
                AstarPath.active.Scan();
            else
            {
                FindObjectOfType<AstarPath>().Scan();
            }
        }

        public void FindPathWithRandomness(Vector2 startPosition, Vector2 endPosition, OnPathDelegate onPathCompleteCallback)
        {
            var startEndSqrDistance = (endPosition - startPosition).sqrMagnitude;
            var randomOffsetPathThresholdSqrDistance = (_randomOffsetPathThresholdSlotsCount * SlotSize) * (_randomOffsetPathThresholdSlotsCount * SlotSize);
            if (startEndSqrDistance > randomOffsetPathThresholdSqrDistance && false)
            {
                var randomMoveSearchSlotsCount = UnityRandom.Range(_randomMoveSearchMinSlotsCount, _randomMoveSearchMaxSlotsCount + 1);
                var randomMoveSearchSqrDistance = (randomMoveSearchSlotsCount * SlotSize) * (randomMoveSearchSlotsCount * SlotSize);
                if (randomMoveSearchSqrDistance > startEndSqrDistance)
                {
                    var startEndSlotsCount = Mathf.FloorToInt((endPosition - startPosition).magnitude / SlotSize);
                    randomMoveSearchSlotsCount = startEndSlotsCount;
                }
                var randomMoveSearchLength = randomMoveSearchSlotsCount * SlotSize;
                var randomMoveSearchOffsetDegrees = UnityRandom.Range(_randomMoveSearchMinOffsetDegrees, _randomMoveSearchMaxOffsetDegrees);
                var angleOffset = UnityRandom.Range(-randomMoveSearchOffsetDegrees, randomMoveSearchOffsetDegrees);
                Vector2 offset = Quaternion.Euler(0, 0, angleOffset) * (endPosition - startPosition).normalized * randomMoveSearchLength;
                endPosition = startPosition + offset;
                Path path = ABPath.Construct(startPosition, endPosition, onPathCompleteCallback);
                AstarPath.active.heuristic = Heuristic.None;
                AstarPath.StartPath(path);
            }
            else
            {
                Path path = ABPath.Construct(startPosition, endPosition, onPathCompleteCallback);
                AstarPath.active.heuristic = Heuristic.None;
                AstarPath.StartPath(path);
            }
        }

        public void FindStraightPath(Vector2 startPosition, Vector2 endPosition, OnPathDelegate onPathCompleteCallback)
        {
            Path path = ABPath.Construct(startPosition, endPosition, onPathCompleteCallback);
            AstarPath.active.heuristic = Heuristic.Manhattan;
            AstarPath.StartPath(path);
        }

        public void FindMoveAwayTargetReal(Vector2 startPosition, Vector2 targetPosition, float distanceToKeep, int spreadLength, OnPathDelegate onPathCompleteCallback)
        {
            var allPositions = GetWalkablePositionsAroundPosition(targetPosition, distanceToKeep, spreadLength);

            float minAngle = 360;
            var selectedPosition = allPositions.FirstOrDefault();

            foreach (var position in allPositions)
            {
                var vector1 = position - startPosition;
                var vector2 = targetPosition - startPosition;
                var angle = MathHelper.AngleBetween(vector1, vector2);
                var compareAngle = Mathf.Abs(angle - 180);
                if(minAngle > compareAngle)
                {
                    minAngle = compareAngle;
                    selectedPosition = position;
                }
            }

            
            Path path = ABPath.Construct(startPosition, selectedPosition, onPathCompleteCallback);
            AstarPath.active.heuristic = Heuristic.None;
            AstarPath.StartPath(path);
        }

        public void FindMoveAwayTargetPath(Vector2 startPosition, Vector2 endPosition, int searchLength, int spreadLength, float aimStrength, OnPathDelegate onPathCompleteCallback)
        {
            FleePath path = FleePath.Construct(startPosition, endPosition, searchLength, onPathCompleteCallback);
            path.spread = spreadLength;
            path.aimStrength = aimStrength;
            AstarPath.active.heuristic = Heuristic.None;
            AstarPath.StartPath(path);
        }

        public void FindNeighbourTargetPath(Vector2 fromPosition, Vector2 targetPosition, OnPathDelegate onPathCompleteCallback)
            => FindStraightPath(fromPosition, targetPosition, onPathCompleteCallback);

        public void FindNeighbourEmptyPath(Vector2 fromPosition, int searchLength, int spreadLength, OnPathDelegate onPathCompleteCallback)
        {
            RandomPath path = RandomPath.Construct(fromPosition, searchLength, onPathCompleteCallback);
            
            path.spread = spreadLength;
            path.aimStrength = 0.0f;
            AstarPath.active.heuristic = Heuristic.None;
            AstarPath.StartPath(path);
        }

        public bool IsWalkable(Vector2 position)
        {
            if (IsValidNodePosition(position))
            {
                var node = ActiveGraph.GetNode(position);
                return node.Walkable;
            }
            else return false;
        }

        public List<GridNode> GetAllWalkableNodesInRange(Vector2 centerPosition, float range)
        {
            var positions = ActiveGraph.nodes.Where(x => x.Walkable
                                && Vector2.Distance(centerPosition, (Vector3)x.position) <= range).ToList();
            return positions;
        }

        public Vector2 GetRandomWalkablePoint()
        {
            var walkablePoints = ActiveGraph.nodes.Where(x => x.Walkable).ToList();
            var point = walkablePoints[UnityRandom.Range(0, walkablePoints.Count)];
            return (Vector3)point.position;
        }

        public void UpdateMap(int indexX, int indexY)
            => ActiveGraph.UpdateNodeStatus(indexX, indexY);

        public void UpdateMapWithAroundPoints(Vector2 position, float maxBoundSize)
        {
            if (ActiveGraph.nodes == null)
                return;

            var validNodeIndexes = new List<Vector2Int>();
            var collisionBoundCenterPosition = position;
            var centerNodeIndex = GetNodeIndex(collisionBoundCenterPosition);
            var bonusExtend = SlotSize;
            var currentMeasuredExtend = 0.0f;

            while (currentMeasuredExtend < maxBoundSize + bonusExtend)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (!(i == 0 && j == 0))
                        {
                            int nodenIdexX = centerNodeIndex.x + i;
                            int nodeIndexY = centerNodeIndex.y + j;
                            var isValidNodeIndex = MapManager.Instance.IsValidNodeIndex(nodenIdexX, nodeIndexY);
                            if (isValidNodeIndex)
                                validNodeIndexes.Add(new Vector2Int(nodenIdexX, nodeIndexY));
                        }
                        else validNodeIndexes.Add(new Vector2Int(centerNodeIndex.x, centerNodeIndex.y));
                    }
                }

                currentMeasuredExtend += SlotHalfSize * 0.5f;
            }

            var noDuplicatedValidNodeIndexes = validNodeIndexes.Distinct().ToList();
            foreach (var nodeIndex in noDuplicatedValidNodeIndexes)
            {
                UpdateMap(nodeIndex.x, nodeIndex.y);
            }
        }

        public bool IsValidNodePosition(Vector2 position)
            => ActiveGraph.IsValidNodePosition(position);

        public bool IsValidNodeIndex(int nodeIndexX, int nodeIndexY)
            => ActiveGraph.IsValidNodeIndex(nodeIndexX, nodeIndexY);

        public Vector2Int GetNodeIndex(Vector2 position)
            => ActiveGraph.GetNodeIndex(position);

        public List<Vector2> GetWalkablePositionsAroundPosition(Vector2 originPosition, float radius, int spreadNodesCount = 0, int numberOfDirections = 16)
        {
            var validPositions = new List<Vector2>();
            var rotateAngle = 360.0f / numberOfDirections;
            for (int c = 0; c <= spreadNodesCount; c++)
            {
                for (int i = 0; i < numberOfDirections; i++)
                {
                    var angle = Quaternion.AngleAxis(rotateAngle * i, Vector3.forward);
                    var checkPosition = (Vector2)(angle * Vector2.up) * (radius + c * SlotSize) + originPosition;
                    if (IsWalkable(checkPosition))
                        validPositions.Add(checkPosition);
                }
            }

            return validPositions;
        }

        [Button("Load Stage Data")]
        private void LoadStageData()
        {
            _spawnPoints = gameObject.GetComponentsInChildren<MapSpawnPoint>(true);
        }

        #endregion Class Methods
    }
}
