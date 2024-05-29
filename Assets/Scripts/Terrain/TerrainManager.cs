using System.Collections.Generic;
using Internal;
using UnityEngine;
using Unity.AI.Navigation;

using Terrain.Procedural;

namespace Terrain
{
    public class TerrainManager: SingletonMonoBehaviour<TerrainManager>
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
        
        private Queue<TerrainChunk> Chunks { get; set; }
        private bool _reverseChunkScale;

        public float ScrolledDistance;
        private TerrainChunk _lastEnqueued;
        
        // Stops Zones
        private MapGenerator _generator;
        private NavMeshSurface _navMesh;

        private void OnDrawGizmos()
        {
            Debug.DrawRay(Vector3.up * 30 + _chunksRoot.transform.position, (-_chunksRoot.right) * 100);
        }

        private void Start()
        {
            Chunks = new Queue<TerrainChunk>(_chunksQueueSize);
            float position = -_generalOffset * 2;
            
            for (int chunkIndex = -1; chunkIndex < _chunksQueueSize; chunkIndex++)
            {
                position += _generalOffset;
                _lastEnqueued = CreateChunk(_chunksRoot, position);
                Chunks.Enqueue(_lastEnqueued);
            }
        }

        private void FixedUpdate()
        {
            Vector3 direction = -_chunksRoot.right;

            if (!_enableTransit) return;
            
            HandleTransit(direction);
        }

        #region Logic
        
        private void HandleTransit(Vector3 direction)
        {
            foreach (TerrainChunk chunk in Chunks) {
                chunk.transform.Translate(direction * (_scrollSpeed * Time.fixedDeltaTime),Space.World);
            }

            ScrolledDistance += _scrollSpeed * Time.fixedDeltaTime;
            if (ScrolledDistance >= 241.2f)
            {
                UpdateRoad();
                ScrolledDistance -= ScrolledDistance;
            }
        }

        private TerrainChunk CreateChunk(Transform root, float positionScalar)
        {
            GameObject instance = Instantiate(_chunkTransitPrefab, root);
            TerrainChunk chunk = instance.GetComponent<TerrainChunk>();

            Vector3 position = _chunksRoot.transform.right * positionScalar;
            instance.transform.position = position;

            if (_reverseChunkScale)
                chunk.Terrain.transform.localScale = new Vector3(-1, 1, 1);
            
            chunk.SetupChunk(0);
            _reverseChunkScale = !_reverseChunkScale;

            return chunk;
        }

        private void UpdateRoad()
        {
            _lastEnqueued = CreateChunk(_chunksRoot, _lastEnqueued.transform.localPosition.x+_generalOffset);
            Chunks.Enqueue(_lastEnqueued);
            Destroy(Chunks.Dequeue().gameObject);
            //EditorApplication.isPaused = true;
        }

        #endregion
    }
}
