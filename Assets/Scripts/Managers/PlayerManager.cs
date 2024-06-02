using System.Collections.Generic;
using Convoy.Modules;
using Internal;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class PlayerManager: SingletonMonoBehaviour<PlayerManager>
    {
        public int PlayerNumber;
        public List<Transform> PlayerOriginPositions;

        private Head _convoyHead;

        private void Start()
        {
            PlayerNumber = 0;
            _convoyHead = FindAnyObjectByType<Head>();
        }

        public void OnPlayerJoined(PlayerInput player)
        {
            PlayerNumber++;
            _convoyHead.UpdateMaximumControllers(PlayerNumber);
        }

        public void OnPlayerLeft(PlayerInput player)
        {
            PlayerNumber--;
            _convoyHead.UpdateMaximumControllers(PlayerNumber);
        }
    }
}
