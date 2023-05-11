using Runtime.Core.Pool;
using UnityEngine;

namespace Runtime.Common
{
    public class AutoReturnPoolAnimator : MonoBehaviour
    {
        #region Unity Event for remove

        public void ReturnPool() => PoolManager.Instance.Return(gameObject);

        #endregion
    }
}
