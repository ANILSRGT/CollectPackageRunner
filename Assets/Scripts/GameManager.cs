using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Canvas")]
    public GameObject gameOverCanvas;
    public GameObject gameCanvas;

    [Header("Game")]
    public TextMeshProUGUI scoreTxt;

    public bool isEndGame = true;
    [HideInInspector] public int collectedObjects = 0;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        scoreTxt.text = "Score : " + collectedObjects;
        ActiveCanvas(gameCanvas);
    }

    public void AddScore(int value)
    {
        collectedObjects += value;
        scoreTxt.text = "Score : " + collectedObjects;
    }

    public void FinishLevel()
    {
        isEndGame = true;
        ActiveCanvas(gameOverCanvas);
    }

    private void ActiveCanvas(GameObject canvas)
    {
        gameOverCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        canvas.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
