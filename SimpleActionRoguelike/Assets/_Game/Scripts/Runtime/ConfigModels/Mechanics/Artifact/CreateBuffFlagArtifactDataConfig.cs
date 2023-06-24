using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class CreateBuffFlagArtifactDataConfigItem : ArtifactDataConfigItem
    {
        public override ArtifactType ArtifactType => ArtifactType.CreateBuffFlag;
        public float range;
        public EquipmentStat buffStat;
        public string flagPrefabName;
    }

    public class CreateBuffFlagArtifactDataConfig : ArtifactDataConfig<CreateBuffFlagArtifactDataConfigItem>
    {
        protected override UniTask<string> GetDescription(IEntityData entityData, CreateBuffFlagArtifactDataConfigItem itemData, CreateBuffFlagArtifactDataConfigItem previousItemData)
        {
            return UniTask.FromResult(string.Empty);
        }
    }
}
