using UnityEngine;

namespace Modules
{
    public class Shield: Module
    {
        public int EnergyConsumptionPerSecond;

        [Header("References")]
        public GameObject BarrierLeft;
        public GameObject BarrierRight;

        [Header("Settings")]
        public float TickRate;

        public override float CurrentBattery
        {
            get => _currentBattery;
            set {
                _currentBattery = value;
                _currentBattery = Mathf.Clamp(_currentBattery, 0, BatteryCapacity);
                UpdateBattery();
            }
        }

        private bool PowerOn { get; set; }
        private float _currentBattery;
        private float _timer;
        private int _side = 1;

        private void Update()
        {
            if (!PowerOn) return;
            
            if (_timer >= TickRate)
            {
                if (CurrentBattery > 0)
                    CurrentBattery -= EnergyConsumptionPerSecond * TickRate;
                else
                    SwitchOff();
                
                _timer = 0;
            }

            _timer += Time.deltaTime;
        }

        public void SwitchPower()
        {
            if (!(CurrentBattery > 0)) return;

            if (PowerOn)
            {
                SwitchOff();
            }
            else
            {
                PowerOn = true;
                switch (_side) {
                    case 1:
                        BarrierRight.SetActive(true);
                        break;
                    case -1:
                        BarrierLeft.SetActive(true);
                        break;
                }
            }
        }

        public void SwitchOff()
        {
            PowerOn = false;
            BarrierLeft.SetActive(false);
            BarrierRight.SetActive(false);
        }

        public void SwitchPolarity()
        {
            if (!PowerOn) return;
            
            if (_side == 1)
            {
                _side = -1;
                BarrierRight.SetActive(false);
                BarrierLeft.SetActive(true);
            }
            else
            {
                _side = 1;
                BarrierRight.SetActive(true);
                BarrierLeft.SetActive(false);
            }
        }
    }
}
