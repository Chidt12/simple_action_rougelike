using Runtime.Core.Message;
using Runtime.Definition;
using Runtime.Helper;
using Runtime.Manager.Data;
using Runtime.Message;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZBase.Foundation.PubSub;

namespace Runtime.UI
{
    public class CurrencyElement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _valueText;
        [SerializeField] private Image _icon;
        [SerializeField] private bool _inGameMoney;

        [SerializeField] private MoneyType _moneyType;
        [SerializeField] private InGameMoneyType _inGameMoneyType;

        private ISubscription _subScription;

        private void Awake()
        {
            if (_inGameMoney)
            {
                _subScription = SimpleMessenger.Subscribe<MoneyInGameUpdatedMessage>(OnInGameMoneyUpdated);
                UpdateInGameUI();
            }
            else
            {
                _subScription = SimpleMessenger.Subscribe<MoneyUpdatedMessage>(OnMoneyUpdated);
                UpdateMoneyUI();
            }
        }

        private void OnMoneyUpdated(MoneyUpdatedMessage message)
        {
            if(message.MoneyType == _moneyType)
            {
                UpdateMoneyUI();
            }

        }

        private void OnInGameMoneyUpdated(MoneyInGameUpdatedMessage message)
        {
            if(message.InGameMoneyType == _inGameMoneyType)
            {
                UpdateInGameUI();
            }
        }

        private void UpdateMoneyUI()
        {
            
        }

        private void UpdateInGameUI()
        {
            var currentResourceValue = DataManager.Transient.GetGameMoneyType(_inGameMoneyType);
            _valueText.text = currentResourceValue.ToDisplayString();
        }


        private void OnDestroy()
        {
            _subScription.Dispose();
        }
    }
}