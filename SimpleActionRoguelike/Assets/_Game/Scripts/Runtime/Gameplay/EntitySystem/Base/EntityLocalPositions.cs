using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityLocalPositions : MonoBehaviour
    {
        [SerializeField] private Transform _bottom;
        [SerializeField] private Transform _middle;
        [SerializeField] private Transform _top;

        public Transform Bottom => _bottom;
        public Transform Middle => _middle;
        public Transform Top => _top;
    }

}