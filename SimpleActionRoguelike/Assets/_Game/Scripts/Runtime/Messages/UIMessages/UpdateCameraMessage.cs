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
}
