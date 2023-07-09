using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using System;

namespace Runtime.ConfigModel
{
    public enum StatusAroundSpawnPositionType
    {
        Center = 0,
        TowardDirection = 1,
        InPointerPoint = 2,
    }

    [Serializable]
    public class StatusArroundAreaArtifactDataConfigItem : RuneArtifactDataConfigItem
    {
        public override ArtifactType ArtifactType => ArtifactType.StatusAroundArea;
        public float damageBonus;
        public DamageFactor[] damageFactors;
        public StatusIdentity triggeredStatus;
        public float damageBoxHeight;
        public float damageBoxWidth;
        public string damageBoxPrefabName;
        public StatusAroundSpawnPositionType statusAroundSpawnPosition;
    }

    public class StatusAroundAreaArtifactDataConfig : ArtifactDataConfig<StatusArroundAreaArtifactDataConfigItem>
    {
        protected async override UniTask<string> GetDescription(IEntityData entityData, StatusArroundAreaArtifactDataConfigItem itemData, StatusArroundAreaArtifactDataConfigItem previousItemData)
        {
            return string.Empty;
        }
    }
}