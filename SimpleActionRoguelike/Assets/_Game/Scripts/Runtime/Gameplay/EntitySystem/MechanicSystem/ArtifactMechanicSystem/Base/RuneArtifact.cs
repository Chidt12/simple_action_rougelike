using Cysharp.Threading.Tasks;
using DG.Tweening;
using Runtime.Constants;
using Runtime.Core.Pool;
using Runtime.Definition;
using Runtime.Manager;
using Runtime.Manager.Gameplay;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [RequireComponent(typeof(Collider2D))]
    public class RuneArtifact : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _icon;
        [SerializeField] private float _flySpeed = 30f;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private Transform _progressTransform;

        private bool _isFlying;
        private Transform _target;
        private ArtifactType _artifactType;
        private float _lifeTime;
        private float _currentTime;

        private void OnDisable()
        {
            _isFlying = false;
        }

        private void OnEnable()
        {
            _collider.enabled = false;
        }

        private void Update()
        {
            if(_isFlying && _target != null)
            {
                if (Vector2.Distance(transform.position, _target.transform.position) <= Time.deltaTime * _flySpeed)
                {
                    GameplayManager.Instance.MechanicSystemManager.AddCollectedArtifact(_artifactType);
                    PoolManager.Instance.Return(gameObject);
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, _target.transform.position, Time.deltaTime * _flySpeed);
                }
            }

            if(_lifeTime > 0 && !_isFlying)
            {
                if(_currentTime >= _lifeTime)
                {
                    PoolManager.Instance.Return(gameObject);
                }
                else
                {
                    _currentTime += Time.deltaTime;
                    _progressTransform.localScale = new Vector2(Mathf.Clamp01((_lifeTime - _currentTime) / _lifeTime), 1);
                }
            }
        }

        public async UniTask InitAsync(float lifeTime, ArtifactType artifactType, CancellationToken cancellationToken)
        {
            _artifactType = artifactType;
            _currentTime = 0;
            _lifeTime = lifeTime;
            _icon.sprite = await AssetLoader.LoadSprite(Constant.IconSpriteAtlasKey($"artifact_{(int)artifactType}_0"), cancellationToken);
            _isFlying = false;
            _collider.enabled = true;
            _progressTransform.localScale = new Vector2(1, 1);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var entityHolder = collision.GetComponent<EntityHolder>();
            if (entityHolder)
            {
                if (entityHolder.EntityData.EntityType == EntityType.Hero)
                {
                    if (GameplayManager.Instance.MechanicSystemManager.CanAddCollectedArtifact())
                    {
                        _isFlying = true;
                        _target = collision.transform;
                    }
                }
            }
        }
    }
}
