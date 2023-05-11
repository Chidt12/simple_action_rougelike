using Runtime.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public enum EffectSource
    {
        None,
        FromNormalAttack,
        FromSkill,
    }

    public enum EffectProperty
    {
        Normal,
        Crit,
        Poison,
        Fire
    }

    public class DamageInfo
    {
        public float damage;
        public float armorPenetration;
        public IEntityData creatorData;
        public IEntityData targetData;
        public EffectSource damageSource;
        public EffectProperty damageProperty;

        public DamageInfo(EffectSource damageSource, float damage, float armorPenetration, IEntityData creatorData, IEntityData targetData, EffectProperty damageProperty)
        {
            this.damageSource = damageSource;
            this.damage = damage;
            this.creatorData = creatorData;
            this.targetData = targetData;
            this.damageProperty = damageProperty;
            this.armorPenetration = armorPenetration;
        }

        public DamageInfo(EffectSource damageSource, float damage, IEntityData creatorData, IEntityData targetData, EffectProperty damageProperty)
        {
            this.damageSource = damageSource;
            this.damage = damage;
            this.creatorData = creatorData;
            this.targetData = targetData;
            this.damageProperty = damageProperty;
            armorPenetration = 0.0f;
        }
    }

    public static class DamageFactorExtensions
    {
        public static DamageFactor[] Add(this DamageFactor[] a, DamageFactor[] b)
        {
            if (a == null)
                return b;

            if (b == null)
                return a;

            var listC = new List<DamageFactor>();
            var listB = b.ToList();

            if (a.Length > 0)
            {
                for (int i = 0; i < a.Length; i++)
                {
                    var sameTypeFactor = b.FirstOrDefault(x => x.damageFactorStatType == a[i].damageFactorStatType);
                    var damageFactorValue = a[i].damageFactorValue + sameTypeFactor.damageFactorValue;
                    listB.Remove(sameTypeFactor);
                    listC.Add(new DamageFactor(a[i].damageFactorStatType, damageFactorValue));
                }
            }

            foreach (var factor in listB)
                listC.Add(factor);

            return listC.ToArray();
        }

        public static DamageFactor[] Add(this DamageFactor[] a, float damagePercent)
        {
            if (a == null)
                return a;

            var listC = new List<DamageFactor>();
            for (int i = 0; i < a.Length; i++)
            {
                var damageFactorValue = a[i].damageFactorValue + damagePercent;
                listC.Add(new DamageFactor(a[i].damageFactorStatType, damageFactorValue));
            }

            return listC.ToArray();
        }

        public static DamageFactor[] Multiply(this DamageFactor[] a, float factor)
        {
            if (a == null)
                return a;

            var listC = new List<DamageFactor>();
            for (int i = 0; i < a.Length; i++)
            {
                var damageFactorValue = a[i].damageFactorValue * factor;
                listC.Add(new DamageFactor(a[i].damageFactorStatType, damageFactorValue));
            }

            return listC.ToArray();
        }
    }

    [Serializable]
    public struct DamageFactor
    {
        #region Members

        public StatType damageFactorStatType;
        public float damageFactorValue;

        #endregion Members

        #region Struct Methods

        public DamageFactor(StatType damageFactorStatType, float damageFactorValue)
        {
            this.damageFactorStatType = damageFactorStatType;
            this.damageFactorValue = damageFactorValue;
        }

        #endregion Struct Methods
    }

    public struct DamageMetaData
    {
        #region Members

        public Vector2 damageDirection;
        public Vector2 attractedPoint;

        #endregion Members

        #region Struct Methods

        public DamageMetaData(Vector2 damageDirection, Vector2 attractedPoint)
        {
            this.damageDirection = damageDirection;
            this.attractedPoint = attractedPoint;
        }

        #endregion Struct Methods
    }
}