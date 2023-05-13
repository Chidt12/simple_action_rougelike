using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core.Modals;

namespace Runtime.UI
{
    public class ModalSelectIngameBuffData
    {
        public readonly Action OnSelectItemBuff;

        public ModalSelectIngameBuffData(Action onSelectItemBuff)
        {
            OnSelectItemBuff = onSelectItemBuff;
        }
    }

    public class ModalSelectIngameBuff : Modal<ModalSelectIngameBuffData>
    {
        [SerializeField] private IngameBuffItemUI[] _items;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            _items = GetComponentsInChildren<IngameBuffItemUI>();
        }
#endif

        public override UniTask Initialize(ModalSelectIngameBuffData data)
        {
            foreach (var item in _items)
            {
                item.Init(data.OnSelectItemBuff);
            }

            return UniTask.CompletedTask;
        }
    }
}