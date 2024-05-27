using System;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public class TransitManager: MonoBehaviour
    {
        [Header("References")]
        public GameObject GroundPrefab;
        public GameObject RailPrefab;
        public Transform RailsRoot;
        public Transform ChunksRoot;
        [Range(2, 5)] public int ChunksQueueSize;

        public static int ChunkNumber = 0;
        public Queue<TerrainChunk> Chunks;
        
        private const int ChunkSize = 240;
        private float GroundHeight => -5.8f;

        private Vector3 MapDirection => transform.right + transform.forward;
        private Vector3 Position { get; set; }
        private readonly Quaternion MapOrientation = Quaternion.Euler(0, -45, 0);

        private float CumulatedPosition
        {
            get => _cumulatedPositionRaw;
            set
            {
                _cumulatedPositionRaw = value;
                Position = new Vector3(_cumulatedPositionRaw, GroundHeight, _cumulatedPositionRaw);
            }
        }
        private float _cumulatedPositionRaw = 0f;
        private float CumulatedOffset { get; set; } = 0f;

        private void Start()
        {
            bool reverseScale = false;
            CumulatedPosition = 0f;
            Chunks = new Queue<TerrainChunk>(ChunksQueueSize);

            for (int chunkIndex = 0; chunkIndex < ChunksQueueSize; chunkIndex++)
            {
                Chunks.Enqueue(CreateChunk(ChunksRoot, CumulatedOffset, reverseScale));
                CumulatedPosition += 169.7f;
                // CumulatedOffset = 0f;
                reverseScale = !reverseScale;
                ChunkNumber++;
            }
        }

        private TerrainChunk CreateChunk(Transform root, float offset, bool reverse)
        {
            GameObject instance = Instantiate(GroundPrefab, MapDirection * CumulatedPosition, MapOrientation, root);
            TerrainChunk chunk = instance.GetComponent<TerrainChunk>();

            if (reverse)
                instance.transform.localScale = new Vector3(-1, 1, 1);

            if (ChunkNumber % 2 == 0)
            {
                Instantiate(RailPrefab, new Vector3((84.85f * ChunkNumber * 2) + 84.85f, 2.2f, (84.85f * ChunkNumber * 2) + 84.85f), MapOrientation, RailsRoot);
            }
            
            chunk.SetupChunk(offset);
            return chunk;
        }
    }
}
