using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShogunsLegacy.Managers
{
    /// <summary>
    /// Main game manager - Singleton pattern
    /// Handles game state, scene transitions, and coordinates other managers
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.MainMenu;

        [Header("Scene References")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        [SerializeField] private string hubSceneName = "Hub";
        [SerializeField] private string gameplaySceneName = "Gameplay";

        [Header("Debug")]
        [SerializeField] private bool debugMode = false;

        public GameState CurrentState => currentState;
        public bool IsPaused { get; private set; }

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
            DontDestroyOnLoad(gameObject);

            Initialize();
        }

        #endregion

        #region Initialization

        private void Initialize()
        {
            // Set target frame rate
            Application.targetFrameRate = 60;

            // Initialize sub-managers here
            if (debugMode)
            {
                Debug.Log("[GameManager] Initialized");
            }
        }

        #endregion

        #region Game State Management

        public void SetGameState(GameState newState)
        {
            if (currentState == newState) return;

            GameState previousState = currentState;
            currentState = newState;

            if (debugMode)
            {
                Debug.Log($"[GameManager] State changed: {previousState} -> {newState}");
            }

            OnGameStateChanged(previousState, newState);
        }

        private void OnGameStateChanged(GameState previous, GameState current)
        {
            // Handle state transitions
            switch (current)
            {
                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    break;

                case GameState.Hub:
                    Time.timeScale = 1f;
                    break;

                case GameState.InRun:
                    Time.timeScale = 1f;
                    break;

                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;

                case GameState.GameOver:
                    Time.timeScale = 1f;
                    break;
            }
        }

        #endregion

        #region Pause System

        public void PauseGame()
        {
            if (IsPaused) return;

            IsPaused = true;
            Time.timeScale = 0f;

            if (debugMode)
            {
                Debug.Log("[GameManager] Game Paused");
            }
        }

        public void ResumeGame()
        {
            if (!IsPaused) return;

            IsPaused = false;
            Time.timeScale = 1f;

            if (debugMode)
            {
                Debug.Log("[GameManager] Game Resumed");
            }
        }

        public void TogglePause()
        {
            if (IsPaused)
                ResumeGame();
            else
                PauseGame();
        }

        #endregion

        #region Scene Management

        public void LoadMainMenu()
        {
            SetGameState(GameState.MainMenu);
            SceneManager.LoadScene(mainMenuSceneName);
        }

        public void LoadHub()
        {
            SetGameState(GameState.Hub);
            SceneManager.LoadScene(hubSceneName);
        }

        public void StartNewRun()
        {
            SetGameState(GameState.InRun);
            SceneManager.LoadScene(gameplaySceneName);
        }

        public void ReturnToHub()
        {
            LoadHub();
        }

        public void QuitGame()
        {
            if (debugMode)
            {
                Debug.Log("[GameManager] Quitting game");
            }

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        #endregion

        #region Debug Helpers

        private void Update()
        {
            // Debug shortcuts
            if (debugMode)
            {
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    LoadMainMenu();
                }
                if (Input.GetKeyDown(KeyCode.F2))
                {
                    LoadHub();
                }
                if (Input.GetKeyDown(KeyCode.F3))
                {
                    StartNewRun();
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Enum representing different game states
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Hub,
        InRun,
        Paused,
        GameOver,
        Victory
    }
}
