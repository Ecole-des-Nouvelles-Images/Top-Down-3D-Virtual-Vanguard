using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

using Gameplay;
using Internal;
using POIs;
using Convoy;

namespace Managers
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        public int Crystals { get; set; }

        [Header("References")]
        [SerializeField] private ConvoyManager _convoy;
        [SerializeField] private GameObject _POI;

        [Header("Phase parameters")]
        public FocusMode FocusMode = FocusMode.Centered;
        public bool UseFarthermostCamera = true;
        
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
            CameraManager.Instance.SwitchCameraFocus(FocusMode, UseFarthermostCamera);
        }

        private void Update()
        {
            if (_convoy.Durability <= 0)
            {
                Debug.Log("Editor warning: Exiting playmode (Convoy destroyed)");
                EditorApplication.ExitPlaymode();
            }
            
            if (GetRemainingCrystals() == 0)
            {
                Debug.LogWarning("Editor warning: Exiting playmode (no more crystal)");
                EditorApplication.ExitPlaymode();
            }
        }
        
        private int GetRemainingCrystals()
        {
            List<CrystalDeposit> crystals = _POI.GetComponentsInChildren<CrystalDeposit>().ToList(); // Cache reference ?
            int totalCrystalAmountRemaining = 0;
            
            foreach (CrystalDeposit deposit in crystals)
                totalCrystalAmountRemaining += deposit.CurrentCapacity;

            return totalCrystalAmountRemaining;
        }
    }
}
