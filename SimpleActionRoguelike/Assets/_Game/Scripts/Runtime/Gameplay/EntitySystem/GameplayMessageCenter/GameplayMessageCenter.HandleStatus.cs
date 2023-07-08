using Runtime.Definition;
using Runtime.Manager.Data;
using Runtime.Message;
using System.Collections.Generic;
using System.Linq;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class GameplayMessageCenter 
    {
        private partial void OnSentStatusEffect(SentStatusEffectMessage message)
        {
            var target = message.Target;
            if (target == null || target.IsDead)
                return;

            // Data should be preloaded somewhere (ex: GameplayDataManager)
            if (message.StatusIdentity.statusType == StatusType.None)
                return;

            var statusDataConfigItem = DataManager.Config.GetStatusDataConfig(message.StatusIdentity.statusType, message.StatusIdentity.statusDataId);
            var newStatusModel = StatusModelFactory.GetStatusModel(message.StatusIdentity.statusType, statusDataConfigItem);
            if (newStatusModel.IsAffectable)
            {
                var newStatusEffectType = newStatusModel.StatusType;
                var newStatusEffectPriority = GetStatusPriorityType(newStatusEffectType);

                // Status priority 0 will be applied no matter what.
                if (newStatusEffectPriority != StatusPriorityType.Priority0)
                {
                    var highestPriority = StatusPriorityType.Priority0;
                    if (target.CurrentStatuses.Count > 0)
                        highestPriority = target.CurrentStatuses.Max(x => GetStatusPriorityType(x.StatusType));

                    // Status which has lower priority will not be applied.
                    if (newStatusEffectPriority < highestPriority)
                        return;

                    // Status priority 3 will not remove other status.
                    if (newStatusEffectPriority != StatusPriorityType.Priority3)
                    {
                        var removedStatuses = new List<IStatus>();
                        // Remove status same priority (1 or 2) and different type.
                        for (int i = target.CurrentStatuses.Count - 1; i >= 0; i--)
                        {
                            if (target.CurrentStatuses[i].StatusType != newStatusEffectType && GetStatusPriorityType(target.CurrentStatuses[i].StatusType) == newStatusEffectPriority)
                                removedStatuses.Add(target.CurrentStatuses[i]);
                        }

                        foreach (var removedStatus in removedStatuses)
                            target.RemoveStatus(removedStatus);
                    }
                }

                var numberOfSameStatusEffects = target.GetNumberOfStatus(newStatusEffectType);
                if (newStatusModel.IsStackable)
                {
                    if(numberOfSameStatusEffects < newStatusModel.MaxStack)
                    {
                        // Add new status => not remove the old one.
                        var newStatus = StatusFactory.GetStatus(newStatusEffectType, newStatusModel, message.Creator, message.Target, message.StatusMetaData);
                        target.AddStatus(newStatus);
                    }
                }
                else
                {
                    // Stop all old same type status effects.
                    if (numberOfSameStatusEffects > 0)
                        target.RemoveStatus(newStatusModel.StatusType);

                    // Add new status
                    var newStatus = StatusFactory.GetStatus(newStatusEffectType, newStatusModel, message.Creator, message.Target, message.StatusMetaData);
                    target.AddStatus(newStatus);
                }
            }
        }

        public enum StatusPriorityType
        {
            Priority0 = 0,
            Priority1 = 1,
            Priority2 = 2,
            Priority3 = 3
        }

        private StatusPriorityType GetStatusPriorityType(StatusType statusEffectType)
        {
            switch (statusEffectType)
            {
                case StatusType.Pull:
                case StatusType.Knockback:
                    return StatusPriorityType.Priority3;

                case StatusType.Stun:
                case StatusType.Freeze:
                case StatusType.Root:
                    return StatusPriorityType.Priority2;

                case StatusType.Terror:
                case StatusType.Taunt:
                    return StatusPriorityType.Priority1;

                default:
                    return StatusPriorityType.Priority0;
            }
        }
    }
}