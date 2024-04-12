using System;
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

        private float CurrentAngle { get; set; }
        private float TargetAngle { get; set; }

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
            if (!LaserBeam.enabled)
            {
                LaserBeam.enabled = true;
                
            }
        }

        #endregion
    }
}
