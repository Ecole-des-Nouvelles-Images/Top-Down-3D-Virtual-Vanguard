using UnityEngine;

namespace Modules
{
    public class Shield: Module
    {
        public int EnergyConsumptionPerSecond;

        [Header("Settings")]
        public float TickRate;

        private float _currentBattery;
        private float _timer;

        protected override void Start()
        {
            base.Start();
            BatteryUI.minValue = 0;
            BatteryUI.maxValue = BatteryCapacity;
        }

        public override float CurrentBattery
        {
            get => _currentBattery;
            set {
                _currentBattery = value;
                _currentBattery = Mathf.Clamp(_currentBattery, 0, BatteryCapacity);
                UIManager.UpdateBattery.Invoke(BatteryUI, _currentBattery);
            }
        }
    }
}
