using System;
using UnityEngine;
using Unity.AI.Navigation;
using Random = UnityEngine.Random;

using Internal;
using Terrain.Procedural;
using Gameplay;

namespace Managers
{
    public class TerrainManager : SingletonMonoBehaviour<TerrainManager>
    {
        [Header("Props Generation")]
        public Transform PropsParent;
        public GameObject CristalPrefab;
        [Range(0, 100)] public int CristalDensity = 10;

        [Header("References")] 
        public Transform Convoy;
        public LayerMask TerrainLayer;
        public Collider RightSide;
        public Collider LeftSide;

        [Header("Procedural settings")]
        public float RaycastHeight = 10;
        
        [Header("Experimental")]
        public MapGenerator Generator;
        public bool EnableScrolling;
        public float ScrollSpeed;
        
        public const int TerrainChunkSize = 240;

        private NavMeshSurface _navMesh;

        private void Awake()
        {
            _navMesh = GetComponent<NavMeshSurface>();
        }

        private void Start()
        {
            Generator.GenerateMap();
            GenerateProps();
            _navMesh.BuildNavMesh();
        }
        
        private void Update()
        {
            if (EnableScrolling)
            {
                Generator.Offset.x += ScrollSpeed * Time.deltaTime;
                Generator.GenerateMap();
            }
        }

        public void UpdateNavMesh()
        {
            _navMesh.BuildNavMesh();
        }

        #region Props

        public void GenerateProps()
        {
            GenerateCrystals(GameManager.Instance.FocusMode);
        }

        [ContextMenu("Generate Crystals")]
        public void GenerateCrystals(FocusMode mode)
        {
            // Debug.Log($"L-Bounds: [min: {LeftSide.bounds.min} / max: {LeftSide.bounds.max}]; R-Bounds: [min: {RightSide.bounds.min} / max: {RightSide.bounds.max}]");
            
            for (int crystals = 0; crystals <= CristalDensity; crystals++)
            {
                float rayPosX = 0;
                float rayPosZ = 0;

                switch (mode)
                {
                    case FocusMode.Left:
                        rayPosX = Random.Range(LeftSide.bounds.min.x, LeftSide.bounds.max.x);
                        rayPosZ = Random.Range(LeftSide.bounds.min.z, LeftSide.bounds.max.z);
                        break;
                    case FocusMode.Right:
                        rayPosX = Random.Range(RightSide.bounds.min.x, RightSide.bounds.max.x);
                        rayPosZ = Random.Range(RightSide.bounds.min.z, RightSide.bounds.max.z);
                        break;
                    case FocusMode.Centered:
                        if (Random.value <= 0.5f) {
                            rayPosX = Random.Range(LeftSide.bounds.min.x, LeftSide.bounds.max.x);
                            rayPosZ = Random.Range(LeftSide.bounds.min.z, LeftSide.bounds.max.z);
                        }
                        else {
                            rayPosX = Random.Range(RightSide.bounds.min.x, RightSide.bounds.max.x);
                            rayPosZ = Random.Range(RightSide.bounds.min.z, RightSide.bounds.max.z);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }
                
                Vector3 raycastOrigin = new(rayPosX, RaycastHeight, rayPosZ);

                if (Physics.Raycast(raycastOrigin, transform.TransformDirection(Vector3.down), out RaycastHit rayHit, RaycastHeight * 2, TerrainLayer))
                {
                    Debug.DrawRay(raycastOrigin, transform.TransformDirection(Vector3.down) * rayHit.distance, Color.green, 30);
                    GameObject prop = Instantiate(CristalPrefab, rayHit.point + Vector3.down * 1.3f, Quaternion.identity, PropsParent);
                    prop.name = "CrystalDeposit_" + crystals;
                }
                else
                {
                    Debug.Log($"Missing rays at: ({rayPosX}, {rayPosZ})");
                    Debug.DrawRay(raycastOrigin, transform.TransformDirection(Vector3.down) * RaycastHeight, Color.red, 30);
                }
            }
        }

        #endregion
    }
}
