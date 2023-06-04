using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public class LoadingLayer : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _loadingText;
        [SerializeField]
        private Animator _animator;

        private bool _isPlayingAnimation;
        private float _currentTime;
        private int _currentLoadingTextIndex;
        private string[] _loadingTexts = new[] { "loading", "loading.", "loading..", "loading..."};

        private void Update()
        {
            if(_currentTime >= 1)
            {
                if(_currentLoadingTextIndex >= _loadingTexts.Length - 1)
                {
                    _currentLoadingTextIndex = 0;
                }
                else
                {
                    _currentLoadingTextIndex++;
                }

                _loadingText.text = _loadingTexts[_currentLoadingTextIndex];
                _currentTime = 0;
            }
            else
            {
                _currentTime += Time.unscaledDeltaTime;
            }
        }

        public async UniTask StartLoading()
        {
            _isPlayingAnimation = true;
            _animator.Play("start", 0, 0);
            await UniTask.WaitUntil(() => !_isPlayingAnimation);
        }

        public async UniTask EndLoading()
        {
            _isPlayingAnimation = true;
            _animator.Play("stop",0 ,0 );
            await UniTask.WaitUntil(() => !_isPlayingAnimation);
        }

        #region Unity Event

        public void EndAnimation()
        {
            _isPlayingAnimation = false;
        }

        #endregion Unity Event
    }
}