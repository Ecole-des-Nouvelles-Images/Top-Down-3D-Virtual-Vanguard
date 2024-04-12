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

        private static float CurrentAngle { get; set; }
        private static float TargetAngle { get; set; }

        #region Debug

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            
            Vector3 labelPosition = transform.position + Vector3.up * 3f;
            Handles.Label(labelPosition, $"({CurrentAngle:F3} rad.)");
        }

        #endregion

        private void Awake()
        {
            LaserBeam.SetPosition(0, FirePoint.position);
        }

        public void Update()
        {
            CurrentAngle = Mathf.LerpAngle(CurrentAngle, TargetAngle, Torque * Time.deltaTime);
            Body.eulerAngles = new Vector3(0, CurrentAngle, 0);
        }

        #region Actions
        
        public static void Rotate(InputValue input)
        {
            Vector2 value = input.Get<Vector2>();

            if (value != Vector2.zero)
                TargetAngle = Mathf.Atan2(value.x, value.y) * Mathf.Rad2Deg;
        }

        public static void Fire()
        {
            
        }

        #endregion
    }
}
