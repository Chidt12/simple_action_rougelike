using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class RushAttackIndicator : MonoBehaviour
    {
        #region Members

        [SerializeField]
        private Transform _leftIndicator;
        [SerializeField]
        private Transform _rightIndicator;

        #endregion Members

        #region Class Methods

        public void Init(float width, float length)
        {
            transform.localScale = new Vector2(1, length);
            _leftIndicator.localPosition = new Vector2(-width / 2, 0);
            _rightIndicator.localPosition = new Vector2(width / 2, 0);
        }

        #endregion Class Methods
    }
}