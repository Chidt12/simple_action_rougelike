using Runtime.Definition;
using System;
using System.Linq;
using UnityEngine;

namespace Runtime.ConfigModel
{
    [Serializable]
    public struct DeathDataIdentity
    {
        public int deathDataId;
        public DeathType deathType;

        public DeathDataIdentity(int deathDataId, DeathType deathType)
        {
            this.deathDataId = deathDataId;
            this.deathType = deathType;
        }
    }

    public static class DeathConfigTypeExtensions
    {
        public static bool IsSpawnedEnemy(this DeathType deathConfigType)
        {
            switch (deathConfigType)
            {
                case DeathType.SpawnEntities:
                case DeathType.SpawnContainEntitiesProjectiles:
                    return true;
                default:
                    return false;
            }
        }
    }

    public class DeathDataConfigItem
    {
        public int dataId;
    }

    public abstract class DeathDataConfig : ScriptableObject
    {
        public abstract DeathDataConfigItem[] Items { get; }
    }

    public class DeathDataConfig<T> : DeathDataConfig where T : DeathDataConfigItem
    {
        public T[] items;

        public override DeathDataConfigItem[] Items
            => items.Cast<DeathDataConfigItem>().ToArray();
    }
}