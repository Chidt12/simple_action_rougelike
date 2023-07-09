using Cysharp.Threading.Tasks;
using Runtime.Constants;
using Runtime.Core.Pool;
using Runtime.Definition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class StackArtifactIcon : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private Transform _scaleTransform;
        [SerializeField] private GameObject _emptyGameObject;
        private float _normalScale = 1;
        private float _selectScale = 1.4f;
        private ArtifactType _artifactType;
        private int _dataId;
        private bool _hasData;

        public bool HasData => _hasData;

        public void UpdateData(ArtifactType artifactType, int dataId, CancellationToken token)
        {
            _hasData = true;
            _artifactType = artifactType;
            _dataId = dataId;
            _emptyGameObject.SetActive(false);
            LoadSpriteAsync(token).Forget();
        }

        public void ToggleSelect(bool isSelected)
        {
            if (isSelected)
                _scaleTransform.localScale = new Vector2(_selectScale, _selectScale);
            else
                _scaleTransform.localScale = new Vector2(_normalScale, _normalScale);
        }

        private async UniTaskVoid LoadSpriteAsync(CancellationToken token)
        {
            _iconImage.sprite = await AssetLoader.LoadSprite(Constant.IconSpriteAtlasKey($"artifact_{(int)_artifactType}_{_dataId}"), token);
        }

        public void Clear()
        {
            _hasData = false;
            _scaleTransform.localScale = new Vector2(_normalScale, _normalScale);
            _emptyGameObject.SetActive(true);
        }
    }
}