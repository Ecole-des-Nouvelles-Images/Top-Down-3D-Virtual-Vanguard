using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(PlayerInputManager))]
    public class PlayerManager : MonoBehaviour
    {
        public List<GameObject> PlayerPrefabs = new(4);

        private PlayerInputManager _manager;
        private int _playerIndex = 0;

        private void Awake()
        {
            _manager = GetComponent<PlayerInputManager>();
            _manager.playerPrefab = PlayerPrefabs[0];
        }

        public void OnPlayerJoined()
        {
            _playerIndex++;
            _playerIndex = Mathf.Clamp(_playerIndex, 0, PlayerPrefabs.Count - 1);
            _manager.playerPrefab = PlayerPrefabs[_playerIndex];
        }

        public void OnPlayerLeft()
        {
            _playerIndex--;
            if (_playerIndex < 0) _playerIndex = 0;
        }
    }
}
