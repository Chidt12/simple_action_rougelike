using System;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace ZBase.UnityScreenNavigator.Foundation.AssetLoaders
{
    public readonly struct AssetLoadHandleId : IEquatable<AssetLoadHandleId>
    {
        private readonly uint _value;

        public AssetLoadHandleId(uint value)
        {
            _value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(AssetLoadHandleId other)
        {
            return _value == other._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is AssetLoadHandleId other)
            {
                return _value == other._value;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return _value.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator AssetLoadHandleId(uint value)
        {
            return new AssetLoadHandleId(value);
        }
    }

    public class AssetLoadHandle
    {
        private Func<float> _percentCompleteFunc;

        public Object TypelessResult { get; private set; }

        public UniTask TypelessTask { get; private set; }

        public AssetLoadHandle(AssetLoadHandleId id)
        {
            Id = id;
        }

        public AssetLoadHandleId Id { get; }

        public bool IsDone => Status != AssetLoadStatus.None;

        public AssetLoadStatus Status { get; private set; }

        public float PercentComplete => _percentCompleteFunc.Invoke();

        public Exception OperationException { get; private set; }

        public void SetStatus(AssetLoadStatus status)
        {
            Status = status;
        }

        public void SetPercentCompleteFunc(Func<float> percentComplete)
        {
            _percentCompleteFunc = percentComplete;
        }

        public void SetOperationException(Exception ex)
        {
            OperationException = ex;
        }

        public void SetTypelessResult(Object result)
        {
            TypelessResult = result;
        }

        public void SetTypelessTask(UniTask task)
        {
            TypelessTask = task;
        }
    }

    public class AssetLoadHandle<T> : AssetLoadHandle
        where T : Object
    {
        public AssetLoadHandle(AssetLoadHandleId id) : base(id) { }

        public T Result { get; private set; }

        public UniTask<T> Task { get; private set; }

        public void SetResult(T result)
        {
            Result = result;
            SetTypelessResult(result);
        }

        public void SetTask(UniTask<T> task)
        {
            Task = task;
            SetTypelessTask(task.AsUniTask());
        }
    }
}