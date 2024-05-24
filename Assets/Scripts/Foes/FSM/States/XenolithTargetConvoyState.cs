using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Foes.FSM.States
{
    public class XenolithTargetConvoyState: XenolithBaseState
    {
        private Xenolith Xenolith { get; set; }

        private Transform Transform => Xenolith.transform;
        private NavMeshAgent Agent => Xenolith.Agent;
        private Collider TargetCollider => Xenolith.TargetCollider;
        private Vector3 Height => Transform.up * Xenolith.Agent.height;

        public override void EnterState(Xenolith xenolith)
        {
            Xenolith = xenolith;
            TargetConvoy();
        }

        public override void UpdateState(Xenolith xenolith)
        {
            Collider[] colliders = new Collider[1];

            if (Physics.OverlapSphereNonAlloc(xenolith.transform.position, xenolith.DetectionRadius, colliders, 1 << LayerMask.NameToLayer("Drones")) == 1)
            {
                xenolith.Target = colliders[0].gameObject;
                xenolith.SwitchState(xenolith.TargetDroneState);
            }
        }

        public override void OnTriggerEnter(Xenolith xenolith, Collider other)
        {
            if (!other.CompareTag("Convoy")) return;

            xenolith.Target = other.gameObject;
            xenolith.SwitchState(xenolith.AttackState);
        }

        #region Logic

        private void TargetConvoy()
        {
            if (Physics.Raycast(Transform.position + Height, Transform.forward, out RaycastHit hit, 500f))
            {
                if (hit.collider == TargetCollider)
                {
                    Agent.SetDestination(hit.point); // Set the target path to the hit point
                    return;
                }
            }
            else // Retry with higher raycast to compensate lower ground.
            {
                if (Physics.Raycast(Transform.position + Height + Vector3.up * 2f, Transform.forward, out hit, 500f))
                {
                    if (hit.collider == TargetCollider)
                    {
                        Agent.SetDestination(hit.point);
                        return;
                    }
                }
            }

            Xenolith.StartCoroutine(Try360Raycast());
        }

        private IEnumerator Try360Raycast()
        {
            float t = 0f;
            float startingAngle = Transform.eulerAngles.y;
            float directionAngle = Transform.position.x >= 0 ? startingAngle + 360f : startingAngle - 360f;

            while (t < 1) // Try a last 360° turn to find the convoy
            {
                if (Physics.Raycast(Transform.position + Height, Transform.forward, out RaycastHit hit, 500f)) {
                    if (hit.collider == TargetCollider)
                    {
                        Agent.SetDestination(hit.point);
                        yield break;
                    }
                }
                        
                if (Xenolith.DebugRaycasts)
                    Debug.DrawRay(Transform.position + Height, Transform.forward * 250f, Color.magenta, 3f);
                        
                t += Time.deltaTime / Xenolith.RaycastSweepSpeed;
                Transform.eulerAngles = Vector3.Lerp(Transform.eulerAngles, new Vector3(0, startingAngle + directionAngle, 0), t);
                yield return null;
            }
            
            if (Agent.hasPath)
            {
                Debug.LogWarning($"{Xenolith.name} didn't find the convoy !");
            }
        }

        #endregion
    }
}
