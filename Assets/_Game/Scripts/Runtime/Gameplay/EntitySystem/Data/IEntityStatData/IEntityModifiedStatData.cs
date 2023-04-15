namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityModifiedStatData : IEntityStatData
    {
        void BuffStat(StatType statType, float value, StatModifyType statModifyType);
        void DebuffStat(StatType statType, float value, StatModifyType statModifyType);
    }
}
