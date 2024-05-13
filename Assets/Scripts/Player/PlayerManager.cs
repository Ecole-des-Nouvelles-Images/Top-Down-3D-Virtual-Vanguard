using System.Collections.Generic;
using Convoy;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerManager: MonoBehaviour
    {
        public List<Transform> PlayerOriginPositions;

        public void OnPlayerJoined(PlayerInput player)
        {
            Debug.Log("Inside OnPlayerJoined");
        }

        public void OnPlayerLeft(PlayerInput player)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            Debug.Log($"Player {controller.PlayerID} left !");
        }
    }
}
