using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerator
{
    public class DungeonArea : MonoBehaviour
    {
        [Header("Bounds Config")]
        [Tooltip("The Bounds offset")]
        public Vector3 boundsOffset = Vector3.zero;
        [Tooltip("The Bounds size, the Corridors Will Scale to the Bounds")]
        public Vector3 boundsSize = Vector3.one;

        [Header("Wall Config")]
        [Tooltip("Wall Prefab")]
        //note needs to have an empty parent
        public GameObject wallPrefab;
        [Tooltip("Adjusts the Position of the wall Spawn")]
        public Vector3 wallPosOffset;

        [Tooltip("Adjusts the Rotation of the Wall Spawn")]
        public Vector3 wallRotationOffset;

        protected bool left, right, back, forward;

        public void SetCorridorDirection(Vector3 direction)
        {
            if (direction == Vector3.left)
                left = true;
            else if (direction == Vector3.right)
                right = true;
            else if (direction == Vector3.back)
                back = true;
            else if (direction == Vector3.forward)
                forward = true;
        }

        public void GenerateWalls(Transform parent = null)
        {
            if (!forward)
                Instantiate(wallPrefab, transform.position + Vector3.forward * boundsSize.z / 2 + wallPosOffset,
                    Quaternion.Euler(wallRotationOffset.x, 180 + wallRotationOffset.y, wallRotationOffset.z), parent.transform);

            if (!back)
                Instantiate(wallPrefab, transform.position + Vector3.back * boundsSize.z / 2 + wallPosOffset,
                    Quaternion.Euler(wallRotationOffset.x, wallRotationOffset.y, wallRotationOffset.z), parent.transform);

            if (!left)
                Instantiate(wallPrefab, transform.position + Vector3.left * boundsSize.x / 2 + wallPosOffset,
                     Quaternion.Euler(wallRotationOffset.x, 90 + wallRotationOffset.y, wallRotationOffset.z), parent.transform);

            if (!right)
                Instantiate(wallPrefab, transform.position + Vector3.right * boundsSize.x / 2 + wallPosOffset,
                   Quaternion.Euler(wallRotationOffset.x, 270 + wallRotationOffset.y, wallRotationOffset.z), parent.transform);
        }

        public void CalculateBounds()
        {
            Bounds bounds = Encap(transform, new Bounds());
            boundsOffset = bounds.center;
            boundsSize = bounds.size;
        }

        Bounds Encap(Transform parent, Bounds blocker)
        {
            if (parent.childCount == 0)
            {
                Renderer rend = parent.GetComponent<Renderer>();

                if (rend != null)
                    blocker.Encapsulate(rend.bounds);

                return blocker;
            }

            foreach (Transform child in parent)
            {
                Renderer renderer = child.GetComponent<Renderer>();

                if (renderer != null)
                    blocker.Encapsulate(renderer.bounds);

                blocker = Encap(child, blocker);
            }
            return blocker;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            //draw the bounds
            Gizmos.DrawWireCube(transform.position + boundsOffset, boundsSize);
        }
    }
}