using UnityEngine;

using Terrain.Procedural;
using Random = UnityEngine.Random;

namespace Terrain
{
    public class TerrainManager : MonoBehaviour
    {
        [Header("Props Generation")]
        public Transform PropsParent;
        public GameObject CristalPrefab;
        [Range(0, 100)] public int CristalDensity = 10;

        [Header("Gameplay Settings")]
        public LayerMask TerrainLayer;
        [Tooltip("Bounding box for spawning object on axis (x, z)")]
        public Vector2 PropsBounds = new Vector2(150, 160);
        [Tooltip("Width of the \"lane\" on the z-axis around the Convoy")]
        public float ConvoyDeadzoneWidth = 15;
        public float RaycastHeight = 10;
        public float SlowDownTransitionTime;
        
        [Header("Experimental")]
        public MapGenerator Generator;
        public bool EnableScrolling;
        public float ScrollSpeed;
        
        public const int TerrainChunkSize = 240;

        #region Debug

        private void OnDrawGizmosSelected()
        {
            // Get the BoundsX and BoundsY parameters
            float boundsX = PropsBounds.x; // replace with your value
            float boundsY = PropsBounds.y; // replace with your value

            // Get the position of the object
            Vector3 position = transform.position;

            // Calculate the four corners of the box
            Vector3 corner1 = new Vector3(position.x - boundsX/2, RaycastHeight, position.z - boundsY/2);
            Vector3 corner2 = new Vector3(position.x + boundsX/2, RaycastHeight, position.z - boundsY/2);
            Vector3 corner3 = new Vector3(position.x + boundsX/2, RaycastHeight, position.z + boundsY/2);
            Vector3 corner4 = new Vector3(position.x - boundsX/2, RaycastHeight, position.z + boundsY/2);

            // Draw the lines connecting the corners to form the box edge
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(corner1, corner2);
            Gizmos.DrawLine(corner2, corner3);
            Gizmos.DrawLine(corner3, corner4);
            Gizmos.DrawLine(corner4, corner1);
        }

        #endregion

        private void Start()
        {
            Generator.GenerateMap();
            GenerateCrystals();
        }

        [ContextMenu("Generate Crystals")]
        public void GenerateCrystals()
        {
            for (int crystals = 0; crystals <= CristalDensity; crystals++)
            {
                int rayPosX = Mathf.RoundToInt(Random.Range(-PropsBounds.x / 2, PropsBounds.x / 2 + 1));
                int rayPosZ = Mathf.RoundToInt(Random.Range(-PropsBounds.y / 2, PropsBounds.y / 2 + 1));
                
                if (rayPosZ <= 0 && rayPosZ > -ConvoyDeadzoneWidth || rayPosZ >= 0 && rayPosZ < ConvoyDeadzoneWidth) {
                    continue;
                }
                
                Vector3 raycastOrigin = new(rayPosX, RaycastHeight, rayPosZ);

                if (Physics.Raycast(raycastOrigin, transform.TransformDirection(Vector3.down), out RaycastHit rayHit, RaycastHeight * 2,  TerrainLayer))
                {
                    GameObject prop = Instantiate(CristalPrefab, rayHit.point + Vector3.down * 0.5f, Quaternion.identity, PropsParent);
                    prop.name = "CrystalDeposit_" + crystals;
                }
            }
        }

        private void Update()
        {
            if (EnableScrolling)
            {
                Generator.Offset.x += ScrollSpeed * Time.deltaTime;
                Generator.GenerateMap();
            }
        }

        /* public void StopScrolling()
        {
            StartCoroutine(SlowDownScrollingCoroutine());
        } */

        /* private IEnumerator SlowDownScrollingCoroutine()
        {
            float t = 0f;

            while (t > 0)
            {
                t += Time.deltaTime * SlowDownTransitionTime;
                Generator.Offset.x = Mathf.Lerp(0, ScrollSpeed, t);
                yield return null;
            }
        } */
    }
}
