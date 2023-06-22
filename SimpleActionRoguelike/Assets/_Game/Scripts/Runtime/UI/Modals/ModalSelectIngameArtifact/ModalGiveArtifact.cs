using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Gameplay.EntitySystem;
using Runtime.Manager;
using System;
using UnityEngine;

namespace Runtime.UI
{
    public class ModalGiveArtifactData
    {
        public readonly IEntityData EntityData;
        public readonly ArtifactIdentity[] Items;
        public readonly Action<ArtifactIdentity> OnSelectArtifact;

        public ModalGiveArtifactData(IEntityData entityData, ArtifactIdentity[] items, Action<ArtifactIdentity> onSelectItemBuff)
        {
            EntityData = entityData;
            Items = items;
            OnSelectArtifact = onSelectItemBuff;
        }
    }

    public class ModalGiveArtifact : Modal<ModalGiveArtifactData>
    {
        [SerializeField] private GiveArtifactItemUI[] _itemUIs;
        [SerializeField] private InventoryPanel _inventoryPanel;

        private bool _isSelected;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            _itemUIs = GetComponentsInChildren<GiveArtifactItemUI>();
        }
#endif

        public override async UniTask Initialize(ModalGiveArtifactData data)
        {
            _isSelected = false;
            GameManager.Instance.SetGameStateType(Definition.GameStateType.GameplayChoosingItem);

            _inventoryPanel.LoadUI().Forget();

            for (int i = 0; i < data.Items.Length; i++)
            {
                await _itemUIs[i].Init(data.EntityData, data.Items[i], (input) =>
                {
                    if (!_isSelected)
                    {
                        _isSelected = true;
                        data.OnSelectArtifact?.Invoke(input);
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