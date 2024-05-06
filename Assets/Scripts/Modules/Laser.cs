using System;
using Player;
using UnityEngine;

namespace Modules
{
    public class Laser: Module
    {
        [Header("Laser-specific")]
        [SerializeField] private int _damagePerSecond;

        private LineRenderer _beam;
        
        private bool _firing;

        protected override void Awake()
        {
            base.Awake();
            _firing = false;
        }

        private void Update()
        {
            if (BatteryCharge < 0) {
                _firing = false;
                return;
            }

            if (_firing) {
                Debug.Log($"{name} >>> Firing");
            }
        }

        public override void Operate()
        {
            _firing = !_firing;
        }

        public override void ExitModule(PlayerController currentController)
        {
            _firing = false;
            
            base.ExitModule(currentController);
        }
    }
}
