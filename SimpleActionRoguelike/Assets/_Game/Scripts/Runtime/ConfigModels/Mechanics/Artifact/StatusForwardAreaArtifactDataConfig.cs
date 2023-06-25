using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class StatusForwardAreaArtifactDataConfigItem : RuneArtifactDataConfigItem
    {
        public override ArtifactType ArtifactType => ArtifactType.StatusForwardArea;
        public float damageBonus;
        public DamageFactor[] damageFactors;
        public StatusIdentity triggeredStatus;
        public float distanceBetweenObject;
        public float numberOfObjects;
        public string forwardPrefabName;
        public float delayBetweenSpawn;
    }

    public class StatusForwardAreaArtifactDataConfig : ArtifactDataConfig<StatusForwardAreaArtifactDataConfigItem>
    {
        protected override UniTask<string> GetDescription(IEntityData entityData, StatusForwardAreaArtifactDataConfigItem itemData, StatusForwardAreaArtifactDataConfigItem previousItemData)
        {
            return UniTask.FromResult(string.Empty);
        }
    }
}