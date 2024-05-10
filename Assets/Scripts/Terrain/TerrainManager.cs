using System;
using Terrain.Procedural;
using UnityEngine;

namespace Terrain
{
    public class TerrainManager : MonoBehaviour
    {
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
    }
}
