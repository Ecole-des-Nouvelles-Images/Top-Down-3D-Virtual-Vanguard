using UnityEngine;
using UnityEngine.UI;

namespace Modules
{
    public abstract class Module : MonoBehaviour
    {
        [Header("UI Elements")]
        public Slider BatteryUI;
        public Image RechargeIndicator;
        
        [Header("Gameplay")]
        public int BatteryCapacity;

        public bool CanBeRecharged => BatteryCapacity > 0;
        public bool IsOccupied { get; protected set; }
        public abstract float CurrentBattery { get; set; }
        
        protected virtual void Start()
        {
            CurrentBattery = BatteryCapacity;
            
            if (BatteryUI == null) return;
            BatteryUI.minValue = 0;
            BatteryUI.maxValue = BatteryCapacity;
            UpdateBattery();
        }

        protected void UpdateBattery()
        {
            if (!BatteryUI) return;
            BatteryUI.value = CurrentBattery;
        }
        
        public void UpdateChargeStatus(bool enable)
        {
            Color color = enable ? Color.white : Color.black;

            RechargeIndicator.color = color;
        }
    }
}
