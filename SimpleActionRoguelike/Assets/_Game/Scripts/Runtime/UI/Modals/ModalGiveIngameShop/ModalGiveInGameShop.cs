using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Manager;
using System;
using UnityEngine;

namespace Runtime.UI
{
    public class ModalGiveInGameShopData
    {
        public readonly ShopInGameStageLoadConfigItem[] Items;
        public readonly Action<ShopInGameStageLoadConfigItem> OnSelectShopInGameItem;

        public ModalGiveInGameShopData(ShopInGameStageLoadConfigItem[] items, Action<ShopInGameStageLoadConfigItem> onSelectShopInGameItem)
        {
            OnSelectShopInGameItem = onSelectShopInGameItem;
            Items = items;
        }
    }

    public class ModalGiveInGameShop : Modal<ModalGiveInGameShopData>
    {
        [SerializeField] private GiveShopInGameUI[] _itemUIs;
        [SerializeField] private InventoryPanel _inventoryPanel;

        private bool _isSelected;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            _itemUIs = GetComponentsInChildren<GiveShopInGameUI>();
        }
#endif

        public override async UniTask Initialize(ModalGiveInGameShopData data)
        {
            _isSelected = false;
            GameManager.Instance.SetGameStateType(Definition.GameStateType.GameplayChoosingItem);

            _inventoryPanel.LoadUI().Forget();

            for (int i = 0; i < data.Items.Length; i++)
            {
                await _itemUIs[i].Init(data.Items[i], (input) => 
                {
                    if (!_isSelected)
                    {
                        _isSelected = true;
                        data.OnSelectShopInGameItem?.Invoke(input);
                        ScreenNavigator.Instance.PopModal(true).Forget();
                    }
                });
            }

            for (int i = 0; i < _itemUIs.Length; i++)
            {
                _itemUIs[i].gameObject.SetActive(i < data.Items.Length);
            }
        }

        public override UniTask Cleanup()
        {
            GameManager.Instance.ReturnPreviousGameStateType();
            return base.Cleanup();
        }
    }
}