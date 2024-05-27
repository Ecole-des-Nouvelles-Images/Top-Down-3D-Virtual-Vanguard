using UnityEngine;

namespace Convoy
{
    public class ConvoyTransit : MonoBehaviour
    {
        public float Speed;

        public Vector3 ConvoyDirection => transform.TransformDirection(-transform.forward);

        private void Update()
        {
            transform.Translate(ConvoyDirection* (Speed * Time.deltaTime));
        }
        
        
    }
}
