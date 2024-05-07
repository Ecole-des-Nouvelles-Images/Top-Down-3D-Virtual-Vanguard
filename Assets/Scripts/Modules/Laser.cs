using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

using Player;

namespace Modules
{
    public class Laser: Module
    {
        [Header("Laser-specific")]
        [SerializeField] private Transform _turret;
        [SerializeField] private int _range;
        [SerializeField] private int _damagePerSecond;
        [SerializeField] private float _torque;
        
        private bool _firing;
        private float _currentAngle;
        private float _targetAngle;

        protected override void Awake()
        {
            base.Awake();
            _firing = false;
        }

        protected override void Update()
        {
            if (!Online)
                Deactivate();
            
            if (BatteryCharge < 0) {
                _firing = false;
                return;
            }

            if (_firing) {
                Debug.Log($"{name} >>> Firing");
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            
            Vector3 labelPosition = transform.position + Vector3.up * 3f;
            Handles.Label(labelPosition, $"({_targetAngle:F3} rad.)");
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
            //Debug.Log(value.x);
            //_turret.Rotate(0, value.x, 0);
            //_turret.Rotate(new Vector3(0, _currentAngle, 0));

            //_targetAngle = Mathf.Atan2(value.x, value.y) * Mathf.Rad2Deg;
        }

        public override void ExitModule(PlayerController currentController)
        {
            _firing = false;
            
            base.ExitModule(currentController);
        }

        #endregion
        
        
    }
}
