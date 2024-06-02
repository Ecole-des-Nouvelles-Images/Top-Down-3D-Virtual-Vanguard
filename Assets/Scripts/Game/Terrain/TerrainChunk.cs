using Game.Foes;
using Game.Terrain.Procedural;
using Unity.AI.Navigation;
using UnityEngine;

namespace Game.Terrain
{
    public class TerrainChunk: MonoBehaviour
    {
        public Side PlayableSide;
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
