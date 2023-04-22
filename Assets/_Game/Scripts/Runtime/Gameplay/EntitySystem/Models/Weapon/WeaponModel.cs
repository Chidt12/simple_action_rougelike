using Runtime.ConfigModel;
using Runtime.Definition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class WeaponModel 
    {
        public abstract WeaponType WeaponType { get; }
        public float DamageBonus { get; }
        public DamageFactor[] DamageFactors { get; }

        public WeaponModel()
        {

        }
    }

    public class WeaponData
    {
        public IWeaponDataConfigItem weaponConfigItem;
        public EquipmentMechanicDataConfigItem mechanicDataConfigItem;

        public WeaponData(IWeaponDataConfigItem weaponConfigItem, EquipmentMechanicDataConfigItem mechanicDataConfigItem)
        {
            this.weaponConfigItem = weaponConfigItem;
            this.mechanicDataConfigItem = mechanicDataConfigItem;
        }
    }
}