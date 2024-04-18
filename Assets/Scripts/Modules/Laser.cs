using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Modules
{
    public class Laser: Module
    {
        public int EnergyConsumptionPerSecond;
        public int DamagePerSecond;

        [Header("References")]
        public Transform Body;
        public Transform FirePoint;
        public LineRenderer LaserBeam;
        
        [Header("Settings")]
        public float Torque = 10f;
        public float LaserRange;
        public float TickRate;
        public int LineRendererSegmentPrecision;

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

        private void OnValidate()
        {
            if (LineRendererSegmentPrecision <= 5)
            {
                LineRendererSegmentPrecision = 5;
            }
        }

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
            
            if (CurrentBattery <= 0) LaserBeam.enabled = false;
            
            if (!LaserBeam.enabled) return;
            
            if (_timer >= TickRate)
            {
                CurrentBattery -= EnergyConsumptionPerSecond * TickRate;
                HandleTarget();
                
                _timer = 0f;
            }

            _timer += Time.deltaTime;
        }

        private void HandleTarget()
        {
            Ray laserRay = new Ray(FirePoint.position, Body.forward);

            if (Physics.Raycast(laserRay, out RaycastHit hit, LaserRange))
            {
                float hitDistance = hit.distance;
                Alien targetHit = hit.collider.gameObject.GetComponent<Alien>();

                LaserBeam.SetPositions(CalculateLineVertices(hitDistance));
                
                if (targetHit)
                    targetHit.CurrentHealth -= DamagePerSecond * TickRate;
            }
            else {
                LaserBeam.SetPositions(CalculateLineVertices(LaserRange));
            }
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

            LaserBeam.enabled = !LaserBeam.enabled;
        }

        #endregion

        #region Utils

        private Vector3[] CalculateLineVertices(float distance)
        {
            Vector3[] linePoints = new Vector3[Mathf.CeilToInt(distance / LineRendererSegmentPrecision) + 1];
            for (int z = 0, i = 0; z <= distance; z += LineRendererSegmentPrecision, i++) {
                z = Mathf.Clamp(z, 0, Mathf.FloorToInt(distance));
                linePoints[i] = new Vector3(0, 0, z);
            }

            LaserBeam.positionCount = linePoints.GetLength(0);
            return linePoints;
        }

        #endregion
    }
}
