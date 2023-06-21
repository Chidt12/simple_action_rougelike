using Runtime.Definition;
using System;
using UnityEngine;

namespace Runtime.ConfigModel
{
    [Serializable]
    public struct ArtifactIdentity
    {
        public ArtifactType artifactType;
        public int level;

        public ArtifactIdentity(ArtifactType artifactType, int level)
        {
            this.artifactType = artifactType;
            this.level = level;
        }
    }

    [Serializable]
    public struct ArtifactStageLoadConfigItem
    {
        public ArtifactIdentity identity;
        public float weight;
    }
    public class ArtifactStageLoadConfig : ScriptableObject
    {
        public ArtifactStageLoadConfigItem[] items;
    }
}
