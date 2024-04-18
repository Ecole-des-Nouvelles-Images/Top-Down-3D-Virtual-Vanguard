using System;
using Player;
using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Modules
{
    public class Drone: MonoBehaviour
    {
        public MeshRenderer Renderer { get; set; }
        public CharacterController Controller { get; set; }
        public PlayerInfo User { get; set; }
        public Anchor Anchor { get; set; }
        public bool IsRegistered { get; set; }
        public bool IsDocked => transform.position == Anchor.Position;
        
        private Vector3 _direction;
        
        #region Debug

        private Vector2 _value;

        private void OnDrawGizmosSelected()
        {
            Vector3 labelPosition = transform.position + Vector3.up * 1.5f;
            Handles.Label(labelPosition, $"({_value.x:F3}, {_value.y:F3})");
        }

        #endregion

        private void Awake()
        {
            Renderer = GetComponent<MeshRenderer>();
            Controller = GetComponent<CharacterController>();
            IsRegistered = false;
        }

        private void Update()
        {
            if (_direction == Vector3.zero) return;
            
            Controller.Move(_direction);
        }

        private void OnDestroy()
        {
            DroneController controller = FindObjectOfType(typeof(DroneController)) as DroneController;
            controller?.Drones.Remove(this);
        }

        public void UpdateDirection(Vector2 value)
        {
            _value = value;
            _direction = new Vector3(_value.x, 0, _value.y) * DroneController.GlobalDroneSpeed  * Time.deltaTime;
        }

        public void RegisterPlayer(PlayerInfo playerData)
        {
            IsRegistered = true;
            User = playerData;
            Renderer.material = User.Material;
        }

        public void ReturnToAnchor()
        {
            transform.position = Anchor.Position;
        }
    }
}
