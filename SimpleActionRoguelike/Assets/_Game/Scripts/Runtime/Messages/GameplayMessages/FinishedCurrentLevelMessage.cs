using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public readonly struct FinishedCurrentLevelMessage : IMessage
    {
        public readonly bool IsWin;

        public FinishedCurrentLevelMessage(bool isWin)
        {
            IsWin = isWin;
        }
    }
}