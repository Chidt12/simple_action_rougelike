using Cysharp.Threading.Tasks;
using Runtime.Manager;
using System;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Modals;

namespace Runtime.UI
{
    public class ModalLose : Modal
    {
        [SerializeField] private Button _replayButton;

        public override UniTask Initialize(Memory<object> args)
        {
            _replayButton.onClick.AddListener(OnReplay);
            return base.Initialize(args);
        }

        private void OnReplay()
        {
            GameManager.Instance.Replay();
        }
    }
}