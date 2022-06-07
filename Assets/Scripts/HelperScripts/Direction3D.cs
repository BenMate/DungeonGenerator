using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerator
{
    public static class Direction3D
    {
        static List<Vector3> directions = new List<Vector3>()
        {
            Vector3.forward,
            Vector3.back,
            Vector3.right,
            Vector3.left
        };

        public static Vector3 GetRandomDirectionXZ()
        {
            //returns a random value from the list of directions
            return directions[Random.Range(0, directions.Count)];
        }
    }
}
