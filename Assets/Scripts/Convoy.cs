using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Modules;

public class Convoy: MonoBehaviour
{
    public GameObject ModuleOrigin;
        
    public static List<Module> Modules = new();

    private void Start()
    {
        Modules = GetModulesInOrder(ModuleOrigin);
        
        if (Modules.Count == 0) Debug.LogError("Convoy's module list is empty");
    }

    #region Utils

    private List<Module> GetModulesInOrder(GameObject parent)
    {
        List<Module> modules = new List<Module>();

        // Get all child objects in the order of the hierarchy
        List<GameObject> childObjects = GetChildsInOrder(parent);

        // Get the Module component from each child object and add it to the list
        foreach (GameObject child in childObjects)
        {
            Module module = child.GetComponent<Module>();
            if (module != null)
            {
                modules.Add(module);
            }
        }

        return modules;
    }

    private List<GameObject> GetChildsInOrder(GameObject parent)
    {
        return (from Transform child in parent.transform select child.gameObject).ToList();
    }

    #endregion
}
