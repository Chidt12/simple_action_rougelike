using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Runtime.UI
{
    public class FaderRound : MonoBehaviour
    {
        public enum CameraModes { Main, Override }

        [Header("Bindings")]
        public CameraModes CameraMode = CameraModes.Main;

        [ShowIf(nameof(CameraMode), CameraModes.Override)]
        [SerializeField]
        /// the camera to pick the position from (usually the "regular" game camera)
        public Camera TargetCamera;
        /// the background to fade 
        public RectTransform FaderBackground;
        /// the mask used to draw a hole in the background that will get faded / scaled
        public RectTransform FaderMask;

        [SerializeField] private float _minScale;
        [SerializeField] private float _maxScale;
        [SerializeField] private float _timing = 0.2f;
        //[SerializeField] DOTweenCurve
    }

}