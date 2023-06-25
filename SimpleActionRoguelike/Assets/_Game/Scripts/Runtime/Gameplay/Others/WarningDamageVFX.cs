using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class WarningDamageVFX : MonoBehaviour
    {
        [SerializeField] private GameObject _graphic;
        [SerializeField] private DamageBox _damageBox;

        public void Init(Vector2 scale)
        {
            transform.localScale = scale;
            _graphic.SetActive(true);
            _damageBox.gameObject.SetActive(false);
        }

        public void InitDamageBox(Action<IEntityData> onTriggeredEntered, Action<IEntityData> onTriggeredExit = null)
        {
            _damageBox.gameObject.SetActive(true);
            _damageBox.StartDamage(onTriggeredEntered, onTriggeredExit);
        }

        public void DisableGraphic()
        {
            _graphic.SetActive(false);
        }
    }
}