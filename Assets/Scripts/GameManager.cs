using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TapticPlugin;

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
        if (!PlayerPrefs.HasKey("VIBRATION"))
        {
            PlayerPrefs.SetInt("VIBRATION", 1);
            VibrationButton.GetComponent<Image>().sprite = on;
        }
        else
        {
            if (PlayerPrefs.GetInt("VIBRATION") == 1)
            {
                VibrationButton.GetComponent<Image>().sprite = on;
            }
            else
            {
                VibrationButton.GetComponent<Image>().sprite = off;
            }
        }
    }

    public InputController inputController;
    public LineDrawer lineDrawer;
    public bool isGameOver, isDrawStarted, isComboActive;
    public int currentLevel;
    int MaxLevelNumber = 20;

    #region UI Elements
    public Text CountDownText, LevelText;
    public GameObject WinPanel, LosePanel, IngamePanel;
    public Button VibrationButton;
    public Sprite on, off;
    #endregion

    private void Start()
    {
        currentLevel = PlayerPrefs.GetInt("LevelId");
        LevelText.text = "Level " + currentLevel;
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
        SoundManager.Instance.StopAllSounds();
        SoundManager.Instance.playSound(SoundManager.GameSounds.Win);
        if (PlayerPrefs.GetInt("VIBRATION") == 1)
            TapticManager.Impact(ImpactFeedback.Light);
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
        //inputController.CloseWrongLines();
        SoundManager.Instance.StopAllSounds();
        SoundManager.Instance.playSound(SoundManager.GameSounds.Lose);
        if (PlayerPrefs.GetInt("VIBRATION") == 1)
            TapticManager.Impact(ImpactFeedback.Light);
        IngamePanel.SetActive(false);
        LosePanel.SetActive(true);
    }

    public void VibrateButtonClick()
    {
        if (PlayerPrefs.GetInt("VIBRATION").Equals(1))
        {//Vibration is on
            PlayerPrefs.SetInt("VIBRATION", 0);
            VibrationButton.GetComponent<Image>().sprite = off;
        }
        else
        {//Vibration is off
            PlayerPrefs.SetInt("VIBRATION", 1);
            VibrationButton.GetComponent<Image>().sprite = on;
        }

        if (PlayerPrefs.GetInt("VIBRATION") == 1)
            TapticManager.Impact(ImpactFeedback.Light);
    }
}
