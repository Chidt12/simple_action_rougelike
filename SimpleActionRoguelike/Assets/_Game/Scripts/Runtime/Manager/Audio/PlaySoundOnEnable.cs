using Cysharp.Threading.Tasks;
using Runtime.Manager.Audio;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class PlaySoundOnEnable : MonoBehaviour
    {
        [SerializeField] private string _sfx;
        [SerializeField] private float _factor = 1;

        private void OnEnable()
        {
            if(!string.IsNullOrEmpty(_sfx))
                AudioManager.Instance.PlaySfx(_sfx, _factor).Forget();
        }
    }
}
