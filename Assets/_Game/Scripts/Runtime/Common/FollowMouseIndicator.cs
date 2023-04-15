using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class FollowMouseIndicator : MonoBehaviour
    {
        private void Update()
        {
            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
