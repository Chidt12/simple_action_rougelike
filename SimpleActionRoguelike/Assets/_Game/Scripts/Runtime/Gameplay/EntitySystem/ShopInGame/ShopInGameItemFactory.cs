using Runtime.Definition;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public static class ShopInGameItemFactory
    {
        public static ShopInGameItem GetShopInGameItem(ShopInGameItemType shopInGameType)
        {
            Type elementType = Type.GetType($"Runtime.Gameplay.EntitySystem.{shopInGameType}ShopInGameItem");
            ShopInGameItem projectileStrategy = Activator.CreateInstance(elementType) as ShopInGameItem;
            return projectileStrategy;
        }
    }
}
