using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Foes
{
    public class Xenoliths : MonoBehaviour
    {
        [Header("Gameplay")]
        [SerializeField] private XenoType _type;
        [SerializeField] private AttackBehavior _behavior;
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private float _moveSpeed = 10;
        [SerializeField] private float _rotationSpeed = 10f;
        
        [Header("Navigation")]
        public NavMeshAgent Agent;
        public Transform Target;
        public Collider TargetCollider;
        public LayerMask TargetsMasks;
        public float RaycastSweepSpeed = 2f;

        public int CurrentHealth
        {
            get => _currentHealth;
            set => _currentHealth = Mathf.Clamp(value, 0, _maxHealth);
        }
        
        private int _currentHealth;

        private Vector3 _targetPath;

        #region Debug

        [Header("Debug options")] public bool DebugRaycasts;

        private void OnDrawGizmos()
        {
            float pathWidth = 0.5f;
            
            if (Agent.hasPath)
            {
                // Calculer le chemin entre la position actuelle de l'agent et sa destination
                NavMeshPath path = Agent.path;

                // Dessiner les lignes du chemin à l'aide de Gizmos.DrawLine
                for (int i = 0; i < path.corners.Length - 1; i++)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(path.corners[i] + Vector3.up * pathWidth, path.corners[i + 1] + Vector3.up * pathWidth);
                    Gizmos.DrawLine(path.corners[i] - Vector3.up * pathWidth, path.corners[i + 1] - Vector3.up * pathWidth);
                }
            }
        }

        #endregion
        
        private void Start()
        {
            Agent.speed = _moveSpeed * Time.deltaTime;

            StartCoroutine(TargetConvoy());
            
            Agent.updatePosition = true;
            Agent.updateRotation = true;
        }

        private IEnumerator TargetConvoy()
        {
            float t = 0f;
            float startingAngle = transform.eulerAngles.y;
            float targetAngle = startingAngle;
            float directionAngle = transform.position.x >= 0 ? 360f : -360f;
            
            // Raycast forward to check if the AI can see the target
            RaycastHit hit;
                
            if (Physics.Raycast(transform.position, transform.forward, out hit, 500f, TargetsMasks))
            {
                if (hit.collider == TargetCollider)
                {
                    // Set the target path to the hit point
                    Agent.SetDestination(hit.point);
                    yield break;
                }
            }
            else // Retry with higher raycast to compensate lower ground.
            {
                if (Physics.Raycast(transform.position + Vector3.up * 2f, transform.forward, out hit, 500f, TargetsMasks))
                {
                    if (hit.collider == TargetCollider)
                    {
                        // Set the target path to the hit point
                        Agent.SetDestination(hit.point);
                        yield break;
                    }
                }
            }

            while (t < 1)
            {
                if (Physics.Raycast(transform.position + Vector3.up * 2f, transform.forward, out hit, 500f, TargetsMasks)) {
                    if (hit.collider == TargetCollider)
                    {
                        // Set the target path to the hit point
                        Agent.SetDestination(hit.point);
                        yield break;
                    }
                }
                    
                if (DebugRaycasts)
                    Debug.DrawRay(transform.position, transform.forward * 250f, Color.magenta, 20f);
                    
                t += Time.deltaTime / RaycastSweepSpeed;
                transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, startingAngle + directionAngle, 0), t);
                yield return null;
            }
            
            if (Agent.hasPath)
            {
                Debug.LogWarning($"{name} didn't find the convoy !");
            }
        }
    }
}
