using System.Numerics;
using UnityEngine;

using Terrain.Procedural;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Terrain
{
    public class TerrainManager : MonoBehaviour
    {
        [Header("Props Generation")]
        public Transform PropsParent;
        public GameObject CristalPrefab;
        [Range(0, 100)] public int CristalDensity = 10;
        
        [Header("Gameplay Settings")]
        public float RaycastHeight = 10;
        public float ConvoyDeadzoneWidth = 15;
        public float BoundariesSpacing = 20;
        public float SlowDownTransitionTime;
        
        [Header("Experimental")]
        public MapGenerator Generator;
        public bool EnableScrolling;
        public float ScrollSpeed;
        
        public const int TerrainChunkSize = 240;

        private void Start()
        {
            GenerateCristals();
        }

        [ContextMenu("Generate Cristals")]
        public void GenerateCristals()
        {
            for (int cristals = 0; cristals <= CristalDensity; cristals++)
            {
                int rayPosX = Mathf.RoundToInt(Random.Range(-TerrainChunkSize / 2 + BoundariesSpacing, TerrainChunkSize / 2 + 1 - BoundariesSpacing));
                int rayPosZ = Mathf.RoundToInt(Random.Range(-TerrainChunkSize / 2 + BoundariesSpacing, TerrainChunkSize / 2 + 1 - BoundariesSpacing));

                string debug = $"RayZ: {rayPosZ}";
                if ((rayPosZ < 0 && rayPosZ > -ConvoyDeadzoneWidth) || (rayPosZ > 0 && rayPosZ < ConvoyDeadzoneWidth)) debug += "- Near train: true";
                Debug.Log(debug);
                
                if (rayPosZ > -ConvoyDeadzoneWidth || rayPosZ < ConvoyDeadzoneWidth) {
                    continue;
                }
                
                Vector3 raycastOrigin = new(rayPosX, RaycastHeight, rayPosZ);

                if (Physics.Raycast(raycastOrigin, transform.TransformDirection(Vector3.down), out RaycastHit rayHit, RaycastHeight * 2))
                {
                    Debug.DrawRay(raycastOrigin, transform.TransformDirection(Vector3.down) * rayHit.distance, Color.green, 30);
                    Debug.Log($"Ray n°{cristals} hit at {rayHit.point}");
                    Instantiate(CristalPrefab, rayHit.point + Vector3.up * 3, Quaternion.identity);
                }
                else
                {
                    Debug.DrawRay(raycastOrigin, transform.TransformDirection(Vector3.down) * RaycastHeight, Color.red, 30);
                    Debug.Log($"Ray n°{cristals} didn't Hit (RaycastOrigin: {raycastOrigin} / RaycastEnd: {transform.TransformDirection(Vector3.down) * RaycastHeight * rayHit.distance}");
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
