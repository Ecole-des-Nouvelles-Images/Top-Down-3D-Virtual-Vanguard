using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.UI
{
    public class StartPanel: MonoBehaviour
    {
        public GameObject MainMenu;
        
        private void Update()
        {
            Gamepad gamepad = Gamepad.current;

            if (gamepad == null)
            {
                Debug.LogWarning("No gamepad detected");
                return;
            }

            if (gamepad.startButton.isPressed)
                ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            MainMenu.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
