namespace Runtime.Gameplay.EntitySystem
{
    public interface IPostCalculateDamageModifier : IPriorityModifier
    {

        DamageInfo Calculate(DamageInfo damageInfo);
    }
}