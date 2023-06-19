using Runtime.ConfigModel;
using Runtime.Definition;
using Runtime.Manager.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class CauseStatusAfterHitsShopInGameItem : ShopInGameItem<CauseStatusAfterHitsShopInGameDataConfigItem>, IFinalDamagedModifier
    {
        private int _currentHitNumber;

        protected override void Apply()
        {
            _currentHitNumber = 0;
            GameplayManager.Instance.MessageCenter.AddFinalDamageCreatedModifier(this);
        }

        public override void Remove()
        {
            GameplayManager.Instance.MessageCenter.RemoveFinalDamageCreatedModifier(this);
        }

        public float Finalize(float damageCreated, IEntityData receiver)
        {

            return damageCreated;
        }
    }
}
