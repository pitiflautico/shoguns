using System.Collections.Generic;
using UnityEngine;

namespace ShogunsLegacy.Utils
{
    /// <summary>
    /// Generic object pooler for performance optimization
    /// Prevents constant Instantiate/Destroy calls for frequently spawned objects
    /// Use for: projectiles, VFX, enemies, particles, etc.
    /// </summary>
    public class ObjectPooler : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        public static ObjectPooler Instance { get; private set; }

        [Header("Pools Configuration")]
        [SerializeField] private List<Pool> pools = new List<Pool>();

        [Header("Settings")]
        [SerializeField] private bool expandPool = true; // Create new instances if pool is empty

        private Dictionary<string, Queue<GameObject>> poolDictionary;

        #region Unity Lifecycle

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            InitializePools();
        }

        #endregion

        #region Initialization

        private void InitializePools()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = CreatePooledObject(pool.prefab);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        private GameObject CreatePooledObject(GameObject prefab)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform); // Organize under this object
            return obj;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Spawn object from pool by tag
        /// </summary>
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"[ObjectPooler] Pool with tag '{tag}' doesn't exist!");
                return null;
            }

            GameObject objectToSpawn;

            // Check if pool has available objects
            if (poolDictionary[tag].Count > 0)
            {
                objectToSpawn = poolDictionary[tag].Dequeue();
            }
            else
            {
                if (expandPool)
                {
                    // Create new instance if pool is empty and expansion is allowed
                    Pool pool = pools.Find(p => p.tag == tag);
                    objectToSpawn = CreatePooledObject(pool.prefab);
                    Debug.Log($"[ObjectPooler] Expanded pool '{tag}' with new instance");
                }
                else
                {
                    Debug.LogWarning($"[ObjectPooler] Pool '{tag}' is empty and expansion is disabled!");
                    return null;
                }
            }

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            // Call OnObjectSpawn if the object has IPooledObject interface
            IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
            pooledObj?.OnObjectSpawn();

            return objectToSpawn;
        }

        /// <summary>
        /// Return object to pool
        /// </summary>
        public void ReturnToPool(string tag, GameObject obj)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"[ObjectPooler] Pool with tag '{tag}' doesn't exist!");
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            poolDictionary[tag].Enqueue(obj);
        }

        /// <summary>
        /// Add a new pool at runtime
        /// </summary>
        public void AddPool(string tag, GameObject prefab, int size)
        {
            if (poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"[ObjectPooler] Pool with tag '{tag}' already exists!");
                return;
            }

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < size; i++)
            {
                GameObject obj = CreatePooledObject(prefab);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(tag, objectPool);
            pools.Add(new Pool { tag = tag, prefab = prefab, size = size });
        }

        /// <summary>
        /// Clear all pools
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var pool in poolDictionary.Values)
            {
                while (pool.Count > 0)
                {
                    GameObject obj = pool.Dequeue();
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
            }

            poolDictionary.Clear();
        }

        #endregion
    }

    /// <summary>
    /// Interface for objects that need to reset state when spawned from pool
    /// </summary>
    public interface IPooledObject
    {
        void OnObjectSpawn();
    }
}
