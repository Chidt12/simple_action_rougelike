using Runtime.Gameplay.Manager;
using UnityEngine;

namespace Runtime.Manager.Gameplay
{
    [SerializeField]
    public class MapLevel : MonoBehaviour
    {
        [SerializeField]
        public MapSpawnPoint[] mapSpawnPoints;
        [SerializeField]
        public PolygonCollider2D confinder;

#if UNITY_EDITOR
        private void OnValidate()
        {
            mapSpawnPoints = GetComponentsInChildren<MapSpawnPoint>();
        }
#endif 
    }
}
