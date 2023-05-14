using Runtime.Definition;
using System;
using UnityEngine;

namespace Runtime.ConfigModel
{
    [Serializable]
    public struct BuffInGameIdentity
    {
        public BuffInGameType buffInGameType;
        public int level;

        public BuffInGameIdentity(BuffInGameType buffInGameType, int level)
        {
            this.buffInGameType = buffInGameType;
            this.level = level;
        }
    }

    [Serializable]
    public struct BuffInGameStageLoadConfigItem
    {
        public BuffInGameIdentity identity;
        public float weight;
    }
    public class BuffInGameStageLoadConfig : ScriptableObject
    {
        public BuffInGameStageLoadConfigItem[] items;
    }
}
