using Cysharp.Threading.Tasks;
using Runtime.Constants;
using Runtime.Core.Pool;
using Runtime.Definition;
using Runtime.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class StatItemUI : MonoBehaviour
    {
        [SerializeField] private StatType _statType;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _value;
        [SerializeField] private TextMeshProUGUI _statName;

        public StatType StatType => _statType;

        private void Awake()
        {
            LoadSpriteAsync().Forget();
        }

        public void SetValue(string value) => _value.text = value;

        private async UniTaskVoid LoadSpriteAsync()
        {
            _statName.text = await LocalizeManager.GetLocalizeAsync(LocalizeTable.GENERAL, LocalizeKeys.GetStatName(_statType));
            _icon.sprite = await AssetLoader.LoadSprite(Constant.IconSpriteAtlasKey($"stats_icon_{(int)_statType}"), this.GetCancellationTokenOnDestroy());
        }
    }
}