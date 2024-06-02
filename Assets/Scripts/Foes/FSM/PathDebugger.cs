using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Foes.FSM {
    public class PathDebugger : MonoBehaviour {
        
        public bool debugPath;
        public NavMeshAgent navMeshAgent;

        private void OnDrawGizmos() {
            float pathWidth = 0.5f;

            if (navMeshAgent.hasPath && debugPath) {
                NavMeshPath path = navMeshAgent.path;

                for (int i = 0; i < path.corners.Length - 1; i++) {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(path.corners[i] + Vector3.up * pathWidth, path.corners[i + 1]);
                    // Gizmos.DrawLine(path.corners[i] - Vector3.up * pathWidth, path.corners[i + 1] - Vector3.up * pathWidth);
                }
            }
        }

    }
}