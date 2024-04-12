using UnityEngine;

namespace Modules
{
    public abstract class Module : MonoBehaviour
    {
        [Header("Gameplay")]
        public int BatteryCapacity;

        public bool IsOccupied { get; private set; }
    }
}
