using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private int level = 0;
    private static GameManager instance = null;
    private bool paused = false;

    private int gemCount;
    private bool gemAcquiredThisLevel = false;

    private UIManager ui;

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Use this for initialization
    void Start () {
        if (instance == null) {
            instance = this;
            gemCount = 0;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Ensure that GameManager is properly set up on each scene
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        InitGame();
    }

    void InitGame() {
        level = SceneManager.GetActiveScene().buildIndex;
        gemAcquiredThisLevel = false;
        paused = false;
        Time.timeScale = 1f;
        if (level != 0) {
            ui = GameObject.Find("UIManager").GetComponent<UIManager>();
            ui.UpdateGemCount(gemCount);
        }
    }

    public void Restart() {
        SceneManager.LoadScene(level);
    }

    public void TogglePause() {
        Time.timeScale = paused ? 1f : 0f;
        paused = !paused;
    }

    public void Quit() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void AdvanceToNextLevel() {
        if (SceneManager.GetActiveScene().name != "EndOfGame") {
            if (gemAcquiredThisLevel) {
                gemCount++;
            }
            SceneManager.LoadScene(level + 1);
        }
    }

    public void AcquireGem() {
        gemAcquiredThisLevel = true;
        ui.UpdateGemCount(gemCount + 1);
    }

    public int GetGemCount() {
        return gemCount;
    }
}
