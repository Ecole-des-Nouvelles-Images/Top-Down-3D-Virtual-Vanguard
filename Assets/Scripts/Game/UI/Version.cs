using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class Version : MonoBehaviour
    {
        public TMP_Text Text;
        
        void Start()
        {
            Text.text = "v" + Application.version;
        }
    }
}
