﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.AI.Navigation;

using Cinemachine;
using Internal;
using Game.Terrain.Procedural;
using Game.Convoy;

namespace Game.Terrain
{
    public class TerrainManager: SingletonMonoBehaviour<TerrainManager>
    {
        [Header("References")]
        [SerializeField] private CinemachineVirtualCamera _transitCamera;
        [SerializeField] private Transform _chunksRoot;
        [SerializeField] private GameObject _chunkTransitPrefab;
        [SerializeField] private List<GameObject> _stopsPrefabs;
        [Range(2, 5)] [SerializeField] private int _chunksQueueSize = 3;
        
        [Header("Transit phase")]
        public bool EnableTransit;
        [SerializeField] private float _scrollSpeed = 10f;
        [SerializeField] private float _offsetBetweenChunks = 240f;
        public float RepeatDistance = 240f;
        public float ScrolledDistance;
        
        private Queue<TerrainChunk> Chunks { get; set; }
        private TerrainChunk _lastEnqueued;

        private Transform _convoy;
        CinemachineBasicMultiChannelPerlin _camNoise;
        
        // Stops Zones
        private GameObject _currentStopZone;
        private MapGenerator _generator;
        private NavMeshSurface _navMesh;

        private void Start()
        {
            Chunks = new Queue<TerrainChunk>(_chunksQueueSize);
            _camNoise = _transitCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _convoy = FindAnyObjectByType<ConvoyManager>().transform;
            
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
            
            MoveChunks(_scrollSpeed, Time.fixedDeltaTime);
            
            if (ScrolledDistance >= RepeatDistance)
            {
                UpdateRoad();
                ScrolledDistance -= ScrolledDistance;
            }
        }

        #region Chunk Management
        
        private void MoveChunks(float speed, float timeDelta)
        {
            Vector3 direction = -_chunksRoot.right;
            
            foreach (TerrainChunk chunk in Chunks) {
                chunk.transform.Translate(direction * (speed * timeDelta),Space.World);
            }

            ScrolledDistance += speed * timeDelta;
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
            
            // chunk.SetupChunk(); // TODO: Disable to avoid overheads when using flat terrain over the MapGenerator;

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
            // stopZone.SetupChunk(); // TODO: Disable to avoid overheads when using flat terrain over the MapGenerator;
            
            Chunks.Enqueue(stopZone);
            _lastEnqueued = stopZone;
        }

        public void FinishTransit()
        {
            StartCoroutine(ReachStopZone());
        }

        private IEnumerator ReachStopZone()
        {
            float initialDistance = _currentStopZone.transform.position.x;
            float initialShakeAmplitude = _camNoise.m_AmplitudeGain;
            
            float actualDistance;
            float normalizeDistance;
            float currentSpeed;
            float stopThreshold = 0;
            
            while (_currentStopZone.transform.position.x > _convoy.position.x + stopThreshold)
            {
                actualDistance = _currentStopZone.transform.position.x;
                normalizeDistance = actualDistance / initialDistance;
                
                float brakeMultiplier = normalizeDistance;
                float shakeMultiplier = normalizeDistance;
                
                currentSpeed = Mathf.Clamp(_scrollSpeed * brakeMultiplier, 3, _scrollSpeed); // TODO: extract "min" as a field or rework smooth formula
                _camNoise.m_AmplitudeGain = Mathf.Lerp(initialShakeAmplitude, 0, 1 - shakeMultiplier);
                
                MoveChunks(currentSpeed, Time.deltaTime);

                yield return null;
            }

            TerrainChunk stopZoneChunk = _currentStopZone.GetComponent<TerrainChunk>();
            CameraManager.Instance.SwitchCameraFocus(false, stopZoneChunk.PlayableSide);
            GameManager.Instance.OnStopZoneReached.Invoke(stopZoneChunk);
        }
        
        public void RestartTransit()
        {
            foreach (TerrainChunk chunk in Chunks.ToList())
            {
                if (chunk == _lastEnqueued) continue;

                Destroy(Chunks.Dequeue().gameObject);
            }

            StartCoroutine(AccelerateConvoy());
        }

        private IEnumerator AccelerateConvoy()
        {
            TerrainChunk startingChunk = CreateChunk(_chunksRoot, _lastEnqueued.transform.localPosition.x + _offsetBetweenChunks);
            float currentSpeed;
            
            CameraManager.Instance.SwitchCameraFocus(true, Side.Centered);
            
            Chunks.Enqueue(startingChunk);
            _lastEnqueued = startingChunk;
            
            ScrolledDistance = 0f;

            while (ScrolledDistance < 240)
            {
                float mulitplier = (ScrolledDistance / 240) + 0.01f ; // TODO: Starting speed controllable with 'ScrolledDistance' starting offset
                currentSpeed = Mathf.Clamp(_scrollSpeed * mulitplier, 0, _scrollSpeed);
                _camNoise.m_AmplitudeGain = 0.08f * mulitplier;
                
                MoveChunks(currentSpeed, Time.deltaTime);

                yield return null;
            }
            
            EnableTransit = true;
            GameManager.Instance.IsInTransit = true;
        }

        #endregion
    }
}
