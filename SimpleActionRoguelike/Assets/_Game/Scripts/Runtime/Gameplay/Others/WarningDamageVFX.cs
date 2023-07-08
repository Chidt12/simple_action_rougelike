using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class WarningDamageVFX : MonoBehaviour
    {
        public void Init(Vector2 scale)
        {
            transform.localScale = scale;
        }
    }
}