using UnityEngine;

namespace Runtime.Core.Pool
{
    public abstract class ReturnPool : MonoBehaviour
    {
        protected abstract float ReturnDelayTime { get; }

        protected virtual void OnEnable() => Invoke("InvokeReturnPool", ReturnDelayTime);

        protected virtual void InvokeReturnPool()
        {
            gameObject.transform.SetParent(null);
            PoolManager.Instance.Return(gameObject);
        }
    }
}