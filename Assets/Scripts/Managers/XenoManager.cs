using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using Internal;
using Gameplay;
using Foes;
using Foes.FSM;

namespace Managers
{
    public class XenoManager : SingletonMonoBehaviour<XenoManager>
    {
        [Header("References")]
        [SerializeField] private List<GameObject> _xenolithsPrefabs;
        [SerializeField] private Transform _xenolithHolder;

        [Header("Spawners")]
        [SerializeField] private List<XenolithSpawner> _spawners;
        [SerializeField] private float _spawnRate;

        private readonly Dictionary<XenoType, GameObject> _xenolithsPrefabsTypes = new();
        private List<XenolithSpawner> _filteredSpawners = new();

        private void Start()
        {
            PrepareXenolithTypes();
            FiltersSpawners(GameManager.Instance.Side);
            StartXenolithOffensive();
        }

        #region Logic

        private void StartXenolithOffensive()
        {
            StartCoroutine(LaunchSpawnCoroutine(new List<XenoType> { XenoType.Lambda }));
        }

        private IEnumerator LaunchSpawnCoroutine(List<XenoType> typePresence)
        {
            float timer = 0f;
            
            while (EditorApplication.isPlaying)
            {
                if (timer > _spawnRate)
                {
                    XenoType type = typePresence[Random.Range(0, typePresence.Count)];
                    GameObject prefab = _xenolithsPrefabsTypes[type];
                    XenolithSpawner spawner = _filteredSpawners[Random.Range(0, _filteredSpawners.Count)];
                    Vector3 position = spawner.Position;
                    Quaternion rotation = Quaternion.Euler(0, spawner.SideTag == Side.Left ? -225 : -45,0);
                    
                    Instantiate(prefab, position, rotation, _xenolithHolder);
                    timer = 0;
                }

                timer += Time.deltaTime;
                yield return null;
            }
        }

        #endregion

        #region Utility

        private void PrepareXenolithTypes()
        {
            string debug = "Prefabs types detected:\n";
            
            foreach (GameObject prefab in _xenolithsPrefabs)
            {
                Xenolith xenolithOld = prefab.GetComponent<Xenolith>();
                _xenolithsPrefabsTypes.Add(xenolithOld.Type, prefab);
                debug += $"({xenolithOld.Type.ToString()}, {prefab.name})";
            }
            
            Debug.Log(debug);
        }

        private void FiltersSpawners(Side mode)
        {
            if (mode != Side.Centered)
                _filteredSpawners = _spawners.FindAll(spawn => spawn.SideTag == mode);
            else
                _filteredSpawners = _spawners.FindAll(spawn => spawn.SideTag is Side.Right or Side.Left);
        }

        #endregion
    }
}
