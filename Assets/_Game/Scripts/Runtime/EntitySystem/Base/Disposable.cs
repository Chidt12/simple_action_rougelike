using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class Disposable : MonoBehaviour, IDisposable
    {
        #region Properties

        protected bool HasDisposed { get; set; }

        #endregion Properties

        #region Class Methods

        public abstract void Dispose();

        #endregion Class Methods
    }
}