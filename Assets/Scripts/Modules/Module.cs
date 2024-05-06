using System;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Modules
{
    public abstract class Module : MonoBehaviour
    {
        [Header("Player Management")] [SerializeField]
        protected int MaximumControllers = 1;

        [Header("Settings")] public bool Online;
        [SerializeField] protected int BatteryCapacity;
        [SerializeField] protected int ConsumptionPerSecond;

        protected readonly List<PlayerController> Controllers = new();
        protected int BatteryCharge { get; private set; }

        protected bool IsFull
        {
            get
            {
                if (Controllers.Count > MaximumControllers) throw new Exception($"Error: Module {name} registered too much players !");
                return Controllers.Count == MaximumControllers;
            }
        }
        protected bool IsOperated => Controllers.Count > 0;

        protected virtual void Awake()
        {
            Online = true;
            BatteryCharge = BatteryCapacity;
        }

        private void Update()
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

        public void EnterModule(PlayerController newController)
        {
            if (IsFull || !Online)
            {
                Debug.Log($"Can't enter in module {name}...");
                return;
            }

            Controllers.Add(newController);
            newController.IsBusy = true;
        }

        public virtual void ExitModule(PlayerController currentController)
        {
            if (!Controllers.Contains(currentController))
                throw new Exception($"Error: Player #{currentController.ID} tries to ExitModule() he wasn't controlling.");

            Controllers.Remove(currentController);
            currentController.IsBusy = false;
        }

        public abstract void Operate();

        public virtual void Interact() {}

        public virtual void Aim() {}
    }
}
