using System;
using System.Collections;
using Convoy;
using Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Foes
{
    public class Xenolith : MonoBehaviour
    {
        [Header("Gameplay")]
        [SerializeField] private XenoType _type;
        [SerializeField] private AttackBehavior _behavior;
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private float _moveSpeed = 10;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _attackSpeed = 3.5f;
        [SerializeField] private float _attackDamage = 10f;
        
        [Header("Navigation")]
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private NavMeshObstacle _obstacle;
        [SerializeField] private Collider _targetCollider;
        [SerializeField] private LayerMask _targetsMasks;
        [SerializeField] private float _raycastSweepSpeed = 2f;

        public int CurrentHealth
        {
            get => _currentHealth;
            set => _currentHealth = Mathf.Clamp(value, 0, _maxHealth);
        }

        private Rigidbody _rigidbody;
        
        private ConvoyManager _target;
        private Vector3 _targetPath;
        private int _currentHealth;
        private float _accumulatedDamages;
        
        private Vector3 Height => transform.up * _agent.height;

        #region Debug

        [Header("Debug options")] public bool DebugRaycasts;

        private void OnDrawGizmos()
        {
            float pathWidth = 0.5f;
            
            if (_agent.hasPath)
            {
                // Calculer le chemin entre la position actuelle de l'agent et sa destination
                NavMeshPath path = _agent.path;

                // Dessiner les lignes du chemin à l'aide de Gizmos.DrawLine
                for (int i = 0; i < path.corners.Length - 1; i++)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(path.corners[i] + Vector3.up * pathWidth, path.corners[i + 1] + Vector3.up * pathWidth);
                    Gizmos.DrawLine(path.corners[i] - Vector3.up * pathWidth, path.corners[i + 1] - Vector3.up * pathWidth);
                }
            }
            
            Handles.Label(transform.position + Height * 2, $"HP: {CurrentHealth}/{_maxHealth}");
        }

        private void OnDrawGizmosSelected()
        {
            if (!XenoManager.Instance.DebugUnitsForwardDirection) return;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + Height, transform.forward * 150f);
            Gizmos.color = Color.white;
        }

        #endregion
        
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            CurrentHealth = _maxHealth;
            _agent.speed = _moveSpeed * Time.deltaTime;

            switch (_behavior)
            {
                case AttackBehavior.ConvoyOnly:
                    StartCoroutine(TargetConvoy());
                    break;
                default:
                    Debug.LogWarning($"Xenolith can't handle \"{_behavior.ToString()}\" behavior yet.");
                    return;
            }
            
            _agent.updatePosition = true;
            _agent.updateRotation = true;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Convoy")) return;
            
            StopAgent();
            _target = other.GetComponent<ConvoyManager>();
            StartCoroutine(AttackConvoy());
        }

        #region Logic

        private IEnumerator TargetConvoy()
        {
            float t = 0f;
            float startingAngle = transform.eulerAngles.y;
            float targetAngle = startingAngle;
            float directionAngle = transform.position.x >= 0 ? 360f : -360f;
            
            // Raycast forward to check if the AI can see the target
            RaycastHit hit;
                
            if (Physics.Raycast(transform.position + Height, transform.forward, out hit, 500f, _targetsMasks))
            {
                if (hit.collider == _targetCollider)
                {
                    _agent.SetDestination(hit.point); // Set the target path to the hit point
                    yield break;
                }
            }
            else // Retry with higher raycast to compensate lower ground.
            {
                if (Physics.Raycast(transform.position + Height + Vector3.up * 2f, transform.forward, out hit, 500f, _targetsMasks))
                {
                    if (hit.collider == _targetCollider)
                    {
                        _agent.SetDestination(hit.point);
                        yield break;
                    }
                }
            }

            while (t < 1) // Try a last 360° turn to find the convoy
            {
                if (Physics.Raycast(transform.position + Height + Vector3.up * 2f, transform.forward, out hit, 500f, _targetsMasks)) {
                    if (hit.collider == _targetCollider)
                    {
                        _agent.SetDestination(hit.point);
                        yield break;
                    }
                }
                    
                if (DebugRaycasts)
                    Debug.DrawRay(transform.position + Height + Vector3.up * 2f, transform.forward * 250f, Color.magenta, 20f);
                    
                t += Time.deltaTime / _raycastSweepSpeed;
                transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, startingAngle + directionAngle, 0), t);
                yield return null;
            }
            
            if (_agent.hasPath)
            {
                Debug.LogWarning($"{name} didn't find the convoy !");
            }
        }

        private IEnumerator AttackConvoy()
        {
            float internalTimer = 0f;
            
            while (this.enabled)
            {
                if (internalTimer >= _attackSpeed)
                {
                    // Trigger attack animation
                    _target.TakeDamage(_attackDamage);
                    internalTimer = 0f;
                    yield return null;
                }

                internalTimer += Time.deltaTime;
                yield return null;
            }
        }

        #endregion

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

        public void StopAgent()
        {
            _agent.isStopped = true;
            _agent.enabled = false;
            _obstacle.enabled = true;
            // TerrainManager.Instance.UpdateNavMesh();
        }

        #endregion
    }
}
