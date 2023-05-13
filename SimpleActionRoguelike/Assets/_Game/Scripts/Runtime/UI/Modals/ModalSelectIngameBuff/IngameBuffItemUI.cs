using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class IngameBuffItemUI : MonoBehaviour
    {
        [SerializeField] private Button _selectButton;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;

        public void Init(Action selectAction)
        {
            _selectButton.onClick.RemoveAllListeners();
            _selectButton.onClick.AddListener(() =>
            {
                selectAction?.Invoke();
            });
        }
    }
}
