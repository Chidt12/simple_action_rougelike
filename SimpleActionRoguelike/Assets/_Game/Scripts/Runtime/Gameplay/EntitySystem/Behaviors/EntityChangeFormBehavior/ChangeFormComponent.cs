using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class ChangeFormComponent : MonoBehaviour
    {
        public abstract void ChangeForm(int value);
    }
}