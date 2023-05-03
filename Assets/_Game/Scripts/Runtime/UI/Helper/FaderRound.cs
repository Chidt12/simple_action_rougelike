using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Helper;
using ZBase.Foundation.PubSub;
using Runtime.Core.Message;
using Runtime.Message;

namespace Runtime.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FaderRound : MonoBehaviour
    {
        public enum CameraModes { Main, Override }

        [Header("Bindings")]
        [SerializeField]
        private CameraModes _cameraMode = CameraModes.Main;

        [ShowIf(nameof(_cameraMode), CameraModes.Override)]
        [SerializeField]
        /// the camera to pick the position from (usually the "regular" game camera)
        private Camera _targetCamera;
        /// the background to fade 
        public RectTransform FaderBackground;
        /// the mask used to draw a hole in the background that will get faded / scaled
        public RectTransform FaderMask;

        [SerializeField] private float _minScale;
        [SerializeField] private float _maxScale;

        private bool _ignoreTimeScale = true;
        [Header("Interaction")]
        /// whether or not the fader should block raycasts when visible
        [SerializeField]
        private bool _shouldBlockRaycasts = false;

        protected CanvasGroup _canvasGroup;

        protected float _initialScale;
        protected float _currentTargetScale;

        protected float _currentDuration;
        protected TweenType _currentCurve;

        protected bool _fading = false;
        protected float _fadeStartedAt;
        protected bool _disableWhenFinished;
        protected List<ISubscription> subscriptions;

        /// <summary>
        /// On Start, we initialize our fader
        /// </summary>
        protected virtual void Awake()
        {
            subscriptions = new();
            Initialization();
        }

        private void OnEnable()
        {
            subscriptions.Add(SimpleMessenger.Subscribe<FadeInMessage>(OnFadeIn));
            subscriptions.Add(SimpleMessenger.Subscribe<FadeOutMessage>(OnFadeOut));
            subscriptions.Add(SimpleMessenger.Subscribe<FadeStopMessage>(OnFadeStop));
            subscriptions.Add(SimpleMessenger.Subscribe<UpdateCameraMessage>(OnUpdateCamera));
        }

        private void OnDisable()
        {
            foreach (var subscription in subscriptions)
                subscription.Dispose();
        }

        private void OnFadeIn(FadeInMessage message)
        {
            _disableWhenFinished = message.stopAfterFinished;
            StartFading(_minScale, _maxScale, message.duration, message.curve,
                message.ignoreTimeScale, message.worldPosition);
        }

        private void OnFadeOut(FadeOutMessage message)
        {
            _disableWhenFinished = message.stopAfterFinished;
            StartFading(_maxScale, _minScale, message.duration, message.curve,
                message.ignoreTimeScale, message.worldPosition);
        }

        private void OnFadeStop(FadeStopMessage message)
        {
            _fading = false;
        }

        private void OnUpdateCamera(UpdateCameraMessage message)
        {
            if(_cameraMode == CameraModes.Main)
            {
                _targetCamera = message.Camera;
            }
        }

        /// <summary>
        /// On init, we grab our components, and disable/hide everything
        /// </summary>
        protected virtual void Initialization()
        {
            if (_cameraMode == CameraModes.Main)
            {
                _targetCamera = Camera.main;
            }
            _canvasGroup = GetComponent<CanvasGroup>();
            FaderMask.transform.localScale = _maxScale * Vector3.one;
        }

        protected virtual void Update()
        {
            if (_canvasGroup == null) { return; }

            if (_fading)
            {
                Fade();
            }
        }

        protected virtual void Fade()
        {
            float currentTime = _ignoreTimeScale ? Time.unscaledTime : Time.time;
            float endTime = _fadeStartedAt + _currentDuration;
            if (currentTime - _fadeStartedAt < _currentDuration)
            {
                float newScale = TweenHelper.Tween(currentTime, _fadeStartedAt, endTime, _initialScale, _currentTargetScale, _currentCurve);
                FaderMask.transform.localScale = newScale * Vector3.one;
            }
            else
            {
                StopFading();
            }
        }

        protected virtual void StopFading()
        {
            FaderMask.transform.localScale = _currentTargetScale * Vector3.one;
            _fading = false;
            if(_disableWhenFinished)
                DisableFader();
        }

        protected virtual void DisableFader()
        {
            if (_shouldBlockRaycasts)
            {
                _canvasGroup.blocksRaycasts = false;
            }
            _canvasGroup.alpha = 0;
        }

        protected virtual void EnableFader()
        {
            if (_shouldBlockRaycasts)
            {
                _canvasGroup.blocksRaycasts = true;
            }
            _canvasGroup.alpha = 1;
        }

        protected virtual void StartFading(float initialAlpha, float endAlpha, float duration, TweenType curve,
            bool ignoreTimeScale, Vector3 worldPosition)
        {
            if (_targetCamera == null)
            {
                Debug.LogWarning(this.name + " : You're using a fader round but its TargetCamera hasn't been setup in its inspector. It can't fade.");
                return;
            }

            Vector3 screenPoint = _targetCamera.WorldToScreenPoint(worldPosition);
            FaderMask.transform.position = screenPoint;

            _ignoreTimeScale = ignoreTimeScale;
            EnableFader();
            _fading = true;
            _initialScale = initialAlpha;
            _currentTargetScale = endAlpha;
            _fadeStartedAt = _ignoreTimeScale ? Time.unscaledTime : Time.time;
            _currentCurve = curve;
            _currentDuration = duration;

            float newScale = TweenHelper.Tween(0f, 0f, duration, _initialScale, _currentTargetScale, _currentCurve);
            FaderMask.transform.localScale = newScale * Vector3.one;
        }
    }

}