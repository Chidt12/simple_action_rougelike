using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Message;
using Runtime.Message;
using System;
using System.Threading;

namespace Runtime.Manager.Gameplay
{
    public class WaveTimer
    {
        #region Members

        private WaveStageLoadConfigItem _data;
        private CancellationTokenSource _cancellationTokenSource;
        private Action _onFinish;
        private int _currentGameplayTime;

        #endregion Members

        #region Properties

        public int CurrentGameplayTime => _currentGameplayTime;

        #endregion Properties

        #region Class Methods

        public void SetUp()
            => _currentGameplayTime = 0;

        public void Start(WaveStageLoadConfigItem data, Action onFinish)
        {
            _data = data;
            _onFinish = onFinish;
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            CountTimeAsync(data.IsInfiniteDuration, _data.duration).Forget();
        }

        public void Dispose()
            => _cancellationTokenSource?.Cancel();

        private async UniTask CountTimeAsync(bool isInfiniteDuration, int duration)
        {
            if (isInfiniteDuration)
            {
                int count = 0;
                while (true)
                {
                    count++;
                    _currentGameplayTime++;
                    SimpleMessenger.Publish(new WaveTimeUpdatedMessage(false, _currentGameplayTime));
                    await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: false, cancellationToken: _cancellationTokenSource.Token);
                }
            }
            else
            {
                for (int i = 0; i < duration; i++)
                {
                    _currentGameplayTime++;
                    SimpleMessenger.Publish(new WaveTimeUpdatedMessage(false, _currentGameplayTime));
                    await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: false, cancellationToken: _cancellationTokenSource.Token);
                }
            }

            _onFinish?.Invoke();
        }

        #endregion Class Methods
    }
}