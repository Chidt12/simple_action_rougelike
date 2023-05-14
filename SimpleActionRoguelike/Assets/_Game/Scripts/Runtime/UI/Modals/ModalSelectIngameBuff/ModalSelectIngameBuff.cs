using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Gameplay.EntitySystem;
using System;
using UnityEngine;

namespace Runtime.UI
{
    public class ModalSelectIngameBuffData
    {
        public readonly IEntityData EntityData;
        public readonly BuffInGameIdentity[] Items;
        public readonly Action<BuffInGameIdentity> OnSelectItemBuff;

        public ModalSelectIngameBuffData(IEntityData entityData, BuffInGameIdentity[] items, Action<BuffInGameIdentity> onSelectItemBuff)
        {
            EntityData = entityData;
            Items = items;
            OnSelectItemBuff = onSelectItemBuff;
        }
    }

    public class ModalSelectIngameBuff : Modal<ModalSelectIngameBuffData>
    {
        [SerializeField] private IngameBuffItemUI[] _itemUIs;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            _itemUIs = GetComponentsInChildren<IngameBuffItemUI>();
        }
#endif

        public async override UniTask Initialize(ModalSelectIngameBuffData data)
        {
            for (int i = 0; i < data.Items.Length; i++)
            {
                await _itemUIs[i].Init(data.EntityData, data.Items[i], data.OnSelectItemBuff);
            }

            for (int i = 0; i < _itemUIs.Length; i++)
            {
                _itemUIs[i].gameObject.SetActive(i < data.Items.Length);
            }
        }
    }
}