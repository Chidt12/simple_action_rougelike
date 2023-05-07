using Runtime.ConfigModel;
using Runtime.Definition;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public static class StatusModelFactory
    {
        public static StatusModel GetStatusModel(StatusType statusType, StatusDataConfigItem configItem)
        {
            Type elementType = Type.GetType($"Runtime.Gameplay.EntitySystem.{statusType}StatusModel");
            StatusModel statusModel = Activator.CreateInstance(elementType, configItem) as StatusModel;
            return statusModel;
        }
    }
}