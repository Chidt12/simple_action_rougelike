using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Runtime.Gameplay.UI
{
    public class RunningText : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _text;
        [SerializeField] private string _content;
        [SerializeField] private float _speed;

        private int _currentIndex = 0;
        private float _currentTime = 0;

        private void Awake()
        {
            _currentIndex = 0;
            _currentTime = 0;
        }

        private void Update()
        {
            _text.text = _content.Substring(0, _currentIndex);
            if(_currentTime > _speed)
            {
                _currentTime = 0;
                if(_currentIndex >= _content.Length)
                    _currentIndex = 0;
                else
                    _currentIndex++;
            }
            else
            {
                _currentTime += Time.deltaTime;
            }
        }

    }
}