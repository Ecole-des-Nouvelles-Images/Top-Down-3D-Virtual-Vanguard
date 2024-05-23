using UnityEngine;

using Gameplay;

namespace Foes
{
    public class XenolithSpawner: MonoBehaviour
    {
        public Side SideTag = Side.None;

        public Vector3 Position => transform.position;
        
        #region Debug

        private void OnValidate()
        {
            SideTag &= Side.Centered;
            
            if ((SideTag & Side.Centered) == Side.Centered)
            {
                Debug.LogWarning($"{name}: Spawner can't be 'Centered', falling back to 'None'.");
                SideTag = Side.None;
            }
        }

        #endregion
    }
}
