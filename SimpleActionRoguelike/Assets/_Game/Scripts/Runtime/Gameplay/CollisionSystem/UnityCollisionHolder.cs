using System;
using UnityEngine;

namespace Runtime.Gameplay.CollisionDetection
{
    [RequireComponent(typeof(Collider2D))]
    public class UnityCollisionHolder : MonoBehaviour
    {
        public Action<Collider2D> OnCollisionEntered { get; set; }
        public Action<Collider2D> OnCollisionExited { get; set; }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            OnCollisionEntered?.Invoke(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            OnCollisionExited?.Invoke(collision);
        }
    }
}