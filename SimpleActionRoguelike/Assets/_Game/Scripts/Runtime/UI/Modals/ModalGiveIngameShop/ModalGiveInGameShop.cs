using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Manager;
using System;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private Button _closeButton;
        [SerializeField] private GiveShopInGameUI[] _itemUIs;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            _itemUIs = GetComponentsInChildren<GiveShopInGameUI>();
        }
#endif

        public override async UniTask Initialize(ModalGiveInGameShopData data)
        {
            GameManager.Instance.SetGameStateType(Definition.GameStateType.GameplayPausing);
            _closeButton.onClick.AddListener(() => ScreenNavigator.Instance.PopModal(true).Forget());

            for (int i = 0; i < data.Items.Length; i++)
            {
                await _itemUIs[i].Init(data.Items[i], data.OnSelectShopInGameItem);
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