namespace Runtime.Gameplay.EntitySystem
{
    public interface IPostCalculateDamageModifier
    {
        DamageInfo Calculate(DamageInfo damageInfo);
    }
}