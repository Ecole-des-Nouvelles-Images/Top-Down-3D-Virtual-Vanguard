using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static int Crystals { get; set; }

        #region Debug

        void OnGUI()
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 36;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            Rect rect = new Rect(10, 10, 200, 100);
            GUI.Box(rect, "Crystals: " + Crystals, style);
        }

        #endregion
        
        public void TransitPhase()
        {
        }

        public void StopPhase()
        {
        }
    }
}
