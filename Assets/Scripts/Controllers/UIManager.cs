using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public float restartHoldTime = 2f;
    public Text gemCount;

    private static UIManager instance = null;
    private GameManager gm;
    private GameObject pauseMenuPanel;

    private float restartTimer = 0f;
    private Text restartingText;

    private Animator anim;


    private void OnEnable() {
        restartingText = GameObject.Find("RestartingText").GetComponent<Text>();
        pauseMenuPanel = GameObject.Find("PauseMenuPanel").gameObject;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Use this for initialization
    void Start () {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
	}

    // Ensure that UIManager is properly set up on each scene
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Init();
    }

    void Init() {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
        anim.SetTrigger("TransitionIn");
        restartTimer = 0f;
        if (pauseMenuPanel.activeSelf) {
            ToggleMenu();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Restart")) {
            restartTimer += Time.deltaTime;
            restartingText.color = Color.Lerp(Color.clear, Color.white, Mathf.Clamp01(restartTimer / restartHoldTime));
            if (restartTimer >= restartHoldTime) {
                TransitionOutRestart();
            }
        }

        if (Input.GetButtonUp("Restart")) {
            restartTimer = 0f;
            restartingText.color = Color.clear;
        }

        if (Input.GetButtonDown("Pause")) {
            TogglePause();
        }
	}

    public void TogglePause() {
        ToggleMenu();
        gm.TogglePause();
        Debug.Log(Time.timeScale);
    }

    void ToggleMenu() {
        pauseMenuPanel.SetActive(!pauseMenuPanel.gameObject.activeSelf);
        foreach (Transform t in pauseMenuPanel.transform) {
            Button button = t.GetComponent<Button>();
            if (button) {
                button.interactable = !button.interactable;
            }
        }
    }

    public void TransitionOutRestart() {
        Time.timeScale = 1f;
        anim.SetTrigger("TransitionOutRestart");
    }

    public void TransitionOutAdvance() {
        anim.SetTrigger("TransitionOutAdvance");
    }

    public void Restart() {
        gm.Restart();
    }

    public void Advance() {
        gm.AdvanceToNextLevel();
    }

    public void UpdateGemCount(int count) {
        gemCount.text = count.ToString();
    }
}
