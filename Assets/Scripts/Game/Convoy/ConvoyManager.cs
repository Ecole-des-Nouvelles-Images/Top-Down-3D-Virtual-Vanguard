using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Convoy
{
    public class ConvoyManager : MonoBehaviour
    {
        public static List<Module> Modules;

        [Header("Settings")]
        [SerializeField] private int _maximumDurability;

        [Header("UI")]
        public Slider Gauge;
        
        public float MaxDurability { get; set; }
        public float Durability
        {
            get => _durability;
            set => _durability = Mathf.Clamp(value, 0, _maximumDurability);
        }

        public bool Operational => Durability > 0;

        private float _durability;
        
        private void Awake()
        {
            MaxDurability = _maximumDurability;
            Durability = MaxDurability;
            Modules = new List<Module>(GetComponentsInChildren<Module>(true));
            UpdateUI();
        }

        public void UpdateUI()
        {
            Gauge.maxValue = MaxDurability;
            Gauge.value = Durability;
        }
    }
}
