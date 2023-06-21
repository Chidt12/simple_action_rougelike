using Runtime.ConfigModel;
using Runtime.Definition;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Runtime.Gameplay.EntitySystem
{
    public class AutoStableGunArtifactSystem : ArtifactSystem<AutoStableGunArtifactDataConfigItem>
    {
        private CancellationTokenSource _cancellationTokenSource;

        public override ArtifactType ArtifactType => ArtifactType.AutoStableGun;


        public async override UniTask Init(IEntityData entityData)
        {
            await base.Init(entityData);
        }

        
    }
}