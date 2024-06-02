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

        public bool WaveInProgress;

        private void Start()
        {
            PrepareXenolithTypes();
        }

        #region Logic

        public void StartXenolithOffensive()
        {
            if (_spawners.Count == 0) {
                Debug.LogWarning("XenoManager: No spawns assigned. Wave hasn't been launched.");
                return;
            }
            
            gameObject.SetActive(true);
            StartCoroutine(LaunchSpawnCoroutine(new List<XenoType> { XenoType.Lambda }));
        }

        private IEnumerator LaunchSpawnCoroutine(List<XenoType> typePresence)
        {
            float timer = 0f;

            WaveInProgress = true;
            
            while (gameObject.activeInHierarchy)
            {
                while (WaveInProgress)
                {
                    if (timer > _spawnRate)
                    {
                        XenoType type = typePresence[Random.Range(0, typePresence.Count)];
                        GameObject prefab = _xenolithsPrefabsTypes[type];
                        XenolithSpawner spawner = _spawners[Random.Range(0, _spawners.Count)];
                        Vector3 position = spawner.Position;
                        Quaternion rotation = Quaternion.Euler(0, spawner.SideTag == Side.Left ? -225 : -45, 0);

                        Instantiate(prefab, position, rotation, _xenolithHolder);
                        timer = 0;
                    }

                    timer += Time.deltaTime;
                    yield return null;
                }

                yield return null;
            }
        }

        public void StopOffensive()
        {
            Xenolith[] xenoliths = GetComponentsInChildren<Xenolith>();

            foreach (Xenolith xenolith in xenoliths)
            {
                xenolith.Agent.enabled = false;
            }
        }

        #endregion

        #region Utility

        private void PrepareXenolithTypes()
        {
            foreach (GameObject prefab in _xenolithsPrefabs)
            {
                Xenolith xenolithOld = prefab.GetComponent<Xenolith>();
                _xenolithsPrefabsTypes.Add(xenolithOld.Type, prefab);
            }
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
