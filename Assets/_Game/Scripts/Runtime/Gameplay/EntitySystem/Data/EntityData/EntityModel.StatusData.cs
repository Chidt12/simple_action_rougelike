using Runtime.Definition;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntityStatusData
    {
        protected StatusType currentState;
        protected List<IStatus> statuses;

        public Action UpdateCurrentStatus { get; set; }

        public StatusType CurrentState => currentState;

        public List<IStatus> CurrentStatuses => statuses;

        public void InitStatus()
        {
            statuses = new();
            currentState = StatusType.None;
            UpdateCurrentStatus = () => { };
        }

        public bool CheckContainStatusInStack(StatusType[] statusTypes)
        {
            return CurrentStatuses.Any(x => statusTypes.Contains(x.StatusType));
        }

        public int GetNumberOfStatus(StatusType statusType)
        {
            return CurrentStatuses.Count(x => x.StatusType == statusType);
        }

        public void AddStatus(IStatus status)
        {
            statuses.Add(status);

            if (UpdateCurrentState())
                UpdateCurrentStatus.Invoke();
        }

        public void RemoveStatus(IStatus status)
        {
            statuses.Remove(status);
            status.Dispose();

            if (UpdateCurrentState())
                UpdateCurrentStatus.Invoke();
        }

        public void RemoveStatus(StatusType statusType)
        {
            var removedStatuses = statuses.Where(x => x.StatusType == statusType);
            foreach (var removedStatus in removedStatuses)
                removedStatus.Dispose();

            statuses.RemoveAll(x => x.StatusType == statusType);

            if(UpdateCurrentState())
                UpdateCurrentStatus.Invoke();
        }

        public void RemoveAllStatus()
        {
            foreach (var status in statuses)
                status.Dispose();
            statuses.Clear();

            if (UpdateCurrentState())
                UpdateCurrentStatus.Invoke();
        }

        private bool UpdateCurrentState()
        {
            if (statuses.Count <= 0)
            {
                if(currentState != StatusType.None)
                {
                    currentState = StatusType.None;
                    return true;
                }
            }
                
            else
            {
                var statusType = statuses.Min(x => x.StatusType);
                if(currentState != statusType)
                {
                    currentState = statusType;
                    return true;
                }
            }

            return false;
        }
    }
}
