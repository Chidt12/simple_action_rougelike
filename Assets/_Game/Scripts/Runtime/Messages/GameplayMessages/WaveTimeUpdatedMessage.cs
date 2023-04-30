using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public readonly struct WaveTimeUpdatedMessage : IMessage
    {
        #region Members

        public readonly bool IsNewWave;
        public readonly float CurrentGameplayTime;
        public readonly int CurrentWaveIndex;
        public readonly int MaxWaveIndex;

        #endregion Members

        #region Struct Methods

        public WaveTimeUpdatedMessage(bool isNewWave, float currentGameplayTime, int currentWaveIndex = default, int maxWaveIndex = default)
        {
            IsNewWave = isNewWave;
            CurrentGameplayTime = currentGameplayTime;
            CurrentWaveIndex = currentWaveIndex;
            MaxWaveIndex = maxWaveIndex;
        }

        #endregion Struct Methods
    }
}
