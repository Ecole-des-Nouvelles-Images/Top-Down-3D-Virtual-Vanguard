using UnityEngine;
using UnityEngine.AI;

namespace Foes
{
    public class Xenoliths : MonoBehaviour
    {
        [Header("Gameplay")] [SerializeField] private XenoType _type;
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private float _moveSpeed = 10;

        [Header("Navigation")] public NavMeshAgent Agent;
        public Collider TargetCollider;
        public float DestinationRandomTargetRadius = 0.5f;

        public int CurrentHealth
        {
            get => _currentHealth;
            set => _currentHealth = Mathf.Clamp(value, 0, _maxHealth);
        }
        
        private int _currentHealth;

        /*
        private void Start()
        {
            Agent.updatePosition = true;
            Agent.updateRotation = true;
            Agent.acceleration = _moveSpeed * Time.deltaTime;
            
            Vector3 targetPoint = GetRandomPointOnCollider(TargetCollider, DestinationRandomTargetRadius);
            Debug.Log($"Target: {targetPoint}");
            Agent.SetDestination(targetPoint);
        }

        private Vector3 GetRandomPointOnCollider(Collider target, float radius)
        {
            // Calculate a random direction from the center of the collider
            Vector3 randomDirection = new(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            // Calculate a random point on the surface of the collider
            Vector3 randomPoint = target.transform.position + randomDirection * radius;
            // Ensure the point is within the NavMesh
            NavMeshHit hit;
            
            if (NavMesh.SamplePosition(randomPoint, out hit, 0.1f, NavMesh.AllAreas))
                return hit.position;
            else
                return null;
        } */
    }
}
