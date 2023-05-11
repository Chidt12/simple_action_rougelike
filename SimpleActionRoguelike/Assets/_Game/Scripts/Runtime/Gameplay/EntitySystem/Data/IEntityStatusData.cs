using Runtime.Definition;
using System;
using System.Collections.Generic;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityStatusData : IEntityData
    {
        public Action UpdateCurrentStatus { get; set; }

        public StatusType CurrentState { get; }

        public List<IStatus> CurrentStatuses { get;}

        public bool CheckContainStatusInStack(StatusType[] statusTypes);
        public int GetNumberOfStatus(StatusType statusType);
        public void AddStatus(IStatus status);
        public void RemoveStatus(IStatus status);
        public void RemoveStatus(StatusType statusType);
        public void RemoveAllStatus();
    }
}