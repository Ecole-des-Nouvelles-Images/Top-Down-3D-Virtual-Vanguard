using System;
using System.Collections.Generic;
using System.Linq;
using Game.Convoy;
using Game.POIs;
using Game.Terrain;
using Internal;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        public int Crystals { get; set; }

        [Header("References")]
        [SerializeField] private ConvoyManager _convoy;
        [SerializeField] private GameObject _pointsOfInterest;
        
        [Header("Phase parameters")] 
        public bool StartFollowCam = true;

        public Action OnStopTransit;
        public Action OnStartTransit;
        public Action<TerrainChunk> OnStopZoneReached;

        public bool IsInTransit = true;

        private TerrainChunk _currentStopZone;
        
        #region Debug

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
            CameraManager.Instance.SwitchCameraFocus(StartFollowCam, Side.Centered);
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
            if (!_convoy.Operational)
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
            _currentStopZone.WaveManager.WaveInProgress = false;
            // _currentStopZone.WaveManager.StopOffensive();
            TerrainManager.Instance.RestartTransit();
        }
        
        private void StartXenolithWave(TerrainChunk stopZone)
        {
            IsInTransit = false;
            _currentStopZone = stopZone;
            _currentStopZone.NavMesh.BuildNavMesh();
            _currentStopZone.WaveManager.StartXenolithOffensive();
        }
        
        #endregion

        #region Utils

        private int GetRemainingCrystals()
        {
            List<CrystalDeposit> crystals = _pointsOfInterest.GetComponentsInChildren<CrystalDeposit>().ToList();
            int totalCrystalAmountRemaining = 0;
            
            foreach (CrystalDeposit deposit in crystals)
                totalCrystalAmountRemaining += deposit.CurrentCapacity;

            return totalCrystalAmountRemaining;
        }

        #endregion
        

    }
}
