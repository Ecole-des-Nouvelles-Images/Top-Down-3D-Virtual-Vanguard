using UnityEngine;

using Terrain.Procedural;

namespace Terrain
{
    public class TerrainChunk: MonoBehaviour
    {
        public TerrainManager Manager;
        public MapGenerator Terrain { get; private set; }

        private void Awake()
        {
            Terrain = GetComponentInChildren<MapGenerator>();
            Manager = FindObjectOfType<TerrainManager>();
        }

        public void SetupChunk(float noiseOffset)
        {
            Terrain.Offset.x = noiseOffset;
            Terrain.GenerateMap();
        }
    }
}
