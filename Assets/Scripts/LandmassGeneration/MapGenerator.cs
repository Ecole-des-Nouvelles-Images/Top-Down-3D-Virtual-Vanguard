using System;
using UnityEngine;

namespace LandmassGeneration
{
    public class MapGenerator : MonoBehaviour
    {
        public int MapWidth;
        public int MapHeight;
        public float NoiseScale;

        public int Octaves;
        [Range(0, 1)] public float Persistance;
        public float Lacunarity;

        public int Seed;
        public Vector2 Offset;

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
            float[,] noiseMap = Noise.GenerateNoiseMap(MapWidth, MapHeight, Seed, NoiseScale, Octaves, Persistance, Lacunarity, Offset);
            
            FindObjectOfType<MapDisplay>().DrawNoiseMap(noiseMap);
        }
    }
}
