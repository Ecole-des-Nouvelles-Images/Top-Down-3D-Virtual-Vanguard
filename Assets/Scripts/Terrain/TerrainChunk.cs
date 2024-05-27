using System;
using UnityEngine;

using Terrain.Procedural;

namespace Terrain
{
    public class TerrainChunk: MonoBehaviour
    {
        public TransitManager Manager;
        public MapGenerator Terrain { get; private set; }

        private void Awake()
        {
            Terrain = GetComponent<MapGenerator>();
            Manager = FindObjectOfType<TransitManager>();
        }

        public void SetupChunk(float noiseOffset)
        {
            Terrain.Offset.x = noiseOffset;
            Terrain.GenerateMap();
        }

        private void OnTriggerEnter(Collider other)
        {
            throw new NotImplementedException();
        }
    }
}
