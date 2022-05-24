using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Idle,
    Play,
    End,
}

public class GameManager : Singleton<GameManager>
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameOverPanel;
    public GameObject gamePanel;

    [Header("Game Panel")]
    public TextMeshProUGUI scoreTxt;

    [HideInInspector] public GameState gameState = GameState.Idle;
    [HideInInspector] public int collectedObjects = 0;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        scoreTxt.text = "Score : " + collectedObjects;
        ActiveCanvas(mainMenuPanel);
    }

    public void StartGame()
    {
        ActiveCanvas(gamePanel);
        Events.OnStartGame?.Invoke();
        gameState = GameState.Play;
    }

    public void AddScore(int value)
    {
        collectedObjects += value;
        scoreTxt.text = "Score : " + collectedObjects;
    }

    public void FinishLevel()
    {
        Events.OnEndGame?.Invoke();
        gameState = GameState.End;
        ActiveCanvas(gameOverPanel);
    }

    private void ActiveCanvas(GameObject canvas)
    {
        gameOverPanel.SetActive(false);
        gamePanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        canvas.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
