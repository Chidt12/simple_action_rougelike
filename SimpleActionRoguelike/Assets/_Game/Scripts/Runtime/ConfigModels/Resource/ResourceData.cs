using Runtime.Definition;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public struct ResourceData
    {
        public int resourceId;
        public ResourceType resourceType;
        public int resourceNumber;

        public ResourceData(ResourceType resourceType, int resourceId, int resourceNumber)
        {
            this.resourceType = resourceType;
            this.resourceId = resourceId;
            this.resourceNumber = resourceNumber;
        }
    }
}