using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class CreateBuffFlagArtifactSystem : ArtifactSystem<CreateBuffFlagArtifactDataConfigItem>
    {
        public override ArtifactType ArtifactType => ArtifactType.CreateBuffFlag;

        public override void Trigger()
        {
            
        }
    }
}