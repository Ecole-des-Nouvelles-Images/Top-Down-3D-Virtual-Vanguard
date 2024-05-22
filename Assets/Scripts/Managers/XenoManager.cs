using System;
using System.Collections.Generic;
using Gameplay;
using Internal;
using UnityEngine;

namespace Managers
{
    public class XenoManager : SingletonMonoBehaviour<XenoManager>
    {
        [Header("References")]
        [SerializeField] private List<GameObject> _xenolithsPrefabs;
        
        [Header("Spawns locations")]
        [SerializeField] private Collider _spawnerLeft;
        [SerializeField] private Collider _spawnerRight;

        [Header("Debug")]
        public bool DebugUnitsForwardDirection = false; 

        private void Start()
        {
            switch (GameManager.Instance.FocusMode)
            {
                case FocusMode.Left:
                    break;
                case FocusMode.Right:
                    break;
                case FocusMode.Centered:
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"XenoManager: Unexpected mode {GameManager.Instance.FocusMode} receive.");
            }
        }

        private void StartXenolithOffensive(List<GameObject> xenolithTypes, List<Bounds> spawnLocations)
        {
            
        }
    }
}
