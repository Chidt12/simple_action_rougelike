using Cysharp.Threading.Tasks;
using Runtime.Manager;
using Runtime.Message;
using System;
using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public class ModalGameplayInventory : BaseModal
    {
        [SerializeField] private InventoryPanel _inventoryPanel;

        public async override UniTask Initialize(Memory<object> args)
        {
            await base.Initialize(args);
            GameManager.Instance.SetGameStateType(Definition.GameStateType.GameplayPausing);
            await _inventoryPanel.LoadUI();
        }

        public override UniTask Cleanup()
        {
            GameManager.Instance.ReturnPreviousGameStateType();
            return base.Cleanup();
        }

        protected override void OnKeyPress(InputKeyPressMessage message)
        {
            base.OnKeyPress(message);
            _inventoryPanel.OnKeyPress(message);
        }
    }
}