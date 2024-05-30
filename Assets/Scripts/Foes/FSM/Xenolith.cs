using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Convoy;
using Convoy.Drones;
using Foes.FSM.States;

namespace Foes.FSM {
    public class Xenolith: MonoBehaviour {
        
        [Header("Gameplay")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private float moveSpeed = 10;
        [SerializeField] private float attackSpeed = 3.5f;
        [SerializeField] public int attackDamage = 10;
        [SerializeField] private float innerReachDistance = 4;
        [SerializeField] private float outerReachDistance = 10;
        [field: SerializeField] public GameObject Target { get; set; }
        
        [Header("Components")]
        public NavMeshAgent navMeshAgent;
        public Animator animator;

        private int _currentHealth;
        private float _accumulatedDamages;
        private FiniteStateMachine _finiteStateMachine;
        
        public int CurrentHealth {
            get => _currentHealth;
            set => _currentHealth = Mathf.Clamp(value, 0, maxHealth);
        }
        
        public bool AttackReady => true;

        public bool TargetInnerReach
        {
            get {
                if (!Target) return false;
                Debug.Log(Vector3.Distance(Target.transform.position, transform.position));
                return Vector3.Distance(Target.transform.position, transform.position) <= innerReachDistance;
            }
        }
        
        public bool TargetOuterReach
        {
            get {
                if (!Target) return false;
                return Vector3.Distance(Target.transform.position, transform.position) <= outerReachDistance;
            }
        }

        private void Awake() {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _finiteStateMachine = new FiniteStateMachine(this);
            _finiteStateMachine.ChangeState(new SelectTarget(this));
            
            Target = FindObjectOfType<ConvoyManager>().gameObject;
            CurrentHealth = maxHealth;
        }

        private void Update()
        {
            _finiteStateMachine.Update();
        }

        public IDamageable FetchNearestDamageable() {
            List<IDamageable> targetableDamageable = new List<IDamageable>();
            
            // Fetch all damageables
            foreach (IDamageable damageable in FindObjectsOfType<MonoBehaviour>().OfType<IDamageable>()) {
                if (!damageable.IsTargetable) continue;
                targetableDamageable.Add(damageable);
            }
            
            // Select the neareast damageable
            IDamageable nearestDamageable = null;
            float nearestDistance = Mathf.Infinity;
            foreach (IDamageable damageable in targetableDamageable) {
                float distance = Vector3.Distance(transform.position, damageable.Transform.position);
                if (distance >= nearestDistance) continue;
                nearestDistance = distance;
                nearestDamageable = damageable;
            }

            return nearestDamageable;
        }
        
        public void TakeDamage(float damages)
        {
            _accumulatedDamages += damages;

            int unitDamage = Mathf.FloorToInt(_accumulatedDamages);
            if (unitDamage > 0)
            {
                CurrentHealth -= unitDamage;
                _accumulatedDamages -= unitDamage;
            }
            
            if (CurrentHealth <= 0)
                Destroy(gameObject);
        }

    }
}
