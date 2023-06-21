using Runtime.Definition;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public static class ArtifactSystemFactory
    {
        public static IArtifactSystem GetArtifactSystem(ArtifactType artifactType)
        {
            Type elementType = Type.GetType($"Runtime.Gameplay.EntitySystem.{artifactType}ArtifactSystem");
            IArtifactSystem buffInGameSystem = Activator.CreateInstance(elementType) as IArtifactSystem;
            return buffInGameSystem;
        }
    }
}
