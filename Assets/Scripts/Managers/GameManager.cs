using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

using Gameplay;
using Internal;
using Convoy;
using Foes;
using POIs;
using Terrain;

namespace Managers
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        public int Crystals { get; set; }

        [Header("References")]
        [SerializeField] private ConvoyManager _convoy;
        [SerializeField] private GameObject _poi;
        [SerializeField] private XenolithSpawner _xenolithSpawner;
        
        [Header("Phase parameters")] 
        public Side Side = Side.Centered;
        public bool UseFarthermostCamera = true;

        public Action OnStopTransit;
        public Action OnStartTransit;
        public Action<TerrainChunk> OnStopZoneReached;

        public bool IsInTransit = true;

        private TerrainChunk _currentStopZone;
        
        #region Debug

        private void OnValidate()
        {
            if (Side == Side.None)
            {
                Debug.LogWarning("GameManager: Side property can't be set to 'None'. Falling back to 'Centered'.");
                Side = Side.Centered;
            }
        }

        void OnGUI()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 36;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            Rect rect = new Rect(10, 10, 200, 100);
            GUI.Box(rect, "Crystals: " + Crystals, style);
        }

        #endregion

        private void Start()
        {
            CameraManager.Instance.SwitchCameraFocus(Side, UseFarthermostCamera);
        }
        
        private void OnEnable()
        {
            OnStopTransit += StopConvoy;
            OnStartTransit += StartTransit;
            OnStopZoneReached += StartXenolithWave;
        }

        private void OnDisable()
        {
            OnStopTransit -= StopConvoy;
            OnStartTransit -= StartTransit;
            OnStopZoneReached -= StartXenolithWave;
        }

        private void Update()
        {
            if (_convoy.Durability <= 0)
            {
                Debug.Log("Editor warning: Exiting playmode (Convoy destroyed)");
                EditorApplication.ExitPlaymode();
            }
        }

        #region Events

        private void StopConvoy()
        {
            TerrainManager.Instance.EnableTransit = false;
            TerrainManager.Instance.GeneratePlayzone();
            TerrainManager.Instance.FinishTransit();
        }
        
        private void StartTransit()
        {
            _currentStopZone.XenoManager.WaveInProgress = false;
            _currentStopZone.XenoManager.StopOffensive();
            TerrainManager.Instance.RestartTransit();
        }
        
        private void StartXenolithWave(TerrainChunk stopZone)
        {
            IsInTransit = false;
            _currentStopZone = stopZone;
            _currentStopZone.NavMesh.BuildNavMesh();
            _currentStopZone.XenoManager.StartXenolithOffensive();
        }
        
        #endregion

        #region Utils

        private int GetRemainingCrystals()
        {
            List<CrystalDeposit> crystals = _poi.GetComponentsInChildren<CrystalDeposit>().ToList();
            int totalCrystalAmountRemaining = 0;
            
            foreach (CrystalDeposit deposit in crystals)
                totalCrystalAmountRemaining += deposit.CurrentCapacity;

            return totalCrystalAmountRemaining;
        }

        #endregion
        

    }
}
