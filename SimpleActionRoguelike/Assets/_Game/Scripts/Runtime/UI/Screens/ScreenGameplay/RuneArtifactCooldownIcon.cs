using Cysharp.Threading.Tasks;
using DG.Tweening;
using Runtime.Constants;
using Runtime.Core.Pool;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class RuneArtifactCooldownIcon : MonoBehaviour
    {
        [SerializeField] private Image _processImage;
        [SerializeField] private DOTweenAnimation _spawnRuneAnimation;
        [SerializeField] private Image _iconImage;

        private ArtifactType _artifactType;
        private int _dataId;
        private ICooldown _cooldown;

        public ArtifactType ArtifactType => _artifactType;

        public void Init(IArtifactSystem artifactSystem, CancellationToken token)
        {
            _cooldown = artifactSystem as ICooldown;
            if (_cooldown != null)
            {
                _processImage.fillAmount = 0;
                _cooldown.OnCountTimeChanged += OnCountTime;
            }

            _artifactType = artifactSystem.ArtifactType;
            _dataId = artifactSystem.DataId;
            LoadSpriteAsync(token).Forget();
        }

        private async UniTaskVoid LoadSpriteAsync(CancellationToken token)
        {
            _iconImage.sprite = await AssetLoader.LoadSprite(Constant.IconSpriteAtlasKey($"artifact_{(int)_artifactType}_{_dataId}"), token);
        }

        private void OnCountTime(bool isFinishedCountTime)
        {
            if (isFinishedCountTime)
                _spawnRuneAnimation.DORestart();
            _processImage.fillAmount = _cooldown.CurrentCountTime / _cooldown.CountTime;
        }
    }
}