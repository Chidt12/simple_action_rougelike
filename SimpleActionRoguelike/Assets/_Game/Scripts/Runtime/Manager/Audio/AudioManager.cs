using Cysharp.Threading.Tasks;
using DG.Tweening;
using Runtime.Constants;
using Runtime.Core.Pool;
using Runtime.Core.Singleton;
using Runtime.Manager.Data;
using UnityEngine;

namespace Runtime.Manager.Audio
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        private readonly float MUSIC_FADE_DURATION = 0.5f;

        [SerializeField] AudioSource _musicAudioSource;
        [SerializeField] AudioSource _sfxLoopAudioSource;

        [Range(0, 1)]
        [SerializeField] float _audioMaxValue;
        [Range(0,1)]
        [SerializeField] float _musicMaxValue;

        private bool _isMuteMusic;
        private bool _isMuteSfx;

        private string _musicAudioKey;
        private float _musicFactor;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            UpdateSettings();
        }

        public void UpdateSettings()
        {
            var musicValue = DataManager.Local.playerBasicLocalData.musicSettings;
            var soundValue = DataManager.Local.playerBasicLocalData.sfxSettings;

            _musicAudioSource.volume = (float)musicValue / Constant.MAX_CONFIG_SOUND * _musicMaxValue;
            _sfxLoopAudioSource.volume = (float)soundValue / Constant.MAX_CONFIG_SOUND * _audioMaxValue;

            _isMuteMusic = musicValue == 0;
            _isMuteSfx = soundValue == 0;

            _musicAudioSource.mute = _isMuteMusic;
            _sfxLoopAudioSource.mute = _isMuteSfx;
        }

        public void PlayMusic(string music, float factor = 1)
        {
            this._musicFactor = factor;
            this._musicAudioKey = music;
            this._musicAudioSource.DOFade(0, this.MUSIC_FADE_DURATION).OnComplete(LoadAndPlayMusic);
        }

        private async void LoadAndPlayMusic()
        {
            this._musicAudioSource.Stop();
            AudioClip audioClip = await AssetLoader.LoadAudioClip(_musicAudioKey);
            if (!audioClip)
            {
                return;
            }

            var musicVolume = DataManager.Local.playerBasicLocalData.musicSettings;
            var volume = (float)musicVolume / Constant.MAX_CONFIG_SOUND * _musicMaxValue * _musicFactor;

            this._musicAudioSource.clip = audioClip;
            this._musicAudioSource.Play();
            this._musicAudioSource.DOFade(volume, this.MUSIC_FADE_DURATION);
        }

        public async UniTask PlaySfx(string sfx, float factor = 1)
        {
            if (this._isMuteSfx)
            {
                return;
            }

            GameObject sfxItem = await PoolManager.Instance.Rent(Constant.SFX_ITEM);
            sfxItem.transform.SetParent(this.transform, false);
            sfxItem.SetActive(true);
            SoundItem item = sfxItem.GetComponent<SoundItem>();;

            var sfxVolumn = DataManager.Local.playerBasicLocalData.sfxSettings;
            var volume = (float)sfxVolumn / Constant.MAX_CONFIG_SOUND * _audioMaxValue * factor;
            await item.PlaySfx(sfx, volume);
        }

        public async void PlayLoopSfx(string sfx, float factor)
        {
            if (this._isMuteSfx)
            {
                return;
            }

            AudioClip audioClip = await AssetLoader.LoadAudioClip(sfx);
            if (!audioClip)
            {
                return;
            }

            this._sfxLoopAudioSource.clip = audioClip;
            this._sfxLoopAudioSource.Play();
        }

        public void StopLoopSfx()
        {
            this._sfxLoopAudioSource.Stop();
        }
    }
}