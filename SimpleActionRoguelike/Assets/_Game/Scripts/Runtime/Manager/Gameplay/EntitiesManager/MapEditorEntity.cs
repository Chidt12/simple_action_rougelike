using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class MapEditorEntity : MonoBehaviour
    {
        #region Members

        [SerializeField]
        private EntityType _entityType = EntityType.Object;
        [SerializeField]
        private int _level;
        [SerializeField]
        private int _entityId;
        [SerializeField]
        private int _waveActive = 0;
        [SerializeField]
        private float _delaySpawnInWave = 0;

        #endregion Members

        #region Properties
        public int Level => _level;
        public EntityType EntityType => _entityType;
        public int EntityId => _entityId;
        public int WaveActive => _waveActive;
        public float DelaySpawnInWave => _delaySpawnInWave;
        public bool ShouldDisable => _waveActive != 0 || _delaySpawnInWave > 0;

        #endregion Properties
    }
}