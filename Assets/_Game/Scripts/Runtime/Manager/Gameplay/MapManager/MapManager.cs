using Pathfinding;
using Runtime.Core.Singleton;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Manager.Gameplay
{
    public class MapManager : MonoSingleton<MapManager>, IDisposable
    {
        #region Properties

        [ReadOnly]
        [SerializeField]
        private MapSpawnPoint[] _spawnPoints;

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

        public void FindPath(Vector2 startPosition, Vector2 endPosition, OnPathDelegate onPathCompleteCallback)
        {
            Path path = ABPath.Construct(startPosition, endPosition, onPathCompleteCallback);
            AstarPath.StartPath(path);
        }

        public void FindMoveAwayTargetPath(Vector2 startPosition, Vector2 endPosition, int searchLength, int spreadLength, float aimStrength, OnPathDelegate onPathCompleteCallback)
        {
            FleePath path = FleePath.Construct(startPosition, endPosition, searchLength, onPathCompleteCallback);
            path.spread = spreadLength;
            path.aimStrength = aimStrength;
            AstarPath.StartPath(path);
        }

        public void FindNeighbourTargetPath(Vector2 fromPosition, Vector2 targetPosition, OnPathDelegate onPathCompleteCallback)
            => FindPath(fromPosition, targetPosition, onPathCompleteCallback);

        public void FindNeighbourEmptyPath(Vector2 fromPosition, int searchLength, int spreadLength, OnPathDelegate onPathCompleteCallback)
        {
            RandomPath path = RandomPath.Construct(fromPosition, searchLength, onPathCompleteCallback);
            path.spread = spreadLength;
            path.aimStrength = 0.0f;
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

        public void UpdateMap(Vector2 position)
            => ActiveGraph.UpdateNodeStatus(position);

        public void UpdateMap(int indexX, int indexY)
            => ActiveGraph.UpdateNodeStatus(indexX, indexY);

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
