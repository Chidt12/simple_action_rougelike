using Runtime.Definition;
using System.Collections.Generic;

namespace Runtime.Manager.Data
{
    public sealed class TransientData
    {
        private Dictionary<InGameMoneyType, long> _inGameMoneyCollection;

        public TransientData()
        {
            _inGameMoneyCollection = new();
        }

        public long GetGameMoneyType(InGameMoneyType moneyType)
        {
            if(_inGameMoneyCollection.TryGetValue(moneyType, out var moneyValue))
                return moneyValue;
            return 0;
        }

        public void AddMoney(InGameMoneyType moneyType, long value)
        {
            if (_inGameMoneyCollection.ContainsKey(moneyType))
                _inGameMoneyCollection[moneyType] += value;
            else
                _inGameMoneyCollection.Add(moneyType, value);
        }

        public void RemoveMoney(InGameMoneyType moneyType, long value)
        {
            if (_inGameMoneyCollection.ContainsKey(moneyType))
                _inGameMoneyCollection[moneyType] -= value;
        }

        public void ClearInGameMoney() => _inGameMoneyCollection.Clear();
    }
}