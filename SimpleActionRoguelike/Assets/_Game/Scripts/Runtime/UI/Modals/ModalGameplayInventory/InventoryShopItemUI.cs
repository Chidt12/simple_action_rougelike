using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Runtime.Definition;
using System;

namespace Runtime.UI
{
    public class InventoryShopItemUI : MonoBehaviour
    {
        [SerializeField] private CustomButton _clickButton;
        [SerializeField] private Image _icon;

        private Action<string> _loadInfoAction;

        public UniTask LoadUI(ShopInGameItemType shopItemType, int dataId, Action<string> loadInfoAction)
        {
            _loadInfoAction = loadInfoAction;
            return UniTask.CompletedTask;
        }
    }
}