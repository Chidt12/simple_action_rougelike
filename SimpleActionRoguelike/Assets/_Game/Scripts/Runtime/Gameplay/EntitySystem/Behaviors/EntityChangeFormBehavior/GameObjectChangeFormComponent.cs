using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class GameObjectChangeFormComponent : ChangeFormComponent
    {
        [SerializeField] private GameObjectData[] _gameObjectsData;

        [Serializable]
        public class GameObjectData
        {
            public int formId;
            public GameObject go;
        }

        public override void ChangeForm(int value)
        {
            foreach (var item in _gameObjectsData)
            {
                if(item.go)
                    item.go.SetActive(value == item.formId);
            }
        }
    }
}