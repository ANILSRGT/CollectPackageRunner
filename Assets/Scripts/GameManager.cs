using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameState
{
    Idle,
    Play,
    End,
}

public class GameManager : SingletonMonobehaviour<GameManager>
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;

    [Header("Game Panel")]
    public TextMeshProUGUI scoreTxt;

    [Header("Management")]
    [ReadOnly] public int currentLevelID = 0;
    [ReadOnly] public LevelManager currentLevelManager;
    [SerializeField] private List<LevelManager> levelManagerPrefabs = new List<LevelManager>();

    [HideInInspector] public GameState gameState = GameState.Idle;
    [HideInInspector] public int collectedObjects = 0;
    private PlayerController _playerController;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        SetupGame();
    }

    /// <summary>
    /// First setup of the game
    /// </summary>
    private void SetupGame()
    {
        _playerController = FindObjectOfType<PlayerController>();
        gameState = GameState.Idle;
        collectedObjects = 0;

        if (!PlayerPrefs.HasKey("LevelID"))
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.LevelID, 0);
        }

        currentLevelID = PlayerPrefs.GetInt(PlayerPrefsKeys.LevelID);
        currentLevelManager = Instantiate(levelManagerPrefabs.Where(x => x.levelProperty.levelID == currentLevelID).First());

        Camera.main.transform.position = currentLevelManager.levelProperty.cameraTransform.position;
        Camera.main.transform.rotation = currentLevelManager.levelProperty.cameraTransform.rotation;

        _playerController.transform.position = currentLevelManager.levelProperty.playerTransform.position;
        _playerController.transform.rotation = currentLevelManager.levelProperty.playerTransform.rotation;

        SetScoreTxt(collectedObjects);
        ActivePanel(mainMenuPanel);
    }

    /// <summary>
    /// Start the game
    /// </summary>
    public void StartGame()
    {
        ActivePanel(gamePanel);
        Events.OnStartGame?.Invoke();
        gameState = GameState.Play;
    }

    /// <summary>
    /// Add score to the game
    /// </summary>
    public void AddScore(int value)
    {
        collectedObjects += value;
        SetScoreTxt(collectedObjects);
    }

    /// <summary>
    /// Set the score text
    /// </summary>
    private void SetScoreTxt(int value) => scoreTxt.text = $"{(value < 0 ? 0 : value)}/{currentLevelManager.levelProperty.requiredCollectableObjCount}";

    /// <summary>
    /// End the game
    /// </summary>
    /// <return>Is the game win?</return>
    public bool FinishLevel()
    {
        Events.OnEndGame?.Invoke();
        gameState = GameState.End;

        if (collectedObjects >= currentLevelManager.levelProperty.requiredCollectableObjCount) ActivePanel(gameWinPanel);
        else ActivePanel(gameOverPanel);

        return collectedObjects >= currentLevelManager.levelProperty.requiredCollectableObjCount;
    }

    /// <summary>
    /// Restart the game
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Go to the next level
    /// </summary>
    public void NextLevel()
    {
        currentLevelID++;
        if (currentLevelID >= levelManagerPrefabs.Count) currentLevelID = 0;
        Debug.Log($"Next level: {currentLevelID}");
        PlayerPrefs.SetInt(PlayerPrefsKeys.LevelID, currentLevelID);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Activate only the panel given as parameter
    /// </summary>
    /// <param name="panel">Panel to activate</param>
    private void ActivePanel(GameObject panel)
    {
        gameOverPanel.SetActive(false);
        gamePanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        gameWinPanel.SetActive(false);
        panel.SetActive(true);
    }
}
