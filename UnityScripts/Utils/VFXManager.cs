using UnityEngine;
using System.Collections.Generic;

namespace ShogunsLegacy.Utils
{
    /// <summary>
    /// Manages VFX spawning and pooling
    /// Singleton pattern for easy access from anywhere
    /// </summary>
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager Instance { get; private set; }

        [System.Serializable]
        public class VFXEntry
        {
            public string name;
            public GameObject prefab;
            public int poolSize = 10;
        }

        [Header("VFX Prefabs")]
        [SerializeField] private List<VFXEntry> vfxEntries = new List<VFXEntry>();

        [Header("Settings")]
        [SerializeField] private bool usePooling = true;
        [SerializeField] private Transform vfxContainer;

        private Dictionary<string, Queue<GameObject>> vfxPools;
        private Dictionary<string, GameObject> vfxPrefabs;

        #region Unity Lifecycle

        private void Awake()
        {
            // Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializePools();
        }

        #endregion

        #region Initialization

        private void InitializePools()
        {
            vfxPools = new Dictionary<string, Queue<GameObject>>();
            vfxPrefabs = new Dictionary<string, GameObject>();

            // Create container if not assigned
            if (vfxContainer == null)
            {
                GameObject container = new GameObject("VFX_Container");
                container.transform.SetParent(transform);
                vfxContainer = container.transform;
            }

            // Initialize pools
            foreach (VFXEntry entry in vfxEntries)
            {
                if (entry.prefab == null) continue;

                vfxPrefabs[entry.name] = entry.prefab;

                if (usePooling)
                {
                    Queue<GameObject> pool = new Queue<GameObject>();

                    for (int i = 0; i < entry.poolSize; i++)
                    {
                        GameObject vfx = CreateVFXObject(entry.prefab);
                        pool.Enqueue(vfx);
                    }

                    vfxPools[entry.name] = pool;
                }
            }
        }

        private GameObject CreateVFXObject(GameObject prefab)
        {
            GameObject vfx = Instantiate(prefab, vfxContainer);
            vfx.SetActive(false);

            // Add auto-return component if using pooling
            if (usePooling)
            {
                VFXAutoReturn autoReturn = vfx.GetComponent<VFXAutoReturn>();
                if (autoReturn == null)
                {
                    autoReturn = vfx.AddComponent<VFXAutoReturn>();
                }
            }

            return vfx;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Spawn VFX by name at position
        /// </summary>
        public GameObject SpawnVFX(string vfxName, Vector3 position, Quaternion rotation)
        {
            if (!vfxPrefabs.ContainsKey(vfxName))
            {
                Debug.LogWarning($"[VFXManager] VFX '{vfxName}' not found!");
                return null;
            }

            GameObject vfx;

            if (usePooling && vfxPools.ContainsKey(vfxName) && vfxPools[vfxName].Count > 0)
            {
                // Get from pool
                vfx = vfxPools[vfxName].Dequeue();
                vfx.transform.position = position;
                vfx.transform.rotation = rotation;
                vfx.SetActive(true);

                // Reset particle system if present
                ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Clear();
                    ps.Play();
                }

                // Reset animator if present
                Animator animator = vfx.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.Rebind();
                    animator.Update(0f);
                }
            }
            else
            {
                // Create new instance
                vfx = Instantiate(vfxPrefabs[vfxName], position, rotation, vfxContainer);

                if (usePooling)
                {
                    // Add auto-return component
                    VFXAutoReturn autoReturn = vfx.GetComponent<VFXAutoReturn>();
                    if (autoReturn == null)
                    {
                        autoReturn = vfx.AddComponent<VFXAutoReturn>();
                    }
                }
            }

            return vfx;
        }

        /// <summary>
        /// Spawn VFX by name at position (shorthand without rotation)
        /// </summary>
        public GameObject SpawnVFX(string vfxName, Vector3 position)
        {
            return SpawnVFX(vfxName, position, Quaternion.identity);
        }

        /// <summary>
        /// Return VFX to pool
        /// </summary>
        public void ReturnVFX(string vfxName, GameObject vfx)
        {
            if (!usePooling)
            {
                Destroy(vfx);
                return;
            }

            if (vfxPools.ContainsKey(vfxName))
            {
                vfx.SetActive(false);
                vfxPools[vfxName].Enqueue(vfx);
            }
            else
            {
                Destroy(vfx);
            }
        }

        /// <summary>
        /// Register new VFX at runtime
        /// </summary>
        public void RegisterVFX(string name, GameObject prefab, int poolSize = 10)
        {
            if (vfxPrefabs.ContainsKey(name))
            {
                Debug.LogWarning($"[VFXManager] VFX '{name}' already registered!");
                return;
            }

            vfxPrefabs[name] = prefab;

            if (usePooling)
            {
                Queue<GameObject> pool = new Queue<GameObject>();

                for (int i = 0; i < poolSize; i++)
                {
                    GameObject vfx = CreateVFXObject(prefab);
                    pool.Enqueue(vfx);
                }

                vfxPools[name] = pool;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get VFX duration (from particle system or animator)
        /// </summary>
        public float GetVFXDuration(string vfxName)
        {
            if (!vfxPrefabs.ContainsKey(vfxName)) return 1f;

            GameObject prefab = vfxPrefabs[vfxName];

            // Check particle system
            ParticleSystem ps = prefab.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                return ps.main.duration;
            }

            // Check animator
            Animator animator = prefab.GetComponent<Animator>();
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
                if (clips.Length > 0)
                {
                    return clips[0].length;
                }
            }

            // Default
            return 1f;
        }

        #endregion
    }

    /// <summary>
    /// Auto-return VFX to pool after duration
    /// Attached automatically to pooled VFX
    /// </summary>
    public class VFXAutoReturn : MonoBehaviour
    {
        private ParticleSystem ps;
        private Animator animator;
        private float duration;
        private float timer;
        private bool isActive;

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
            animator = GetComponent<Animator>();

            CalculateDuration();
        }

        private void OnEnable()
        {
            isActive = true;
            timer = duration;
        }

        private void Update()
        {
            if (!isActive) return;

            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                ReturnToPool();
            }
        }

        private void CalculateDuration()
        {
            // Get duration from particle system
            if (ps != null)
            {
                duration = ps.main.duration + ps.main.startLifetime.constantMax;
            }
            // Get duration from animator
            else if (animator != null && animator.runtimeAnimatorController != null)
            {
                AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
                if (clips.Length > 0)
                {
                    duration = clips[0].length;
                }
            }
            else
            {
                duration = 1f; // Default
            }
        }

        private void ReturnToPool()
        {
            isActive = false;

            // Try to find VFX name by checking VFXManager entries
            if (VFXManager.Instance != null)
            {
                // For now, just disable. VFXManager will handle re-pooling
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
