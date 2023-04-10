using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.MapGen
{
    public class ProceduralGenerationAlgorithms : MonoBehaviour
    {
        #region Class Methods

        public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
        {
            HashSet<Vector2Int> path = new HashSet<Vector2Int>();
            path.Add(startPosition);
            var previousPosition = startPosition;

            for (int i = 0; i < walkLength; i++)
            {
                var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
                path.Add(newPosition);
                previousPosition = newPosition;
            }
            return path;
        }

        public static class Direction2D
        {
            public static List<Vector2Int> cardinalDirectionList = new List<Vector2Int>()
            {
                new Vector2Int(0, 1), // UP
                new Vector2Int(1, 0), // RIGHT
                new Vector2Int(0, -1), // DOWN
                new Vector2Int(-1, 0), // LEFT
            };

            public static Vector2Int GetRandomCardinalDirection()
            {
                return cardinalDirectionList[Random.Range(0, cardinalDirectionList.Count)];
            }
        }

        #endregion Class Methods
    }

}