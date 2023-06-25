using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class CreateBuffFlagArtifactDataConfigItem : RuneArtifactDataConfigItem
    {
        public override ArtifactType ArtifactType => ArtifactType.CreateBuffFlag;
        public float rangeWidth;
        public float rangeHeight;
        public EquipmentStat[] buffStats;
        public string flagPrefabName;
        public string buffVfxPrefabName;
    }

    public class CreateBuffFlagArtifactDataConfig : ArtifactDataConfig<CreateBuffFlagArtifactDataConfigItem>
    {
        protected override UniTask<string> GetDescription(IEntityData entityData, CreateBuffFlagArtifactDataConfigItem itemData, CreateBuffFlagArtifactDataConfigItem previousItemData)
        {
            return UniTask.FromResult(string.Empty);
        }
    }
}
