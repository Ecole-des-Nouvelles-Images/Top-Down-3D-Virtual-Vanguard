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
        }

        public virtual void UpdateBattery(float capacity)
        {
            
        }
        
        public virtual void UpdateChargeStatus()
        {
            
        }
    }
}
