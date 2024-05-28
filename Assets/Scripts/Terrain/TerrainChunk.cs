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
            Terrain = GetComponentInChildren<MapGenerator>();
            Manager = FindObjectOfType<TransitManager>();
        }

        public void SetupChunk(float noiseOffset)
        {
            Terrain.Offset.x = noiseOffset;
            Terrain.GenerateMap();
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Convoy")) return;
            
            Manager.UpdateRoad();
        }
    }
}
