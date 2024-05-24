using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

using Convoy;
using Foes.FSM.States;

namespace Foes.FSM
{
    public class Xenolith: MonoBehaviour
    {
        [Header("Gameplay")]
        public XenoType XenoType;
        public AttackBehavior Behavior;
        public int MaxHealth = 100;
        public float MoveSpeed = 10;
        public float AttackSpeed = 3.5f;
        public float AttackDamage = 10f;
        
        [Header("Navigation")]
        public NavMeshAgent Agent;
        public NavMeshObstacle Obstacle;

        [Header("Detection")]
        public float RaycastSweepSpeed = 2f;

        private XenolithBaseState _currentState;
        
        #region States

        public readonly XenolithTargetConvoyState TargetConvoyState = new ();
        public readonly XenolithTargetDroneState TargetDroneState = new ();
        public readonly XenolithEatCrystalState EatCrystalState = new ();
        public readonly XenolithAttackState AttackState = new ();

        #endregion
        
        public ConvoyManager Target { get; private set; }
        public Collider TargetCollider { get; private set; }
        public XenoType Type => XenoType;
        public int CurrentHealth
        {
            get => _currentHealth;
            set => _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
        }
        public Vector3 Height => transform.up * Agent.height;
        
        private int _currentHealth;
        private float _accumulatedDamages;
        
        #region Debug

        [Header("Debug options")]
        public bool DebugRaycasts;

        private void OnDrawGizmos()
        {
            float pathWidth = 0.5f;
            
            if (Agent.hasPath)
            {
                NavMeshPath path = Agent.path;

                for (int i = 0; i < path.corners.Length - 1; i++)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(path.corners[i] + Vector3.up * pathWidth, path.corners[i + 1] + Vector3.up * pathWidth);
                    Gizmos.DrawLine(path.corners[i] - Vector3.up * pathWidth, path.corners[i + 1] - Vector3.up * pathWidth);
                }
            }
            
            Handles.Label(transform.position + Height * 2, $"HP: {CurrentHealth}/{MaxHealth}");
        }

        #endregion
        
        private void Start()
        {
            Target = FindObjectOfType<ConvoyManager>();
            TargetCollider = Target.GetComponent<Collider>();
            Agent.speed = MoveSpeed * Time.deltaTime;
            CurrentHealth = MaxHealth;

            switch (Behavior)
            {
                case AttackBehavior.ConvoyOnly:
                    _currentState = TargetConvoyState;
                    break;
                case AttackBehavior.ClosestTarget:
                    _currentState = TargetConvoyState;
                    break;
                case AttackBehavior.EatCrystal:
                    throw new NotImplementedException("Xenolith: \"EatCrystal\" behavior not available");
                default:
                    throw new ArgumentOutOfRangeException($"Xenolith: Unexpected {Behavior.ToString()} behavior");
            }
            
            _currentState.EnterState(this);
        }

        private void Update()
        {
            _currentState.UpdateState(this);
        }

        public void SwitchState(XenolithBaseState state)
        {
            _currentState = state;
            _currentState.EnterState(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            _currentState.OnTriggerEnter(this, other);
        }
 
        #region Reactions

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

        #endregion
    }
}
