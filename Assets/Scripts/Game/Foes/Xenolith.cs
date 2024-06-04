using System.Collections.Generic;
using System.Linq;
using Game.Convoy.Drones;
using Game.Foes.FSM.States;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Foes.FSM {
    public class Xenolith: MonoBehaviour {
        
        [Header("Gameplay")]
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private float _moveSpeed = 10;
        [SerializeField] private float _attackSpeed = 3.5f;
        [SerializeField] public int AttackDamage = 10;
        [SerializeField] private float _innerReachDistance = 4;
        [SerializeField] private float _outerReachDistance = 10;
        [field: SerializeField] public GameObject Target { get; set; }
        
        [Header("Components")]
        public NavMeshAgent Agent;
        public Animator Animator;

        private int _currentHealth;
        private float _accumulatedDamages;
        private FiniteStateMachine _finiteStateMachine;
        
        public int CurrentHealth {
            get => _currentHealth;
            set => _currentHealth = Mathf.Clamp(value, 0, _maxHealth);
        }
        public bool TargetInnerReach
        {
            get {
                if (!Target) return false;
                return Vector3.Distance(Target.transform.position, transform.position) <= _innerReachDistance;
            }
        }
        public bool TargetOuterReach
        {
            get {
                if (!Target) return false;
                return Vector3.Distance(Target.transform.position, transform.position) <= _outerReachDistance;
            }
        }

        public bool AttackReady => _internalTimer >= _attackSpeed;

        private float _internalTimer = 0f;

        private void Awake() {
            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            Agent.speed = _moveSpeed;
        }

        private void Start()
        {
            _finiteStateMachine = new FiniteStateMachine(this);
            _finiteStateMachine.ChangeState(new SelectTarget(this));
            
            //TODO ATTENTION TEST Gilbert Target = FindObjectOfType<ConvoyManager>().gameObject;
            CurrentHealth = _maxHealth;
            
            GameManager.Instance.OnStartTransit += OnStartTransit;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnStartTransit -= OnStartTransit;
        }

        private void Update()
        {
            _finiteStateMachine.Update();
            _internalTimer += Time.deltaTime;
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

        public IDamageable FetchRandomDamageable()
        {
            List<IDamageable> targetableDamageable = new List<IDamageable>();
            
            // Fetch all damageables
            foreach (IDamageable damageable in FindObjectsOfType<MonoBehaviour>().OfType<IDamageable>()) {
                if (!damageable.IsTargetable) continue;
                targetableDamageable.Add(damageable);
            }

            return targetableDamageable[Random.Range(0, targetableDamageable.Count)];
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

        public void ResetAttackTimer()
        {
            _internalTimer = 0f;
        }
        
        private void OnStartTransit()
        {
            Agent.enabled = false;
            _finiteStateMachine.IsActive = false;
        }
    }
}
