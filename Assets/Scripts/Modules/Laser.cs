using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Modules
{
    public class Laser: Module
    {
        public int EnergyConsumptionPerSecond;
        public int DamageOverTime;

        [Header("References")]
        public Transform Body;
        public Transform FirePoint;
        public LineRenderer LaserBeam;
        
        [Header("Settings")]
        public float Torque = 10f;
        public float LaserRange;
        public float TickRate;

        private Vector2 _laserTarget;

        public override float CurrentBattery
        {
            get => _currentBattery;
            set {
                _currentBattery = value;
                _currentBattery = Mathf.Clamp(_currentBattery, 0, BatteryCapacity);
                UpdateBattery();
            }
        }

        private float CurrentAngle { get; set; }
        private float TargetAngle { get; set; }

        private float _currentBattery;
        private float _timer = 0f;

        #region Debug

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            
            Vector3 labelPosition = transform.position + Vector3.up * 3f;
            Handles.Label(labelPosition, $"({CurrentAngle:F3} rad.)");
        }

        #endregion

        public void Update()
        {
            CurrentAngle = Mathf.LerpAngle(CurrentAngle, TargetAngle, Torque * Time.deltaTime);
            Body.eulerAngles = new Vector3(0, CurrentAngle, 0);

            if (!LaserBeam.enabled) return;
            
            if (_timer >= TickRate)
            {
                if (CurrentBattery > 0)
                    CurrentBattery -= EnergyConsumptionPerSecond * TickRate;
                else
                    LaserBeam.enabled = false;
                
                _timer = 0;
            }

            _timer += Time.deltaTime;
        }

        #region Actions
        
        public void Rotate(InputValue input)
        {
            Vector2 value = input.Get<Vector2>();

            if (value != Vector2.zero)
                TargetAngle = Mathf.Atan2(value.x, value.y) * Mathf.Rad2Deg;
        }

        public void Fire()
        {
            if (CurrentBattery <= 0) {
                LaserBeam.enabled = false;
                return;
            }
            
            if (!LaserBeam.enabled) {
                LaserBeam.enabled = true;
                // Damage, Battery, Raycasting
            }
            else {
                LaserBeam.enabled = false;
            }
        }

        #endregion
    }
}
