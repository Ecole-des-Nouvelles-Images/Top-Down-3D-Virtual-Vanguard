using System;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public class TransitManager: MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _chunkTransitPrefab;
        [SerializeField] private List<GameObject> _stopsPrefabs;
        [SerializeField] private GameObject _railPrefab;
        [SerializeField] private Transform _chunksRoot;
        [Range(2, 5)] [SerializeField] private int _chunksQueueSize = 3;
        
        [Header("Transform values")]
        [SerializeField] private float _generalOffset = 169.7f;
        [SerializeField] private float _groundHeight = 2.2f;
        [SerializeField] private float _railHeight = -3.55f;

        [Header("Transit phase")] 
        [SerializeField] private bool _enableTransit = false;
        [SerializeField] private float _scrollSpeed = 10f;
        

        public static int ChunkNumber = -1;
        public static Queue<TerrainChunk> Chunks;
        
        public float CumulatedPosition { get; set; }

        private Vector3 MapDirection => transform.right + transform.forward;
        private Quaternion MapOrientation => Quaternion.Euler(0, -45, 0);

        private bool _ignoreFirstChunkCrossed = true;
        private bool _reverseChunkScale;

        private void Start()
        {
            CumulatedPosition = -169.7f;
            Chunks = new Queue<TerrainChunk>(_chunksQueueSize);

            for (int chunkIndex = -1; chunkIndex < _chunksQueueSize; chunkIndex++)
            {
                Chunks.Enqueue(CreateChunk(_chunksRoot));
            }
        }

        private void Update()
        {
            Vector3 direction = -(transform.right + transform.forward);
            
            if (_enableTransit) {
                _chunksRoot.transform.Translate(direction * (_scrollSpeed * Time.deltaTime));
            }
        }

        #region Logic

        public TerrainChunk CreateChunk(Transform root)
        {
            GameObject instance = Instantiate(_chunkTransitPrefab, MapDirection * _generalOffset * 2, MapOrientation, root);
            TerrainChunk chunk = instance.GetComponent<TerrainChunk>();

            if (_reverseChunkScale)
                chunk.Terrain.transform.localScale = new Vector3(-1, 1, 1);
            
            Instantiate(_railPrefab, new Vector3(_generalOffset * ChunkNumber, _railHeight, _generalOffset * ChunkNumber), MapOrientation, instance.transform);

            chunk.SetupChunk(0);
            
            CumulatedPosition += _generalOffset;
            _reverseChunkScale = !_reverseChunkScale;
            ChunkNumber++;
            return chunk;
        }

        public void UpdateRoad()
        {
            Chunks.Enqueue(CreateChunk(_chunksRoot));
            Destroy(Chunks.Dequeue().gameObject);
        }

        #endregion
    }
}
