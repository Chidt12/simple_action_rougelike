using Runtime.Core.Pool;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class StatusVFX : MonoBehaviour, IDisposable
    {
        public void Dispose()
        {
            PoolManager.Instance.Return(gameObject);
        }
    }
}