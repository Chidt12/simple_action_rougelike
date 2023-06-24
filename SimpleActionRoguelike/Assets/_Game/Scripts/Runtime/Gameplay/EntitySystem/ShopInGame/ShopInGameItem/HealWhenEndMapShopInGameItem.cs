using Runtime.ConfigModel;
using Runtime.Core.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZBase.Foundation.PubSub;
using Runtime.Message;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public class HealWhenEndMapShopInGameItem : ShopInGameItem<HealWhenEndMapShopInGameDataConfigItem>
    {
        private ISubscription _subscription;

        public override void Remove()
        {
            _subscription.Dispose();
        }

        protected override void Apply()
        {
            _subscription = SimpleMessenger.Subscribe<FinishedCurrentLevelMessage>(OnFinishedCurrentLevel);
        }

        private void OnFinishedCurrentLevel(FinishedCurrentLevelMessage message)
        {
            if (message.IsWin)
            {
                owner.Heal(dataConfigItem.healAmount, EffectSource.None, EffectProperty.Normal);
            }
        }
    }
}