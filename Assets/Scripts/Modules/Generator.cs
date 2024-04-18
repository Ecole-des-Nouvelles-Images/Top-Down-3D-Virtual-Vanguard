using UnityEngine;

namespace Modules
{
    public class Generator: Module
    {
        public int PowerOutputPerSecond;
        public float TickRate;

        public int CoupledModuleIndex { get; private set; }
        
        public override float CurrentBattery { get; set; }

        private float _currentBattery;
        private float _timer = 0f;

        protected override void Start()
        {
            base.Start();
            CoupledModuleIndex = 3;
            UpdateChargeStatus(true);
        }

        private void Update()
        {
            Module moduleToRecharge = Convoy.Modules[CoupledModuleIndex];
            
            if (moduleToRecharge.BatteryCapacity <= 0 || !moduleToRecharge.CanBeRecharged)
                return;

            if (_timer >= TickRate)
            {
                Convoy.Modules[CoupledModuleIndex].CurrentBattery += PowerOutputPerSecond * TickRate;
                _timer = 0f;
            }

            _timer += Time.deltaTime;
        }

        public void SwitchEnergy(int direction)
        {
            Convoy.Modules[CoupledModuleIndex].UpdateChargeStatus(false);
            
            if (direction == -1) {
                if (CoupledModuleIndex <= 0) CoupledModuleIndex = 0;
                else CoupledModuleIndex--;
            }
            else if (direction == 1) {
                if (CoupledModuleIndex < Convoy.Modules.Count - 1) CoupledModuleIndex++;
                else CoupledModuleIndex = Convoy.Modules.Count - 1;
            }
            
            Convoy.Modules[CoupledModuleIndex].UpdateChargeStatus(true);
        }
    }
}
