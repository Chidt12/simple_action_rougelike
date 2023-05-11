using Runtime.Definition;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public class WeaponModelFactory
    {
        public static WeaponModel GetWeaponModel(WeaponType weaponType, WeaponData weaponData)
        {
            Type elementType = Type.GetType($"Runtime.Gameplay.EntitySystem.{weaponType}WeaponModel");
            WeaponModel weaponModel = Activator.CreateInstance(elementType, weaponData) as WeaponModel;
            return weaponModel;
        }
    }
}
