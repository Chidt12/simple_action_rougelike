using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public readonly struct FinishedCurrentLevelMessage : IMessage
    {
        public readonly bool IsWin;
        public readonly bool IsFinished;

        public FinishedCurrentLevelMessage(bool isWin, bool isFinished)
        {
            IsWin = isWin;
            IsFinished = isFinished;
        }
    }
}