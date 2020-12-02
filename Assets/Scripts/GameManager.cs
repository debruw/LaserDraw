using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public InputController inputController;
    public LineDrawer lineDrawer;
    public bool isGameOver, isDrawStarted, isComboActive;
    public int currentLevel;
    int MaxLevelNumber = 3;

    #region UI Elements
    public Text CountDownText, LevelText;
    public GameObject WinPanel, LosePanel, IngamePanel;
    #endregion

    private void Start()
    {
        currentLevel = PlayerPrefs.GetInt("LevelId");
        LevelText.text = "Level " + currentLevel;
        if (currentLevel == 1 || currentLevel == 7)
        {
            //if (TutorialCanvas != null)
            //{
            //    TutorialCanvas.SetActive(true);
            //}
        }
        StartCoroutine(Countdown(3));
    }

    IEnumerator Countdown(int seconds)
    {
        int count = seconds;

        while (count > 0)
        {
            // display something...
            CountDownText.text = count.ToString();
            yield return new WaitForSeconds(1);
            count--;
        }

        // count down is finished...
        CountDownText.gameObject.SetActive(false);
        StartGame();
    }

    public void StartGame()
    {
        inputController.StartTimer();
        GameManager.Instance.isDrawStarted = true;
    }

    public void NextLevelClick()
    {
        if (currentLevel > MaxLevelNumber)
        {
            int rand = Random.Range(1, MaxLevelNumber);
            if (rand == PlayerPrefs.GetInt("LastRandomLevel"))
            {
                rand = Random.Range(1, MaxLevelNumber);
            }
            else
            {
                PlayerPrefs.SetInt("LastRandomLevel", rand);
            }
            SceneManager.LoadScene("Level" + rand);
        }
        else
        {
            SceneManager.LoadScene("Level" + currentLevel);
        }
    }

    public void RetryClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SpeedUpDraw()
    {
        lineDrawer.lineDrawSpeed = lineDrawer.lineDefaultSpeed * 2;
    }

    public GameObject Fireworks;
    public void ExplodeFireworks()
    {        
        Fireworks.SetActive(true);
        StartCoroutine(WaitAndWin());
    }

    IEnumerator WaitAndWin()
    {        
        yield return new WaitForSeconds(1f);        
        GameWin();
    }

    void GameWin()
    {
        currentLevel++;
        PlayerPrefs.SetInt("LevelId", currentLevel);
        inputController.CloseWrongLines();
        IngamePanel.SetActive(false);
        WinPanel.SetActive(true);
    }

    public void StartLose()
    {
        isGameOver = true;        
        lineDrawer.Sparks.SetActive(false);
        StartCoroutine(WaitAndLose());
    }

    IEnumerator WaitAndLose()
    {       
        yield return new WaitForSeconds(1f);        
        GameLose();
    }

    void GameLose()
    {
        inputController.CloseWrongLines();
        IngamePanel.SetActive(false);
        LosePanel.SetActive(true);
    }
}
