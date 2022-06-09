using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DungeonGenerator
{
    public class BoundsGenerater : MonoBehaviour
    {
        [Header("Bounds Config")]
        [Tooltip("The Bounds offset, be careful as it will change the position of the bounds")]
        public Vector3 boundsOffset = Vector3.zero;
        [Tooltip("Segments only need to know the Bounds 'Z' Size, Corridors will scale towards the Rooms Bounds so make sure its accurate")]
        public Vector3 boundsSize = Vector3.one;

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
