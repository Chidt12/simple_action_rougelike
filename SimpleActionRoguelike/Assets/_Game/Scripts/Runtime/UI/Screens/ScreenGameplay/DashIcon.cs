using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class DashIcon : MonoBehaviour
    {
        [SerializeField] private GameObject _fullGameObject;
        [SerializeField] private DOTweenAnimation _dotweenAnimation;
        
        public void ToggleActive(bool value, bool playAnimation)
        {
            if (value && playAnimation)
                _dotweenAnimation.DORestart();
            _fullGameObject.SetActive(value);
        }
    }
}
