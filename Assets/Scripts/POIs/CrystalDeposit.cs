using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace POIs
{
    public class CrystalDeposit: POI
    {
        public Vector2Int CapacityRange;
        [Range(0, 10)] public float MiningSpeedMultiplier = 1;

        public int Capacity { get; private set; }
        public int CurrentCapacity {
            get => _currentCapacity;
            set => _currentCapacity = Mathf.Clamp(value, 0, Capacity);
        }

        private Transform _camera;
        private RectTransform _ui;
        private Slider _uiGauge;
        private int _currentCapacity;

        private void Start()
        {
            _camera = CameraManager.Instance.CurrentCamera.transform;
            if (_camera == null) {
                throw new Exception("Crystal Deposit can't find the current camera");
            }
            
            _ui = GetComponentInChildren<RectTransform>();
            _uiGauge = GetComponentInChildren<Slider>();

            Capacity = Random.Range(CapacityRange.x, CapacityRange.y + 1);
            CurrentCapacity = Capacity;
            _uiGauge.maxValue = Capacity;
            _uiGauge.value = CurrentCapacity;
            
            _ui.rotation = _camera.rotation;
        }

        private void Update()
        {
            if (CurrentCapacity <= 0)
            {
                Debug.Log($"{name} depleted and destroyed");
                Destroy(this.gameObject);
            }
        }

        public void UpdateUIGauge()
        {
            _uiGauge.value = CurrentCapacity;
        }
    }
}
