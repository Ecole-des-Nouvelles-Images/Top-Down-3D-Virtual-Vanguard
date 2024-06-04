﻿using System;
using System.Collections.Generic;
using System.Linq;
using Game.Convoy.Drones;
using Game.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Convoy
{
    public abstract class Module : MonoBehaviour, IDamageable
    {
        [Header("Player Management")] [SerializeField]
        protected int MaximumControllers = 1;

        [Header("Settings")] public bool Online;
        [SerializeField] protected int BatteryCapacity;
        [SerializeField] protected int ConsumptionPerSecond;

        [Header("Interface")]
        [SerializeField] protected Slider BatteryGauge;
        [SerializeField] protected Image ChargeStatus;
        [SerializeField] protected Image ModuleIcon;

        public string Type => GetType().ToString();
        public int BatteryMaxCapacity => BatteryCapacity;
        public float BatteryCharge { get; set; }

        protected ConvoyManager Convoy;

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
        
        protected virtual void Awake()
        {
            Convoy = FindAnyObjectByType<ConvoyManager>();
            Online = true;
            BatteryCharge = BatteryCapacity;
            UpdateInterfaceBatteryCharge();
        }

        protected virtual void Update()
        {
            if (!Online)
                Deactivate();
        }

        public virtual void Deactivate()
        {
            foreach (PlayerController player in Controllers.ToList())
            {
                ExitModule(player);
            }

            Online = false;
        }

        #region Actions

        public virtual bool EnterModule(PlayerController newController)
        {
            if (!Online || IsFull || Controllers.Contains(newController))
                return false;

            Controllers.Add(newController);
            newController.IsBusy = true;

            if (BatteryMaxCapacity > 0) {
                BatteryGauge.gameObject.SetActive(true);
                ModuleIcon.gameObject.SetActive(false);
            }

            if (MaximumControllers == 1)
            {
                BatteryGauge.transform.Find("Fill Area/Fill").GetComponent<Image>().color = newController.PlayerColor;
                ModuleIcon.color = newController.PlayerColor;
            }
            
            return true;
        }

        public virtual bool ExitModule(PlayerController currentController)
        {
            if (!Controllers.Contains(currentController))
                return false;

            Controllers.Remove(currentController);
            currentController.IsBusy = false;
            Debug.Log($"Exiting module {name}");

            if (BatteryMaxCapacity > 0)
            {
                BatteryGauge.gameObject.SetActive(false);
                ModuleIcon.gameObject.SetActive(true);
            }
            
            BatteryGauge.transform.Find("Fill Area/Fill").GetComponent<Image>().color = Color.white;
            ModuleIcon.color = Color.white;
            
            return true;
        }

        public abstract void Operate(PlayerController currentController);

        public virtual void Interact(PlayerController currentController) {}

        public virtual void Aim(InputValue input) {}

        #endregion

        #region Interface Utilities

        public void WakeInterfaceBattery(bool enable)
        {
            // ChargeStatus.color = enable ? Color.yellow : Color.black;
            
            if (BatteryMaxCapacity > 0)
            {
                BatteryGauge.gameObject.SetActive(enable);
                ModuleIcon.gameObject.SetActive(!enable);
            }
        }

        public void UpdateInterfaceBatteryCharge()
        {
            BatteryGauge.maxValue = BatteryCapacity;
            BatteryGauge.value = BatteryCharge;
        }

        #endregion
        
        #region IDamageable

        public GameObject GameObject => gameObject;
        public Transform Transform => transform;
        public bool IsTargetable => Online;

        public void TakeDamage(int damage)
        {
            Convoy.Durability -= damage;
            
        }
        
        #endregion
    }
}
