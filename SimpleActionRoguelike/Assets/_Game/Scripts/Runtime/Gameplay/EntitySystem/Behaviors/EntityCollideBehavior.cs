using Cysharp.Threading.Tasks;
using Runtime.Gameplay.CollisionDetection;
using Sirenix.OdinInspector;
using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityCollideBehavior : EntityBehavior<IEntityStatData>, ICollisionBody, IDisposeEntityBehavior
    {
        #region Members

        [SerializeField] private bool _disableWhenGetHurt;

        [ShowIf(nameof(_disableWhenGetHurt))]
        [SerializeField] private float _disableWhenGetHurtTime;

        protected CancellationTokenSource cancellationTokenSource;
        protected bool isDamaging;
        protected ICollisionShape collisionShape;
        private Collider2D _collider;
        protected CollisionSearchTargetType[] collisionBodySearchTypes;
        protected CollisionBodyType collisionBodyType;

        #endregion Members

        #region Properties

        public int RefId { get; set; }
        public ICollisionShape CollisionShape => collisionShape;
        public Vector2 CollisionSystemPosition => _collider.bounds.center;
        public CollisionSearchTargetType[] CollisionSearchTargetTypes => collisionBodySearchTypes;
        public Collider2D Collider => _collider;
        public CollisionBodyType CollisionBodyType => collisionBodyType;

        #endregion Properties

        #region Class Methods

        protected override UniTask<bool> BuildDataAsync(IEntityStatData data)
        {
            collisionBodySearchTypes = data.EntityType.GetCollisionBodySearchTypes(false);
            collisionBodyType = data.EntityType.GetCollisionBodyType();
            var collider = transform.GetComponent<Collider2D>();
            if (collider != null && data != null)
            {
                cancellationTokenSource = new();
                this._collider = collider;
                _collider.enabled = true;
                collisionShape = this.CreateCollisionShape(collider);
                CollisionSystem.Instance.AddBody(this);

                if(_disableWhenGetHurt)
                    data.HealthStat.OnDamaged += OnDamaged;

                return UniTask.FromResult(true);
            }
            return UniTask.FromResult(false);
        }

        private void OnDamaged(float arg1, EffectSource arg2, EffectProperty arg3)
        {
            if(!isDamaging)
                OnDamagedAsync().Forget();
        }

        private async UniTaskVoid OnDamagedAsync()
        {
            CollisionSystem.Instance.RemoveBody(this);
            _collider.enabled = false;

            await UniTask.Delay(TimeSpan.FromSeconds(_disableWhenGetHurtTime), cancellationToken: cancellationTokenSource.Token);

            CollisionSystem.Instance.AddBody(this);
            _collider.enabled = true;
        }

        public virtual void OnCollision(CollisionResult result, ICollisionBody other) { }

        public void Dispose()
        {
            cancellationTokenSource?.Cancel();
            CollisionSystem.Instance.RemoveBody(this);
        }

        #endregion Class Methods
    }
}
