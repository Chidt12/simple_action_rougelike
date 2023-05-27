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
        [SerializeField]
        public int point;
        [SerializeField]
        public float limitDistanceToHero;

        /// <summary>
        /// use this second for check enemy;
        /// </summary>
        [SerializeField]
        public string[] exceptEnemyIds;

        /// <summary>
        /// use this first for check enemy.
        /// </summary>
        [SerializeField]
        public string[] includedEnemyIds;

#if UNITY_EDITOR
        private void OnValidate()
        {
            mapSpawnPoints = GetComponentsInChildren<MapSpawnPoint>();
        }
#endif 
    }
}
