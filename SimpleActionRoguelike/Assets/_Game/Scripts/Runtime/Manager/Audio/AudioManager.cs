using Cysharp.Threading.Tasks;
using DG.Tweening;
using Runtime.Constants;
using Runtime.Core.Pool;
using Runtime.Core.Singleton;
using UnityEngine;

namespace Runtime.Manager.Audio
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        private readonly float MUSIC_FADE_DURATION = 0.5f;

        [SerializeField] AudioSource _musicAudioSource;
        [SerializeField] AudioSource _sfxLoopAudioSource;

        private bool _isMuteMusic;
        private bool _isMuteSfx;

        private string _musicAudioKey;

        protected override void Awake()
        {
            base.Awake();
            LoadSettings();
        }

        private void LoadSettings()
        {
        }

        public void PlayMusic(string music)
        {
            if (this._isMuteMusic)
            {
                return;
            }

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

            this._musicAudioSource.clip = audioClip;
            this._musicAudioSource.Play();
            this._musicAudioSource.DOFade(1, this.MUSIC_FADE_DURATION);
        }

        public async UniTask PlaySfx(string sfx)
        {
            if (this._isMuteSfx)
            {
                return;
            }

            GameObject sfxItem = await PoolManager.Instance.Rent(Constant.SFX_ITEM);
            sfxItem.transform.SetParent(this.transform, false);
            sfxItem.SetActive(true);
            SoundItem item = sfxItem.GetComponent<SoundItem>();;
            await item.PlaySfx(sfx);
        }

        public async void PlayLoopSfx(string sfx)
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