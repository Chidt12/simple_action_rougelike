using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Runtime.Definition;

namespace Runtime.UI
{
    public class InventoryArtifactItemUI : MonoBehaviour
    {
        [SerializeField] private CustomButton _clickButton;
        [SerializeField] private Image _icon;

        public async UniTask LoadUI(BuffInGameType buffInGameType, int dataId)
        {

        }
    }
}
