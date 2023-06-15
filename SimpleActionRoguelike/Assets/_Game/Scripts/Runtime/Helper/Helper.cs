using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Runtime.Helper
{
    public static class Helper
    {
        public static T GetSuitableValue<T>(this List<T> values, T defaultValue, int index = 0)
        {
            if(values != null && values.Count > index)
            {
                return values[index];
            }
            return defaultValue;
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
                return gameObject.AddComponent<T>();
            return component;
        }

        public static Transform FindChildTransform(this Transform transform, string name)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform childTransform = transform.GetChild(i);
                if (string.Equals(childTransform.name, name))
                    return childTransform;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform childTransform = transform.GetChild(i).FindChildTransform(name);
                if (childTransform != null)
                    return childTransform;
            }

            return null;
        }

        public static GameObject FindChildGameObject(this Transform transform, string name)
        {
            Transform childTransform = transform.FindChildTransform(name);
            if (childTransform != null)
                return childTransform.gameObject;
            else
                return null;
        }

        public static string ToSnakeCase(this string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return inputString;

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(char.ToLower(inputString[0]));

            for (int i = 1; i < inputString.Length; i++)
            {
                char character = inputString[i];
                if (char.IsUpper(character) || (char.IsDigit(character) && i > 1 && !char.IsDigit(inputString[i - 1])))
                    stringBuilder.Append('_');
                stringBuilder.Append(char.ToLower(character));
            }

            return stringBuilder.ToString();
        }

        //arrayToCurve is original Vector3 array, smoothness is the number of interpolations. 
        public static List<Vector3> MakeSmoothCurve(List<Vector3> arrayToCurve, float smoothness)
        {
            List<Vector3> points;
            List<Vector3> curvedPoints;
            int pointsLength = 0;
            int curvedLength = 0;

            if (smoothness < 1.0f) smoothness = 1.0f;

            pointsLength = arrayToCurve.Count;

            curvedLength = (pointsLength * Mathf.RoundToInt(smoothness)) - 1;
            curvedPoints = new List<Vector3>(curvedLength);

            float t = 0.0f;
            for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
            {
                t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

                points = new List<Vector3>(arrayToCurve);

                for (int j = pointsLength - 1; j > 0; j--)
                {
                    for (int i = 0; i < j; i++)
                    {
                        points[i] = (1 - t) * points[i] + t * points[i + 1];
                    }
                }

                curvedPoints.Add(points[0]);
            }

            return curvedPoints;
        }

        public static Vector2 Bezier(Vector2 a, Vector2 b, float t)
        {
            return Vector2.Lerp(a, b, t);
        }

        public static Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float t)
        {
            return Vector2.Lerp(Bezier(a, b, t), Bezier(b, c, t), t);
        }

        public static Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
        {
            return Vector2.Lerp(Bezier(a, b, c, t), Bezier(b, c, d, t), t);
        }
    }
}