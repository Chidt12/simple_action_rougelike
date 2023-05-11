using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Extensions
{
    public static class GameExtensions
    {
        #region Class Methods

        public static Quaternion ToQuaternion(this Vector2 direction, float offsetDegree = 90.0f)
            => Quaternion.Euler(Vector3.forward * (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - offsetDegree));

        public static string ToColorString(this string textInfo, Color color)
        {
            string colorString = $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{textInfo}</color>";
            return colorString;
        }

        public static string ToDisplayString(this long resourceNumber)
        {
            if (resourceNumber >= 1000000000)
            {
                return $"{(double)resourceNumber / 1000000000:F2}B";
            }
            else if (resourceNumber >= 1000000)
            {
                return $"{(double)resourceNumber / 1000000:F2}M";
            }
            else
            {
                int numDigits = (int)Math.Floor(Math.Log10(resourceNumber)) + 1;
                if (numDigits <= 3)
                    return $"{resourceNumber:F0}";
                else
                    return $"{resourceNumber / 1000.0:F3}";
            }
        }

        #endregion Class Methods
    }
}
