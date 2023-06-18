using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Runtime.Definition;

namespace Runtime.UI
{
    public class InventoryShopItemUI : MonoBehaviour
    {
        [SerializeField] private CustomButton _clickButton;
        [SerializeField] private Image _icon;

        public async UniTask LoadUI(ShopInGameItemType shopItemType, int dataId)
        {

        }
    }
}