namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityStatData : IEntityData
    {
        bool TryGetStat(StatType statType, out EntityStat statValue);
        float GetTotalStatValue(StatType statType);
        float GetBaseStatValue(StatType statType);
    }
}
