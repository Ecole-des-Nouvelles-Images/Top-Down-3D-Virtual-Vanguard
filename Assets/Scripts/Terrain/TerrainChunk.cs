using UnityEngine;

using Terrain.Procedural;

namespace Terrain
{
    public class TerrainChunk: MonoBehaviour
    {
        public MapGenerator Terrain { get; private set; }

        private void Awake() {
            Terrain = GetComponentInChildren<MapGenerator>();
        }

        public void SetupChunk(float noiseOffset) {
            Terrain.Offset.x = noiseOffset;
            Terrain.GenerateMap();
        }
    }
}
