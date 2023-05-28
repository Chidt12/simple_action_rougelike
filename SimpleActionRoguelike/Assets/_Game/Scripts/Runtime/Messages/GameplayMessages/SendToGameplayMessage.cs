using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public enum SendToGameplayType
    {
        BuyShop,
        GiveArtifact,
        GiveShopItem,
        GoNextStage,
    }

    public readonly struct SendToGameplayMessage : IMessage
    {
        public readonly SendToGameplayType SendToGameplayType;
        public readonly object Data;

        public SendToGameplayMessage(SendToGameplayType sendToGameplayType, object args = null)
        {
            SendToGameplayType = sendToGameplayType;
            Data = args;
        }
    }
}