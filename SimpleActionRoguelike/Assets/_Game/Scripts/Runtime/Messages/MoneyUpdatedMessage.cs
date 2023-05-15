using Runtime.Definition;
using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public readonly struct MoneyUpdatedMessage : IMessage
    {
        public readonly MoneyType MoneyType;
        public readonly long DeltaValue;

        public MoneyUpdatedMessage(MoneyType moneyType, long deltaValue)
        {
            MoneyType = moneyType;
            DeltaValue = deltaValue;
        }
    }

    public readonly struct MoneyInGameUpdatedMessage : IMessage
    {
        public readonly InGameMoneyType InGameMoneyType;
        public readonly long DeltaValue;

        public MoneyInGameUpdatedMessage(InGameMoneyType moneyType, long deltaValue)
        {
            InGameMoneyType = moneyType;
            DeltaValue = deltaValue;
        }
    }
}
