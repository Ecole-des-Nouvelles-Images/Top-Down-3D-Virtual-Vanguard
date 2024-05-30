using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Convoy;
using UnityEngine;
using Unity.AI.Navigation;

using Internal;
using Managers;
using Terrain.Procedural;

namespace Terrain
{
    public class TerrainManager: SingletonMonoBehaviour<TerrainManager>
    {
        [Header("References")]
        [SerializeField] private CinemachineVirtualCamera _transitCamera;
        [SerializeField] private Transform _chunksRoot;
        [SerializeField] private GameObject _chunkTransitPrefab;
        [SerializeField] private GameObject _railPrefab;
        [SerializeField] private List<GameObject> _stopsPrefabs;
        [Range(2, 5)] [SerializeField] private int _chunksQueueSize = 3;
        
        [Header("Transit phase")]
        public bool EnableTransit = false;
        [SerializeField] private float _scrollSpeed = 10f;
        [SerializeField] private float _offsetBetweenChunks = 240f;
        public float RepeatDistance = 240f;
        public float ScrolledDistance;
        
        private Queue<TerrainChunk> Chunks { get; set; }
        private TerrainChunk _lastEnqueued;
        private bool _reverseChunkScale;
        
        // Stops Zones
        private GameObject _currentStopZone;
        private MapGenerator _generator;
        private NavMeshSurface _navMesh;

        private void OnDrawGizmos()
        {
            Debug.DrawRay(Vector3.up * 30 + _chunksRoot.transform.position, (-_chunksRoot.right) * 100);
        }

        private void Start()
        {
            Chunks = new Queue<TerrainChunk>(_chunksQueueSize);
            float position = -_offsetBetweenChunks;
            
            for (int chunkIndex = -1; chunkIndex < _chunksQueueSize; chunkIndex++)
            {
                position += _offsetBetweenChunks;
                _lastEnqueued = CreateChunk(_chunksRoot, position);
                Chunks.Enqueue(_lastEnqueued);
            }
        }

        private void FixedUpdate()
        {
            if (!EnableTransit) return;
            
            MoveChunks(_scrollSpeed * Time.fixedDeltaTime);
            ScrolledDistance += _scrollSpeed * Time.fixedDeltaTime;

            RepeatDistance = 240;
            if (ScrolledDistance >= RepeatDistance)
            {
                UpdateRoad();
                ScrolledDistance -= ScrolledDistance;
            }
        }

        #region Chunk Management
        
        private void MoveChunks(float distance)
        {
            Vector3 direction = -_chunksRoot.right;
            
            foreach (TerrainChunk chunk in Chunks) {
                chunk.transform.Translate(direction * distance,Space.World);
            }
        }
        
        private void UpdateRoad()
        {
            _lastEnqueued = CreateChunk(_chunksRoot, _lastEnqueued.transform.localPosition.x + _offsetBetweenChunks);
            Chunks.Enqueue(_lastEnqueued);
            Destroy(Chunks.Dequeue().gameObject);
            //EditorApplication.isPaused = true;
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
        
        #endregion

        #region Phase Transitions

        public void GeneratePlayzone()
        {
            GameObject prefab = _stopsPrefabs[Random.Range(0, _stopsPrefabs.Count)];
            Vector3 position = _chunksRoot.transform.right * (_lastEnqueued.transform.localPosition.x + _offsetBetweenChunks);

            _currentStopZone = Instantiate(prefab, _chunksRoot);
            _currentStopZone.transform.position = position;

            TerrainChunk stopZone = _currentStopZone.GetComponent<TerrainChunk>();
            stopZone.SetupChunk(0);
            
            Chunks.Enqueue(stopZone);
        }

        public void FinishTransit()
        {
            StartCoroutine(ReachStopZone());
        }

        private IEnumerator ReachStopZone()
        {
            Transform convoy = FindAnyObjectByType<ConvoyManager>().transform;
            CinemachineBasicMultiChannelPerlin camNoise = _transitCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            
            Vector3 direction = -_chunksRoot.right;
            float initialDistance = _currentStopZone.transform.position.x;
            float initialShakeAmplitude = camNoise.m_AmplitudeGain;
            
            float actualDistance;
            float normalizeDistance;
            float currentSpeed;
            float stopThreshold = 1;
            
            while (_currentStopZone.transform.position.x > convoy.position.x + stopThreshold)
            {
                actualDistance = _currentStopZone.transform.position.x;
                normalizeDistance = actualDistance / initialDistance;
                
                float brakeMultiplier = 1 - normalizeDistance;
                currentSpeed = Mathf.Clamp(_scrollSpeed * brakeMultiplier, 8, _scrollSpeed); // TODO: extract "min" as a field or 
                
                float shakeMultiplier = 1 - (initialDistance - actualDistance) / initialDistance;
                camNoise.m_AmplitudeGain = Mathf.Lerp(initialShakeAmplitude, 0, shakeMultiplier);
                
                // Debug.Log($"Remaining Distance : {actualDistance}");
                
                foreach (TerrainChunk chunk in Chunks)
                {
                   chunk.transform.Translate(direction * (currentSpeed * Time.deltaTime), Space.World); 
                }

                yield return null;
            }

            GameManager.Instance.IsInTransit = false;
        }
        
        
        #endregion
    }
}
