using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Runtime.Gameplay.TextDamage
{
    public class TextDamage : FloatingText
    {
        [SerializeField]
        private TextMeshPro _damageText;

        public int Index { get; set; }

        public void Init(float value, Vector2 spawnPosition)
        {
            transform.localPosition = spawnPosition;
            _damageText.text = value > 0 ? $"+{Mathf.Floor(value)}" : Mathf.Floor(value).ToString();
        }

        protected override void OnComplete()
        {
            TextDamageController.Instance.Despawn(this);
        }
    }
}
