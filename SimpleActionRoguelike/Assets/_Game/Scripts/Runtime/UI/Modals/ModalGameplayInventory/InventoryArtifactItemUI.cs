using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Runtime.Definition;
using System;
using Runtime.Core.Pool;
using Runtime.Constants;
using System.Threading;
using Runtime.Manager.Data;
using Runtime.Gameplay.EntitySystem;

namespace Runtime.UI
{
    public class InventoryArtifactItemUI : MonoBehaviour, IInventoryItem
    {
        [SerializeField] private CustomButton _clickButton;
        [SerializeField] private Image _icon;

        private ArtifactType _artifactType;
        private int _level;
        private int _dataId;
        private bool _hasData;
        private Action<string> _loadInfoAction;

        public async UniTask LoadUI(ArtifactType artifactType, int level, int dataId, Action<string> loadInfoAction, CancellationToken token)
        {
            _dataId = dataId;
            _hasData = true;
            _artifactType = artifactType;
            _level = level;

            _loadInfoAction = loadInfoAction;
            _icon.gameObject.SetActive(true);
            _icon.sprite = await AssetLoader.LoadSprite(Constant.IconSpriteAtlasKey($"artifact_{(int)artifactType}_{dataId}"), token);
        }

        public void ClearUI(Action<string> loadInfoAction)
        {
            _hasData = false;
            _loadInfoAction = loadInfoAction;
            _icon.gameObject.SetActive(false);
        }

        public void ToggleSelect(bool value)
        {
            _clickButton.ToggleSelect(value);
            if (value)
            {
                if (_hasData)
                    LoadDescriptionAsync().Forget();
                else
                    _loadInfoAction?.Invoke(string.Empty);
            }
        }

        private async UniTaskVoid LoadDescriptionAsync()
        {
            var buffInGameDataConfig = await DataManager.Config.LoadArtifactDataConfig(_artifactType);
            var heroData = EntitiesManager.Instance.HeroData;
            var description = await buffInGameDataConfig.GetDescription(heroData, _level, _dataId);
            _loadInfoAction?.Invoke(description.Item1);
        }
    }
}
