using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public enum UpdateCurrentArtifactType
    {
        Removed,
        Added,
        LevelUp
    }

    public readonly struct UpdateCurrentArtifactMessage : IMessage
    {
        public readonly IArtifactSystem UpdatedArtifact;
        public readonly UpdateCurrentArtifactType UpdateArtifactType;

        public UpdateCurrentArtifactMessage(IArtifactSystem artifactSystem, UpdateCurrentArtifactType updateCurrentArtifactType)
        {
            UpdatedArtifact = artifactSystem;
            UpdateArtifactType = updateCurrentArtifactType;
        }
    }


    public enum UpdatedCurrentCollectedArtifactType
    {
        Add,
        Used,
    }

    public readonly struct UpdateCurrentCollectedArtifactMessage : IMessage
    {
        public readonly ArtifactType ArtifactType;
        public readonly UpdatedCurrentCollectedArtifactType UpdatedCurrentCollectedArtifactType;

        public UpdateCurrentCollectedArtifactMessage(ArtifactType artifactType, UpdatedCurrentCollectedArtifactType updatedCurrentCollectedArtifactType)
        {
            ArtifactType = artifactType;
            UpdatedCurrentCollectedArtifactType = updatedCurrentCollectedArtifactType;
        }
    }
}