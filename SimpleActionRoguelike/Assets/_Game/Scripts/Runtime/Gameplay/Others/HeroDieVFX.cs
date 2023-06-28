using Runtime.Core.Message;
using Runtime.Message;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class HeroDieVFX : Disposable
    {
        public override void Dispose() { }

        private void OnEnable()
        {
            SimpleMessenger.Publish(new PresentEndGameCameraMessage(transform));
        }
    }
}
