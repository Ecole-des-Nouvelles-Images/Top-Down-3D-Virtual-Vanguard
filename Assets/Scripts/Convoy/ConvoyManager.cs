using System.Collections.Generic;
using Convoy.Drones;
using UnityEditor;
using UnityEngine;

namespace Convoy
{
    public class ConvoyManager : MonoBehaviour, IDamageable
    {
        public static List<Module> Modules;

        [Header("Settings")] [SerializeField] private int _maximumDurability;
        
        public bool IsTargetable => true;
        public GameObject GameObject => gameObject;
        public Transform Transform => transform;
        
        public float Durability
        {
            get => _durability;
            set => _durability = Mathf.Clamp(value, 0, _maximumDurability);
        }

        public bool Operational => Durability > 0;

        private float _durability;

        #region Debug

        private void OnDrawGizmos()
        {
            GUIStyle labelStyle = new()
            {
                fontSize = 15,
                normal =
                {
                    textColor = Color.white
                }
            };

            string durabilityText = $"Durability: {Durability:F0}/{_maximumDurability:F0}";
            Handles.Label(transform.position + Vector3.down * 3, durabilityText, labelStyle);
        }

        #endregion

        private void Awake()
        {
            Durability = _maximumDurability;
            Modules = new List<Module>(GetComponentsInChildren<Module>(true));
        }

        public void TakeDamage(int damage)
        {
            Durability -= damage;
            // Trigger damage SFX;
            // Update UI;
        }
    }
}
