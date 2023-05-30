using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Runtime.Manager.Gameplay
{
    [Serializable]
    [CreateAssetMenu(fileName = "MapLevel", menuName = "Map/MapLevel")]
    public class MapLevelScriptableObject : ScriptableObject
    {
        public string prefabName;
        public int point;
        public float limitDistanceToHero;
        /// <summary>
        /// use this second for check enemy;
        /// </summary>
        public string[] exceptEnemyIds;
        /// <summary>
        /// use this first for check enemy.
        /// </summary>
        public string[] includedEnemyIds;
    }

}