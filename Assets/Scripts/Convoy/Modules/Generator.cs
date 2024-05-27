using System;
using Player;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

namespace Convoy.Modules
{
    public class Generator: Module
    {
        [Header("Generator specifics")]
        [SerializeField] public int PowerOutput;
        
        // OnModuleOperate action triggers 2 times: prevents "cancelled" call instead of touching the input.
        private bool _ignoreInputInteraction;
        
        private int TargetModuleID
        {
            get => _targetModuleID;
            set
            {
                _targetModuleID =  Mathf.Clamp(value, 0, ConvoyManager.Modules.Count - 1);
                
                if (_targetModuleID < 0 || _targetModuleID > ConvoyManager.Modules.Count)
                    throw new IndexOutOfRangeException("Error: Trying to access invalid module > TargetModuleID out-of-bounds");
                
                _targetModule = ConvoyManager.Modules[_targetModuleID];
            }
        }

        private Module _targetModule;
        private int _targetModuleID;

        private void Start()
        {
            base.Awake();
            TargetModuleID = ConvoyManager.Modules.FindIndex(module => module.GetType() == this.GetType());
            _targetModule.WakeInterfaceBattery(true);
        }

        private void FixedUpdate()
        {
            if (Online && _targetModule && _targetModule.BatteryMaxCapacity > 0 && _targetModule.BatteryCharge < _targetModule.BatteryMaxCapacity)
            {
                _targetModule.BatteryCharge += PowerOutput * Time.fixedDeltaTime;
                _targetModule.BatteryCharge = Mathf.Clamp(_targetModule.BatteryCharge, 0, _targetModule.BatteryMaxCapacity);
                _targetModule.UpdateInterfaceBatteryCharge();
            }
        }

        public override void Operate(PlayerController currentController)
        {
            if (!_ignoreInputInteraction)
            {
                if (!Online) return;

                _targetModule.WakeInterfaceBattery(false);
                TargetModuleID--;
                _targetModule.WakeInterfaceBattery(true);
                _ignoreInputInteraction = true;
            }
            else 
            {
                _ignoreInputInteraction = false;
            }
        }

        public override void Interact(PlayerController currentController)
        {
            if (!Online) return;

            _targetModule.WakeInterfaceBattery(false);
            TargetModuleID++;
            _targetModule.WakeInterfaceBattery(true);
        }
    }
}
