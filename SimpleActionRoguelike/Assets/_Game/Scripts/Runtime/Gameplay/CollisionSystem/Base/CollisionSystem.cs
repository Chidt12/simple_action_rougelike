using System;
using System.Linq;
using UnityEngine;
using Runtime.Core.Singleton;
using System.Collections.Generic;

namespace Runtime.Gameplay.CollisionDetection
{
    public sealed class CollisionSystem : MonoSingleton<CollisionSystem>, IDisposable
    {
        #region Members

        [SerializeField]
        private float _mapWidth;
        [SerializeField]
        private float _mapHeight;
        [SerializeField]
        private Vector2 _center;
        [SerializeField]
        private bool _drawGizmos;
        private const int MAX_COLLISION_BODIES = 10_000;
        private ICollisionBody[] _bodyList = new ICollisionBody[MAX_COLLISION_BODIES];
        private HashSet<int> _collidedPair = new HashSet<int>();
        private List<int> _collidedPairCache = new List<int>();
        private HashSet<int> _removedBodyIds = new HashSet<int>();
        private QuadTree[] _quadTrees = new QuadTree[6];
        private Queue<int> _refIdsQueue = new Queue<int>();
        private int _currentBodyCount;

        #endregion Members

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            _currentBodyCount = 0;
            InitQuadTree(new RectConfig(_center, _mapWidth, _mapHeight), 10, 5);
        }

        private void Update()
        {
            Step();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_drawGizmos)
            {
                foreach (var body in _bodyList)
                    body.CollisionShape.DrawGizmos();
            }
        }
#endif

        #endregion API Methods

        #region Class Methods

        public void InitQuadTree(RectConfig rect, int maxBodiesPerNode = 6, int maxLevel = 6)
        {
            _quadTrees[(int)CollisionSearchTargetType.Hero] = new QuadTree(_bodyList, rect, maxBodiesPerNode, maxLevel);
            _quadTrees[(int)CollisionSearchTargetType.Enemy] = new QuadTree(_bodyList, rect, maxBodiesPerNode, maxLevel);
            _quadTrees[(int)CollisionSearchTargetType.Object] = new QuadTree(_bodyList, rect, maxBodiesPerNode, maxLevel);
            _quadTrees[(int)CollisionSearchTargetType.Projectile] = new QuadTree(_bodyList, rect, maxBodiesPerNode, maxLevel);
        }

        public void Dispose()
        {
            foreach (var quadTree in _quadTrees)
                quadTree?.Dispose();
        }

        public bool AddBody(ICollisionBody body)
        {
            if (!_bodyList.Contains(body))
            {
                if (_refIdsQueue.Count > 0)
                {
                    body.RefId = _refIdsQueue.Dequeue();
                    _bodyList[body.RefId] = body;
                    AddBodyToQuadTree(body);
                }
                else if (_currentBodyCount < MAX_COLLISION_BODIES)
                {
                    body.RefId = _currentBodyCount;
                    _bodyList[_currentBodyCount] = body;
                    AddBodyToQuadTree(body);
                    _currentBodyCount++;
                }
                return true;
            }
            return false;
        }

        private void AddBodyToQuadTree(ICollisionBody collisionBody)
        {
            switch (collisionBody.CollisionBodyType)
            {
                case CollisionBodyType.Hero:
                    _quadTrees[(int)CollisionSearchTargetType.Hero].AddBody(collisionBody.RefId);
                    break;
                case CollisionBodyType.Enemy:
                    _quadTrees[(int)CollisionSearchTargetType.Enemy].AddBody(collisionBody.RefId);
                    break;
                case CollisionBodyType.Object:
                    _quadTrees[(int)CollisionSearchTargetType.Object].AddBody(collisionBody.RefId);
                    break;
                case CollisionBodyType.Projectile:
                    _quadTrees[(int)CollisionSearchTargetType.Projectile].AddBody(collisionBody.RefId);
                    break;
                case CollisionBodyType.Trap:
                case CollisionBodyType.TargetDetect:
                case CollisionBodyType.Default:
                    break;
                default:
                    break;
            }
        }

        public bool RemoveBody(ICollisionBody body)
        {
            if (body.RefId >= 0 && body.RefId <= _currentBodyCount)
            {
                // UnCollided Remove Body.
                foreach (var i in _collidedPair)
                {
                    var body2RefId = i % (MAX_COLLISION_BODIES + 1);
                    if (body2RefId == body.RefId)
                    {
                        var body1 = FindCollisionBody(i / (MAX_COLLISION_BODIES + 1));
                        var body2 = FindCollisionBody(body2RefId);
                        if (body1 == null || body2 == null)
                            continue;

                        var result = new CollisionResult();
                        result.collisionType = CollisionType.Exit;
                        body2.OnCollision(result, body1);
                        body1.OnCollision(result, body2);
                    }
                }

                _removedBodyIds.Add(body.RefId);
                _bodyList[body.RefId] = null;

                // refresh QuadTree each frame if bodies can move
                foreach (var quadTree in _quadTrees)
                    quadTree?.Clear();

                for (int i = 0; i <= _currentBodyCount; i++)
                {
                    if (_bodyList[i] != null)
                        AddBodyToQuadTree(_bodyList[i]);
                }
                return true;
            }
            return false;
        }

        private void Step()
        {
            if (_removedBodyIds.Count > 0)
            {
                foreach (var id in _removedBodyIds)
                    _refIdsQueue.Enqueue(id);
                _refIdsQueue.Clear();
            }

            // Get collided pair and remove pair not collide anymore.
            for (int i = 0; i <= _currentBodyCount; i++)
            {
                if (_bodyList[i] == null || _bodyList[i].CollisionSearchTargetTypes == null || _bodyList[i].CollisionSearchTargetTypes.Length == 0)
                    continue;

                var maxDist = _bodyList[i].CollisionShape.MaxExtent;
                var searchTargetsLength = _bodyList[i].CollisionSearchTargetTypes.Length;
                for (int j = 0; j < searchTargetsLength; j++)
                {
                    if (_bodyList[i] == null)
                        break;

                    var collisionSearchType = _bodyList[i].CollisionSearchTargetTypes[j];
                    var bodies = _quadTrees[(int)collisionSearchType].GetBodies(_bodyList[i].CollisionSystemPosition, maxDist); // Improved?

                    foreach (var bodyRefId in bodies)
                    {
                        if (_bodyList[i] == null)
                            break;
                        var otherBody = _bodyList[bodyRefId];
                        if (otherBody == null || i == bodyRefId)
                            continue;
                        var result = CheckCollide(_bodyList[i], otherBody);
                    }
                }
            }

            // Remain pairs is out of all extents. So all were triggered exit.
            foreach (var i in _collidedPair)
            {
                var body1 = FindCollisionBody(i / (MAX_COLLISION_BODIES + 1));
                var body2 = FindCollisionBody(i % (MAX_COLLISION_BODIES + 1));
                if (body1 == null || body2 == null)
                    continue;

                var result = new CollisionResult();
                result.collisionType = CollisionType.Exit;
                body2.OnCollision(result, body1);
                body1.OnCollision(result, body2);
            }
            _collidedPair.Clear();

            // Add all detected collided pairs above.
            for (int i = 0; i < _collidedPairCache.Count; i++)
                _collidedPair.Add(_collidedPairCache[i]);
            _collidedPairCache.Clear();

            // refresh QuadTree each frame if bodies can move
            foreach (var quadTree in _quadTrees)
                quadTree?.Clear();

            for (int i = 0; i <= _currentBodyCount; i++)
            {
                if (_bodyList[i] != null)
                    AddBodyToQuadTree(_bodyList[i]);
            }
        }

        public ICollisionBody FindCollisionBody(int refId)
            => _bodyList[refId];

        private bool CheckCollide(ICollisionBody body1, ICollisionBody body2)
        {
            var pairIdx = GetPairIdx(body1, body2);
            var paired = _collidedPair.Contains(pairIdx);// Improved by array?

            var collisionResult = body1.CollisionShape.TestCollision(body2.CollisionShape);
            if (collisionResult.collided)
            {
                if (paired)
                {
                    collisionResult.collisionType = CollisionType.Stay;
                    _collidedPair.Remove(pairIdx);
                    /* Don't need to handle on Collision Stay, skip for the sake of performance.
                    //body2.OnCollision(result, body1);
                    //body1.OnCollision(result, body2);
                    */
                }
                else
                {
                    collisionResult.collisionType = CollisionType.Enter;
                    body2.OnCollision(collisionResult, body1);
                    body1.OnCollision(collisionResult, body2);
                }
                _collidedPairCache.Add(pairIdx);
                return true;
            }
            else
            {
                if (paired)
                {
                    collisionResult.collisionType = CollisionType.Exit;
                    body2.OnCollision(collisionResult, body1);
                    body1.OnCollision(collisionResult, body2);
                    _collidedPair.Remove(pairIdx);
                    return false;
                }
            }
            return false;
        }

        private int GetPairIdx(ICollisionBody a, ICollisionBody b)
            => a.RefId * (MAX_COLLISION_BODIES + 1) + b.RefId;

        #endregion Class Methods
    }
}