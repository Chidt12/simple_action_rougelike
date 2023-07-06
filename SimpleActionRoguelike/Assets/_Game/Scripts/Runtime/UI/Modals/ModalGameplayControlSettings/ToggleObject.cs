using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.UI
{
    public class ToggleObject : MonoBehaviour
    {
        [SerializeField] private GameObject _activeGameObject;
        [SerializeField] private GameObject _inActiveGameObject;

        public void Toggle(bool value)
        {
            _activeGameObject.SetActive(value);
            _inActiveGameObject.SetActive(!value);
        }
    }
}