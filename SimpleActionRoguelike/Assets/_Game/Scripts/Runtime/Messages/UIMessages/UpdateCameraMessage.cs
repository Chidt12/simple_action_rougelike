using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public readonly struct UpdateCameraMessage : IMessage
    {
        public readonly Camera Camera;

        public UpdateCameraMessage(Camera camera)
        {
            Camera = camera;
        }
    }

    public readonly struct PresentEndGameCameraMessage : IMessage
    {
        public readonly Transform FocusTarget;

        public PresentEndGameCameraMessage(Transform target)
        {
            FocusTarget = target;
        }
    }
}
