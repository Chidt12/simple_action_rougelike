using Runtime.Definition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class StatItemUI : MonoBehaviour
    {
        [SerializeField] private StatType _statType;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _value;
        [SerializeField] private StatItemIcon[] _icons;

        public StatType StatType => _statType;

        private void Awake()
        {
            _icon.sprite = _icons.FirstOrDefault(x => x.statType == _statType).icon;
        }

        public void SetValue(string value) => _value.text = value;

        [Serializable]
        public struct StatItemIcon
        {
            public StatType statType;
            public Sprite icon;
        }
    }
}