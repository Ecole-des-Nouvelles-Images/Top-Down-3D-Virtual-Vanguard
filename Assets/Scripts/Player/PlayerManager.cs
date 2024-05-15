using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerManager: MonoBehaviour
    {
        public List<Transform> PlayerOriginPositions;

        public void OnPlayerJoined(PlayerInput player) {}

        public void OnPlayerLeft(PlayerInput player) {}
    }
}
