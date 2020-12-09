using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TapticPlugin;
using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    public GameObject[] buttons;

    public Stopwatch gameTimer = new Stopwatch();

    public GameObject WrongLine1, WrongLine2, WrongLine3;

    // Use this for initialization
    void Start()
    {
        ButtonController.OnClicked += OnGameButtonClick;
    }

    public void StartTimer()
    {
        this.gameTimer.Start();
        ActivateButton();
    }

    int currentButton;

    public void ActivateWrongClick()
    {
        GameManager.Instance.lineDrawer.lineDrawSpeed = 0;
        if (!WrongLine1.gameObject.activeSelf)
        {
            WrongLine1.GetComponent<LineRenderer>().SetPosition(0, GameManager.Instance.lineDrawer.pointAlongLine);
            WrongLine1.SetActive(true);
            if (PlayerPrefs.GetInt("VIBRATION") == 1)
                TapticManager.Impact(ImpactFeedback.Light);
        }
        else if (WrongLine1.gameObject.activeSelf && !WrongLine2.gameObject.activeSelf)
        {
            WrongLine2.GetComponent<LineRenderer>().SetPosition(0, GameManager.Instance.lineDrawer.pointAlongLine);
            WrongLine2.SetActive(true);
            if (PlayerPrefs.GetInt("VIBRATION") == 1)
                TapticManager.Impact(ImpactFeedback.Light);
        }
        else if (WrongLine2.gameObject.activeSelf)
        {
            WrongLine3.GetComponent<LineRenderer>().SetPosition(0, GameManager.Instance.lineDrawer.pointAlongLine);
            WrongLine3.SetActive(true);
            GameManager.Instance.StartLose();
            if (PlayerPrefs.GetInt("VIBRATION") == 1)
                TapticManager.Impact(ImpactFeedback.Medium);
        }
    }

    public void ActivateButton()
    {
        if (GameManager.Instance.isGameOver)
        {
            return;
        }
        StartCoroutine(WaitAndActivate());
    }

    ButtonController buttonController;
    public float ButtonWaitTime = .5f;
    IEnumerator WaitAndActivate()
    {
        yield return new WaitForSeconds(ButtonWaitTime);
        if (GameManager.Instance.lineDrawer.i < GameManager.Instance.lineDrawer.LinePoints.Length - 2)
        {
            if (currentButton < buttons.Length)
            {
                if (currentButton == 0 || buttons[currentButton - 1] == null)
                {
                    buttonController = buttons[currentButton].GetComponent<ButtonController>();
                    buttonController.InitializeButton(gameTimer.ElapsedMilliseconds);
                    currentButton++;
                }
            }
        }
        else
        {
            UnityEngine.Debug.Log("<color=yellow/> !!! </color>");
        }
    }

    bool isFirstButton = true;
    public GameObject FloatingText;
    public string[] FloatingStrings;
    public void OnGameButtonClick(ButtonController button)
    {
        if (GameManager.Instance.isGameOver)
        {
            return;
        }
        UnityEngine.Debug.Log("<color=green>ButtonClick</color>");
        Text go = Instantiate(FloatingText, button.endButton.transform.position, Quaternion.identity, transform).GetComponent<Text>();
        go.text = FloatingStrings[Random.Range(0, FloatingStrings.Length)];
        
        if (GameManager.Instance.lineDrawer.lineDrawSpeed == 0)
        {
            GameManager.Instance.lineDrawer.lineDrawSpeed = GameManager.Instance.lineDrawer.lineDefaultSpeed;
        }
        //TODO clicked
        if (isFirstButton)
        {
            isFirstButton = false;
            GameManager.Instance.lineDrawer.BeamEffect2.SetActive(true);
            GameManager.Instance.lineDrawer.lineDrawSpeed = GameManager.Instance.lineDrawer.lineDefaultSpeed;
        }
        else
        {
            GameManager.Instance.isComboActive = true;
            GameManager.Instance.SpeedUpDraw();
        }
    }

    public void CloseWrongLines()
    {
        WrongLine1.SetActive(false);
        WrongLine2.SetActive(false);
        WrongLine3.SetActive(false);
    }

    private void OnDestroy()
    {
        ButtonController.OnClicked -= OnGameButtonClick;
    }
}
