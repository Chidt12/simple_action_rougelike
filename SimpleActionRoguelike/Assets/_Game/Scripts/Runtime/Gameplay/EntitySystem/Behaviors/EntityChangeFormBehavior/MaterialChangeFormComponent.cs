using System;
using System.Linq;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class MaterialChangeFormComponent : ChangeFormComponent
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private MaterialData[] _materials;

        [Serializable]
        public class MaterialData
        {
            public int formId;
            public Material material;
        }

        public override void ChangeForm(int value)
        {
            var material = _materials.FirstOrDefault(x => x.formId == value);
            _spriteRenderer.material = material.material;
        }
    }
}