using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerManager: MonoBehaviour
    {
        public List<Transform> PlayerOriginPositions;

        public void OnPlayerJoined(PlayerInput player)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            
            Debug.Log($"Player {controller.PlayerID + 1} joined !");
        }

        public void OnPlayerLeft(PlayerInput player)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            
            Debug.Log($"Player {controller.PlayerID} left !");
        }
    }
}
