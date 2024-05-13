using System;
using System.Collections;
using Terrain.Procedural;
using UnityEngine;

namespace Terrain
{
    public class TerrainManager : MonoBehaviour
    {
        [Header("Gameplay Settings")]
        public float SlowDownTransitionTime;
        
        [Header("Experimental")]
        public MapGenerator Generator;
        public bool EnableScrolling;
        public float ScrollSpeed;

        private void Update()
        {
            if (EnableScrolling)
            {
                Generator.Offset.x += ScrollSpeed * Time.deltaTime;
                Generator.GenerateMap();
            }
        }

        public void StopScrolling()
        {
            StartCoroutine(SlowDownScrollingCoroutine());
        }

        private IEnumerator SlowDownScrollingCoroutine()
        {
            float t = 0f;

            while (t > 0)
            {
                t += Time.deltaTime * SlowDownTransitionTime;
                Generator.Offset.x = Mathf.Lerp(0, ScrollSpeed, t);
                yield return null;
            }
        }
    }
}
