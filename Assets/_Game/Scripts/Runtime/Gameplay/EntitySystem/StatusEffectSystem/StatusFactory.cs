using Runtime.Definition;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public static class StatusFactory
    {
        public static IStatus GetStatus(StatusType statusType, StatusModel statusModel, IEntityData creator, IEntityStatusData owner, StatusMetaData metaData)
        {
            Type elementType = Type.GetType($"Runtime.Gameplay.EntitySystem.{statusType}Status");
            IStatus status = Activator.CreateInstance(elementType) as IStatus;
            status.Init(statusModel, creator, owner, metaData);
            return status;
        }
    }
}