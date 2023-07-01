using Cysharp.Threading.Tasks;
using Runtime.Core.Pool;
using UnityEngine;

namespace Runtime.Manager.Audio
{
    public class SoundItem : MonoBehaviour
    {
        [SerializeField] private AudioSource _sfxAudio;

        private void OnEnable()
        {
            this._sfxAudio.Stop();
        }

        public async UniTask PlaySfx(string sfx)
        {
            AudioClip audioClip = await AssetLoader.LoadAudioClip(sfx);

            if (!audioClip)
            {
                return;
            }

            this._sfxAudio.clip = audioClip;
            this._sfxAudio.Play();
            Invoke(nameof(ReturnItem), audioClip.length + 0.1f);
        }

        private void ReturnItem()
        {
            PoolManager.Instance.Return(gameObject);
        }
    }
}