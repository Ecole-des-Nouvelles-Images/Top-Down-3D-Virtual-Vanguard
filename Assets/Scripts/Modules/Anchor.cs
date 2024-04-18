using UnityEngine;

namespace Modules
{
    public class Anchor: MonoBehaviour
    {
        [HideInInspector] public Vector3 Position;
        [HideInInspector] public Drone Occupant;

        public bool IsTaken => Occupant != null;

        #region Debug

        void OnDrawGizmos()
        {
            Vector3 position = transform.position;
            float size = 0.09f;

            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(position, size);
        }

        #endregion

        private void Awake()
        {
            Position = this.transform.position;
        }
    }
}
