using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZBase.UnityScreenNavigator.Foundation.Animation
{
    internal delegate float CalcDeltaTime(float deltaTime);

    public enum DeltaTimeType
    {
        Unscaled,
        Timescaled,
    }

    internal class UpdateDispatcher : MonoBehaviour
    {
        private static UpdateDispatcher _instance;
        
        public static UpdateDispatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    var gameObject = new GameObject($"{nameof(UnityScreenNavigator)}.{nameof(UpdateDispatcher)}");
                    var component = gameObject.AddComponent<UpdateDispatcher>();
                    DontDestroyOnLoad(gameObject);
                    _instance = component;
                }

                return _instance;
            }
        }

        public DeltaTimeType DeltaTimeType { get; private set; }

        private readonly List<Updatable> _targets = new();
        private Action _updater;

        private void Awake()
        {
            SetDeltaTime(DeltaTimeType.Unscaled);
        }

        private void Update()
        {
            _updater();
        }

        private void UnscaledUpdate()
        {
            var targets = _targets;
            var count = targets.Count;

            for (var i = 0; i < count; i++)
            {
                targets[i].Update(Time.unscaledDeltaTime);
            }
        }

        private void ScaledUpdate()
        {
            var targets = _targets;
            var count = targets.Count;

            for (var i = 0; i < count; i++)
            {
                targets[i].Update(Time.deltaTime);
            }
        }

        public void SetDeltaTime(DeltaTimeType type)
        {
            DeltaTimeType = type;

            if (type == DeltaTimeType.Timescaled)
                _updater = ScaledUpdate;
            else
                _updater = UnscaledUpdate;
        }

        public void Register(IUpdatable target, CalcDeltaTime calcDeltaTime = null)
        {
            _targets.Add(new Updatable(target, calcDeltaTime));
        }

        public void Unregister(IUpdatable target)
        {
            _targets.Remove(new Updatable(target));
        }

        private readonly struct Updatable : IEquatable<Updatable>
        {
            private readonly IUpdatable _target;
            private readonly CalcDeltaTime _calcDeltaTime;

            public Updatable(IUpdatable target, CalcDeltaTime calcDeltaTime = null)
            {
                _target = target ?? throw new ArgumentNullException(nameof(target));
                _calcDeltaTime = calcDeltaTime;
            }

            public void Update(float deltaTime)
            {
                var time = _calcDeltaTime?.Invoke(deltaTime) ?? deltaTime;
                _target?.Update(time);
            }

            public override int GetHashCode()
            {
                if (_target == null)
                    return 0;

                return _target.GetHashCode();
            }

            public bool Equals(Updatable other)
            {
                return ReferenceEquals(_target, other._target);
            }

            public override bool Equals(object obj)
            {
                if (obj is Updatable other)
                {
                    return ReferenceEquals(_target, other._target);
                }

                return false;
            }
        }
    }

    public static class AnimationUpdateDeltaTime
    {
        public static void Set(DeltaTimeType type)
        {
            UpdateDispatcher.Instance.SetDeltaTime(type);
        }

        public static DeltaTimeType Get()
        {
            return UpdateDispatcher.Instance.DeltaTimeType;
        }
    }
}