using System;
using UnityEngine;

namespace LandmassGeneration
{
    public enum GenerationMode
    {
        NoiseMap,
        Mesh
    }
    
    [RequireComponent(typeof(MapDisplay))]
    public class MapGenerator : MonoBehaviour
    {
        [Header("Components")]
        public MapDisplay MapDisplay;
        
        [Header("Map Scale")]
        public int MapWidth;
        public int MapHeight;
        public float NoiseScale;
        public float HeightScale;

        [Header("Noise Precision")]
        public int Octaves;
        [Range(0, 1)] public float Persistence;
        public float Lacunarity;

        [Header("Randomness")]
        public int Seed;
        public Vector2 Offset;

        [Header("Generation")]
        public GenerationMode DrawMode;
        public bool AutoUpdate;

        private void OnValidate()
        {
            if (MapWidth < 1) MapWidth = 1;
            if (MapHeight < 1) MapHeight = 1;
            if (Lacunarity < 1) Lacunarity = 1;
            if (NoiseScale <= 0) NoiseScale = 0.001f;
            if (Octaves <= 0) Octaves = 1;
        }

        public void GenerateMap()
        {
            float[,] noiseMap = Noise.GenerateNoiseMap(MapWidth, MapHeight, Seed, NoiseScale, Octaves, Persistence, Lacunarity, Offset);

            switch (DrawMode)
            {
                case GenerationMode.NoiseMap:
                    MapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap, MapDisplay.FilterMode));
                    break;
                case GenerationMode.Mesh:
                    MapDisplay.DrawMesh(MeshGenerator.GenerateMesh(noiseMap, HeightScale), TextureGenerator.TextureFromHeightMap(noiseMap, MapDisplay.FilterMode));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
