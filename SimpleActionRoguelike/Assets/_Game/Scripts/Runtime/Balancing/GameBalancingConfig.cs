using UnityEngine;

namespace Runtime.Gameplay.Balancing
{
    [CreateAssetMenu(fileName = "GameBalancingConfig", menuName = "Balancing/GameBalancingConfig")]
    public class GameBalancingConfig : ScriptableObject
    {
        public AnimationCurve heroPointsVsMapPoints;
        public int minWave = 2; // boss only have 1 wave.
        public int maxWave = 5;
        public int minEnemyTypes = 2;
        public int maxEnemyTypes = 4;
    }
}
