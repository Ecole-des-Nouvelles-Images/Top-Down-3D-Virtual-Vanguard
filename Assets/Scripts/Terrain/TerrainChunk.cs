using System.Collections;
using Managers;
using UnityEngine;

using Terrain.Procedural;
using Unity.AI.Navigation;

namespace Terrain
{
    public class TerrainChunk: MonoBehaviour
    {
        public XenoManager XenoManager;
        
        public MapGenerator Terrain { get; set; }
        public NavMeshSurface NavMesh { get; set; }

        private void Awake()
        {
            Terrain = GetComponentInChildren<MapGenerator>();
            NavMesh = GetComponentInChildren<NavMeshSurface>();
        }

        public void SetupChunk(bool reverseChunkScale)
        {
            if (reverseChunkScale) {
                Terrain.transform.localScale = new Vector3(-1, 1, 1);
            }
            
            Terrain.GenerateMap();
        }
    }
}
