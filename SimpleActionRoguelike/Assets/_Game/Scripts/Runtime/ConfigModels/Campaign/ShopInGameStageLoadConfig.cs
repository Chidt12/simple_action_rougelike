using Runtime.Definition;
using System;
using UnityEngine;

namespace Runtime.ConfigModel
{
    [Serializable]
    public struct ShopInGameIdentity
    {
        public ShopInGameItemType shopInGameItemType;
        public int dataId;

        public ShopInGameIdentity(ShopInGameItemType shopInGameItemType, int dataId)
        {
            this.dataId = dataId;
            this.shopInGameItemType = shopInGameItemType;
        }
    }

    [Serializable]
    public struct ShopInGameStageLoadConfigItem
    {
        public ShopInGameIdentity identity;
        public ResourceData cost;
        public float weight;
        public int canAppear;
        public bool isPower;
    } 

    public class ShopInGameStageLoadConfig : ScriptableObject
    {
        public ShopInGameStageLoadConfigItem[] items;
    }
}
