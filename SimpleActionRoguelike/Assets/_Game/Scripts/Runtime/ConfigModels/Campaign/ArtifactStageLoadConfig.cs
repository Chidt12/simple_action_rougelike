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
        public int dataId;

        public ArtifactIdentity(ArtifactType artifactType, int level, int dataId)
        {
            this.artifactType = artifactType;
            this.level = level;
            this.dataId = dataId;
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
