using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace Runtime.Gameplay.EntitySystem
{
    public class ScreenHome : Screen
    {
        public override UniTask Initialize(Memory<object> args)
        {
            Debug.LogError("Loaded");
            return base.Initialize(args);
        }
    }
}