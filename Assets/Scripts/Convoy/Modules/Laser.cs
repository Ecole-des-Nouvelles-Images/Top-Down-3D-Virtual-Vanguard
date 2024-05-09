using UnityEngine;
using UnityEngine.InputSystem;
using Player;

namespace Convoy.Modules
{
    public class Laser: Module
    {
        [Header("Laser specifics")]
        [SerializeField] private Transform _turret;
        [SerializeField] private Transform _canon;
        [SerializeField] private float _torque;
        [SerializeField] private int _range;
        [SerializeField] private int _damagePerSecond;
        
        private bool _firing;

        protected override void Awake()
        {
            base.Awake();
            _firing = false;
        }

        protected override void Update()
        {
            if (!Online) return;
            
            if (BatteryCharge <= 0) {
                _firing = false;
                return;
            }

            if (_firing) {
                DrawLaser();
            }
        }

        #region Actions

        public override void Operate()
        {
            _firing = !_firing;
        }

        public override void Aim(InputValue input)
        {
            Vector2 value = input.Get<Vector2>();
            
            _turret.LookAt(_turret.position + new Vector3(value.x,0, value.y));
        }

        public override void ExitModule(PlayerController currentController)
        {
            _firing = false;
            
            base.ExitModule(currentController);
        }

        #endregion

        #region Debug

        private void DrawLaser()
        {
            if (_range <= 0) {
                Debug.LogWarning("Range must be greater than 0");
                return;
            }
            
            Vector3 endPoint = _canon.position + _turret.forward * _range;
            Debug.DrawLine(_canon.position, endPoint, Color.red, Time.deltaTime);
        }

        #endregion
    }
}
