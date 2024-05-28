using Terrain;
using UnityEngine;

namespace Convoy
{
    public class ConvoyTransit : MonoBehaviour
    {
        public float Speed;

        public Vector3 ConvoyDirection => transform.TransformDirection(-transform.forward);
        public TransitManager Manager;

        private void Start()
        {
            Manager = FindAnyObjectByType<TransitManager>();
        }

        private void Update()
        {
            transform.Translate(ConvoyDirection* (Speed * Time.deltaTime));
        }
    }
}
