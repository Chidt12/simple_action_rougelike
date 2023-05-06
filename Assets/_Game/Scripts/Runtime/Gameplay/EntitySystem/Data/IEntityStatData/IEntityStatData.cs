using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityStatData : IEntityData
    {
        public HealthEntityStat HealthStat { get; }
        public ShieldEntityStat ShieldStat { get; }

        bool TryGetStat(StatType statType, out EntityStat statValue);
        float GetTotalStatValue(StatType statType);
        float GetBaseStatValue(StatType statType);
    }
}
