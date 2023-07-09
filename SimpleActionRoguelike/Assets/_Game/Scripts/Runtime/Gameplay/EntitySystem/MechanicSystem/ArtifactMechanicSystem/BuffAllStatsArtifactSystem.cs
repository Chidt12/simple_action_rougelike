using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class BuffAllStatsArtifactSystem : RuneArtifactSystem<BuffAllStatsArtifactDataConfigItem>
    {
        public override ArtifactType ArtifactType => ArtifactType.BuffAllStats;
    }
}