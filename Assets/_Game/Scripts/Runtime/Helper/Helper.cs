using System.Collections;
using System.Collections.Generic;
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

        public static T GetSuitableValue<T>(this T[] values, T defaultValue, int index = 0)
        {
            if (values != null && values.Length > index)
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
    }
}