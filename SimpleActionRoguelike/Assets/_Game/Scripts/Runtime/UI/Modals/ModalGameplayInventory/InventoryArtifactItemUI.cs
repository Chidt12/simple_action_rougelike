using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Runtime.Definition;
using System;

namespace Runtime.UI
{
    public class InventoryArtifactItemUI : MonoBehaviour, IInventoryItem
    {
        [SerializeField] private CustomButton _clickButton;
        [SerializeField] private Image _icon;

        private Action<string> _loadInfoAction;

        public UniTask LoadUI(ArtifactType buffInGameType, int level, Action<string> loadInfoAction)
        {
            _loadInfoAction = loadInfoAction;
            _icon.gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }

        public void ClearUI(Action<string> loadInfoAction)
        {
            _loadInfoAction = loadInfoAction;
            _icon.gameObject.SetActive(false);
        }

        public void ToggleSelect(bool value)
        {
            _clickButton.ToggleSelect(value);
        }
    }
}
