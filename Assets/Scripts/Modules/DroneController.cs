using UnityEditor;
using UnityEngine;

namespace Modules
{
    public class DroneController: Module
    {
        [Header("References")]
        public CharacterController DroneJ1;
        public CharacterController DroneJ2;
        public CharacterController DroneJ3;
        public CharacterController DroneJ4;

        [Header("Gameplay")]
        public float DroneSpeed;
        public float RespawnDelay;
        
        public override float CurrentBattery { get; set; }
        
        private Vector3 _initialPosJ1;
        private Vector3 _initialPosJ2;
        private Vector3 _initialPosJ3;
        private Vector3 _initialPosJ4;

        private Vector3 _direction;

        #region Debug

        private Vector2 _value;

        private void OnDrawGizmosSelected()
        {
            Vector3 labelPosition = transform.position + Vector3.up * 1.5f;
            Handles.Label(labelPosition, $"({_value.x:F3}, {_value.y:F3})");
        }

        #endregion
        
        protected override void Start()
        {
            _initialPosJ1 = DroneJ1.transform.position;
            _initialPosJ2 = DroneJ2.transform.position;
            _initialPosJ3 = DroneJ3.transform.position;
            _initialPosJ4 = DroneJ4.transform.position;
        }

        private void Update()
        {
            DroneJ1.Move(_direction);
        }

        public void UpdateMove(Vector2 value)
        {
            _value = value;
            _direction = _value * DroneSpeed  * Time.deltaTime;
        }

        public void ResetDrone()
        {
            Debug.Log("Reset Drones");
            DroneJ1.transform.position = _initialPosJ1;
            DroneJ2.transform.position = _initialPosJ2;
            DroneJ3.transform.position = _initialPosJ3;
            DroneJ4.transform.position = _initialPosJ4;
        }
    }
}
