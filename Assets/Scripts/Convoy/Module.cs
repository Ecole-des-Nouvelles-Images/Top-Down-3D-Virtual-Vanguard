using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using Player;
using UnityEditor;

namespace Convoy
{
    public abstract class Module : MonoBehaviour
    {
        [Header("Player Management")] [SerializeField]
        protected int MaximumControllers = 1;

        [Header("Settings")] public bool Online;
        [SerializeField] protected int BatteryCapacity;
        [SerializeField] protected int ConsumptionPerSecond;

        public string Type => GetType().ToString();
        public int BatteryMaxCapacity => BatteryCapacity;
        public float BatteryCharge { get; set; }

        protected readonly List<PlayerController> Controllers = new();
        protected bool IsFull
        {
            get
            {
                if (Controllers.Count > MaximumControllers) throw new Exception($"Error: Module {name} registered too much players !");
                return Controllers.Count == MaximumControllers;
            }
        }
        protected bool IsOperated => Controllers.Count > 0;

        #region Debug

        protected virtual void OnDrawGizmos()
        {
            string batteryStatus = BatteryMaxCapacity > 0 ? $"Battery: {Mathf.RoundToInt(BatteryCharge)}/{Mathf.RoundToInt(BatteryCapacity)}" : "No battery";
            Handles.Label(transform.position + (Vector3.down *2) + (Vector3.left * 2), batteryStatus);
        }

        #endregion
        
        protected virtual void Awake()
        {
            Online = true;
            // BatteryCharge = BatteryCapacity;
        }

        protected virtual void Update()
        {
            if (!Online)
                Deactivate();
        }

        public void Deactivate()
        {
            foreach (PlayerController player in Controllers)
            {
                ExitModule(player);
            }

            Online = false;
        }

        #region Actions

        public void EnterModule(PlayerController newController)
        {
            if (IsFull || !Online) {
                Debug.Log($"Can't enter in module {name}...");
                return;
            }

            Controllers.Add(newController);
            newController.IsBusy = true;
            Debug.Log($"Entering module {name}");
        }

        public virtual void ExitModule(PlayerController currentController)
        {
            if (!Controllers.Contains(currentController))
                return;

            Controllers.Remove(currentController);
            currentController.IsBusy = false;
            Debug.Log($"Exiting module {name}");
        }

        public abstract void Operate();

        public virtual void Interact() {}

        public virtual void Aim(InputValue input) {}

        #endregion
    }
}
