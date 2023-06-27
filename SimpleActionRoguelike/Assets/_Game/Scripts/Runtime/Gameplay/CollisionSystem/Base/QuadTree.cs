using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.CollisionDetection
{
    public struct RectConfig
    {
        #region Members

        public Vector2 center;
        public float width;
        public float height;

        #endregion Members

        #region Class Methods

        public RectConfig(Vector2 center, float width, float height)
        {
            this.center = center;
            this.width = width;
            this.height = height;
        }

        #endregion Struct Methods
    }

    public class QuadTree : IDisposable
    {
        #region Members

        private RectConfig _rect;
        private int _maxBodiesPerNode;
        private int _maxLevel;
        private int _currentLevel;
        private List<int> _bodyRefIds;
        private QuadTree _childA;
        private QuadTree _childB;
        private QuadTree _childC;
        private QuadTree _childD;
        private List<int> _cacheBodyRefIds;
        private const int CAPACITY = 256;

        private ICollisionBody[] _allBodies;

        #endregion Members

        #region Class Methods

        public QuadTree(ICollisionBody[] allBodies, RectConfig rect, int maxBodiesPerNode = 6, int maxLevel = 6)
        {
            _rect = rect;
            _maxBodiesPerNode = maxBodiesPerNode;
            _maxLevel = maxLevel;
            _bodyRefIds = new(CAPACITY);
            _allBodies = allBodies;
        }

        public QuadTree(RectConfig rect, QuadTree parent)
            : this(parent._allBodies, rect, parent._maxBodiesPerNode, parent._maxLevel)
        {
            _currentLevel = parent._currentLevel + 1;
        }

        public void AddBody(int bodyRefId)
        {
            if (_childA != null)
            {
                var child = GetQuadChild(_allBodies[bodyRefId].CollisionSystemPosition);
                child.AddBody(bodyRefId);
            }
            else
            {
                _bodyRefIds.Add(bodyRefId);
                if (_bodyRefIds.Count > _maxBodiesPerNode && _currentLevel < _maxLevel)
                    Split();
            }
        }

        private void Split()
        {
            var width = _rect.width / 2;
            var height = _rect.height / 2;
            /// A | B
            /// __ __
            /// D | C

            // split a
            var aCenter = new Vector2(_rect.center.x - width / 2, _rect.center.y + height / 2);
            var aRectConfig = new RectConfig(aCenter, width, height);

            // split b
            var bCenter = new Vector2(_rect.center.x + width / 2, _rect.center.y + height / 2);
            var bRectConfig = new RectConfig(bCenter, width, height);

            // split c
            var cCenter = new Vector2(_rect.center.x + width / 2, _rect.center.y - height / 2);
            var cRectConfig = new RectConfig(cCenter, width, height);

            // split d
            var dCenter = new Vector2(_rect.center.x - width / 2, _rect.center.y - height / 2);
            var dRectConfig = new RectConfig(dCenter, width, height);

            // assign Quadtrees
            _childA = QuadTreePool.Rent(aRectConfig, this);
            _childB = QuadTreePool.Rent(bRectConfig, this);
            _childC = QuadTreePool.Rent(cRectConfig, this);
            _childD = QuadTreePool.Rent(dRectConfig, this);

            for (int i = _bodyRefIds.Count - 1; i >= 0; i--)
            {
                try
                {
                    var child = GetQuadChild(_allBodies[_bodyRefIds[i]].CollisionSystemPosition);
                    child.AddBody(_bodyRefIds[i]);
                    _bodyRefIds.RemoveAt(i);
                }
                catch
                {
                    throw new Exception("failed");
                }
            }
        }

        private QuadTree GetQuadChild(Vector3 position)
        {
            if (_childA == null)
                return null;

            Vector2 point = new Vector2(position.x, position.y);

            if (point.x > _rect.center.x)
            {

                if (point.y > _rect.center.y)
                    return _childB;
                else
                    return _childC;
            }
            else
            {
                if (point.y > _rect.center.y)
                    return _childA;
                else
                    return _childD;
            }
        }

        public void Clear()
        {
            QuadTreePool.Return(_childA);
            QuadTreePool.Return(_childB);
            QuadTreePool.Return(_childC);
            QuadTreePool.Return(_childD);
            _childA = null;
            _childB = null;
            _childC = null;
            _childD = null;
            _bodyRefIds.Clear();
        }

        public void DrawGizmos()
        {
            //draw children
            if (_childA != null) _childA.DrawGizmos();
            if (_childB != null) _childB.DrawGizmos();
            if (_childC != null) _childC.DrawGizmos();
            if (_childD != null) _childD.DrawGizmos();

            //draw rect
            Gizmos.color = Color.cyan;
            var p1 = new Vector3(_rect.center.x - _rect.width / 2, _rect.center.y - _rect.height / 2, 0.1f);
            var p2 = new Vector3(p1.x + _rect.width, p1.y, 0.1f);
            var p3 = new Vector3(p1.x + _rect.width, p1.y + _rect.height, 0.1f);
            var p4 = new Vector3(p1.x, p1.y + _rect.height, 0.1f);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p4, p1);
        }

        public bool ContainsRect(Rect rect)
        {
            Rect currentRect = new Rect(_rect.center.x - _rect.width / 2, _rect.center.y - _rect.height / 2, _rect.height, _rect.width);
            return currentRect.Overlaps(rect);
        }

        public bool ContainsCircle(Vector2 circleCenter, double radius)
        {
            if (Math.Abs(circleCenter.x - _rect.center.x) <= _rect.width / 2 + radius)
            {
                if (Math.Abs(circleCenter.y - _rect.center.y) <= _rect.height / 2 + radius)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
        public List<int> GetBodies(Vector3 center, double radius)
        {
            if (_cacheBodyRefIds == null) _cacheBodyRefIds = new(CAPACITY);
            else _cacheBodyRefIds.Clear();

            Vector2 point = new Vector2(center.x, center.y);
            GetBodies(point, radius, _cacheBodyRefIds);
            return _cacheBodyRefIds;
        }

        public List<int> GetBodies(Rect rect)
        {
            if (_cacheBodyRefIds == null) _cacheBodyRefIds = new List<int>();
            else _cacheBodyRefIds.Clear();
            GetBodies(rect, _cacheBodyRefIds);
            return _cacheBodyRefIds;
        }

        private void GetBodies(Vector2 point, double radius, List<int> bods)
        {
            //No Child
            if (_childA == null)
            {
                bods.AddRange(_bodyRefIds);
            }
            else
            {
                if (_childA.ContainsCircle(point, radius))
                    _childA.GetBodies(point, radius, bods);
                if (_childB.ContainsCircle(point, radius))
                    _childB.GetBodies(point, radius, bods);
                if (_childC.ContainsCircle(point, radius))
                    _childC.GetBodies(point, radius, bods);
                if (_childD.ContainsCircle(point, radius))
                    _childD.GetBodies(point, radius, bods);
            }
        }

        private void GetBodies(Rect rect, List<int> bods)
        {
            // No Child
            if (_childA == null)
            {
                bods.AddRange(_bodyRefIds);
            }
            else
            {
                if (_childA.ContainsRect(rect))
                    _childA.GetBodies(rect, bods);
                if (_childB.ContainsRect(rect))
                    _childB.GetBodies(rect, bods);
                if (_childC.ContainsRect(rect))
                    _childC.GetBodies(rect, bods);
                if (_childD.ContainsRect(rect))
                    _childD.GetBodies(rect, bods);
            }
        }

        public void Dispose()
        {
            _cacheBodyRefIds?.Clear();
            _bodyRefIds?.Clear();
            _childA?.Dispose();
            _childB?.Dispose();
            _childC?.Dispose();
            _childD?.Dispose();
        }

        #endregion Class Methods

        #region Class In Class

        private static class QuadTreePool
        {
            #region Members

            private static Queue<QuadTree> _pool;
            private static int _maxPoolCount = 1024;
            private static int _defaultMaxBodiesPerNode = 6;
            private static int _defaultMaxLevel = 6;

            #endregion Members

            #region Class Methods

            public static void Return(QuadTree tree)
            {
                if (tree == null)
                    return;
                tree.Clear();
                if (_pool.Count > _maxPoolCount)
                    return;
                _pool.Enqueue(tree);
            }

            private static void Init()
            {
                _pool = new Queue<QuadTree>();
                for (int i = 0; i < _maxPoolCount; i++)
                    _pool.Enqueue(new QuadTree(null, new RectConfig(Vector2.zero, 0, 0), _defaultMaxBodiesPerNode, _defaultMaxLevel));
            }

            public static QuadTree Rent(RectConfig rect, QuadTree parent)
            {
                if (_pool == null)
                    Init();

                QuadTree tree = null;

                if (_pool.Count > 0)
                {
                    tree = _pool.Dequeue();
                    tree._rect = rect;
                    tree._maxLevel = parent._maxLevel;
                    tree._maxBodiesPerNode = parent._maxBodiesPerNode;
                    tree._currentLevel = parent._currentLevel + 1;
                    tree._allBodies = parent._allBodies;
                }
                else
                {
                    tree = new QuadTree(rect, parent);
                }

                return tree;
            }

            #endregion Class Methods
        }

        #endregion Class In Class
    }
}