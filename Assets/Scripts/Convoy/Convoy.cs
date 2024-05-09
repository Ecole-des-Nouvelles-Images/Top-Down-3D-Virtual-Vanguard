using System.Collections.Generic;
using UnityEngine;

namespace Convoy
{
    public class Convoy: MonoBehaviour
    {
        public static List<Module> Modules;

        private void Awake()
        {
            Modules = new (GetComponentsInChildren<Module>(true));
        }
    }
}
