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

    #region UI Elements
    public Text CountDownText;
    public GameObject WinPanel, LosePanel, IngamePanel;
    #endregion

    private void Start()
    {
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

    public void Restart()
    {
        SceneManager.LoadScene(0);
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
        IngamePanel.SetActive(false);
        WinPanel.SetActive(true);
    }

    public void Lose()
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
        IngamePanel.SetActive(false);
        LosePanel.SetActive(true);
    }
}
