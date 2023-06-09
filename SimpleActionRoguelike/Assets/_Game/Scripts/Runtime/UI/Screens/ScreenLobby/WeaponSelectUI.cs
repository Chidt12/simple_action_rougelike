using Runtime.Definition;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class WeaponSelectUI : MonoBehaviour
    {
        [SerializeField] private WeaponType _weaponType;
        [SerializeField] private Image _weaponIcon;
        [SerializeField] private WeaponSelectIcon[] _icons;

        public WeaponType WeaponType => _weaponType;

        private void Awake()
        {
            _weaponIcon.sprite = _icons.FirstOrDefault(x => x.weaponType == _weaponType).icon;
        }

        [Serializable]
        public struct WeaponSelectIcon
        {
            public WeaponType weaponType;
            public Sprite icon;
        }
    }
}