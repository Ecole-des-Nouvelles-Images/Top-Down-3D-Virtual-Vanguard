using System;
using UnityEditor;
using UnityEngine;

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
                _targetModuleID =  Mathf.Clamp(value, 0, Convoy.Modules.Count - 1);
                
                if (_targetModuleID < 0 || _targetModuleID > Convoy.Modules.Count)
                    throw new IndexOutOfRangeException("Error: Trying to access invalid module > TargetModuleID out-of-bounds");
                
                _targetModule = Convoy.Modules[_targetModuleID];
            }
        }

        private Module _targetModule;
        private int _targetModuleID;

        #region Debug

        protected override void OnDrawGizmos()
        {
            string text;

            if (_targetModule == null)
                text = "Target: none";
            else
                text = "Target: {" + (TargetModuleID == 0 && _targetModule == this ? "Undefined" : TargetModuleID.ToString()) + "} " + (_targetModule == this ? "Self" : _targetModule.name);
            
            Handles.Label(transform.position + (Vector3.down *2) + (Vector3.left * 2), text);
        }

        #endregion

        private void Start()
        {
            base.Awake();
            TargetModuleID = Convoy.Modules.FindIndex(module => module.GetType() == this.GetType());
        }

        private void FixedUpdate()
        {
            if (Online && _targetModule != null && _targetModule.BatteryMaxCapacity > 0 && _targetModule.BatteryCharge < _targetModule.BatteryMaxCapacity)
            {
                _targetModule.BatteryCharge += PowerOutput * Time.fixedDeltaTime;
                _targetModule.BatteryCharge = Mathf.Clamp(_targetModule.BatteryCharge, 0, _targetModule.BatteryMaxCapacity);
            }
        }

        public override void Operate()
        {
            if (!_ignoreInputInteraction)
            {
                if (!Online) return;

                TargetModuleID++;
                _ignoreInputInteraction = true;
            }
            else 
            {
                _ignoreInputInteraction = false;
            }
        }

        public override void Interact()
        {
            if (!Online) return;

            TargetModuleID--;
        }
    }
}
