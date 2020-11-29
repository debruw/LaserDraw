using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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
        if(!WrongLine1.gameObject.activeSelf)
        {
            WrongLine1.GetComponent<LineRenderer>().SetPosition(0, GameManager.Instance.lineDrawer.pointAlongLine);
            WrongLine1.SetActive(true);
        }
        else if(WrongLine1.gameObject.activeSelf && !WrongLine2.gameObject.activeSelf)
        {
            WrongLine2.GetComponent<LineRenderer>().SetPosition(0, GameManager.Instance.lineDrawer.pointAlongLine);
            WrongLine2.SetActive(true);
        }
        else if(WrongLine2.gameObject.activeSelf)
        {
            WrongLine3.GetComponent<LineRenderer>().SetPosition(0, GameManager.Instance.lineDrawer.pointAlongLine);
            WrongLine3.SetActive(true);
            GameManager.Instance.StartLose();            
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
    IEnumerator WaitAndActivate()
    {
        yield return new WaitForSeconds(1.5f);
        if (currentButton < buttons.Length)
        {
            buttonController = buttons[currentButton].GetComponent<ButtonController>();
            buttonController.InitializeButton(gameTimer.ElapsedMilliseconds);
            currentButton++;
        }
    }

    bool isFirstButton = true;
    public void OnGameButtonClick(ButtonController button)
    {
        if (GameManager.Instance.isGameOver)
        {
            return;
        }
        UnityEngine.Debug.Log("ButtonClick");
        GameManager.Instance.inputController.ActivateButton();
        if (GameManager.Instance.lineDrawer.lineDrawSpeed == 0)
        {
            GameManager.Instance.lineDrawer.lineDrawSpeed = GameManager.Instance.lineDrawer.lineDefaultSpeed;
        }
        //TODO clicked
        if (isFirstButton)
        {
            isFirstButton = false;            
        }
        else
        {
            GameManager.Instance.isComboActive = true;
            GameManager.Instance.SpeedUpDraw();
        }
    }

    private void OnDestroy()
    {
        ButtonController.OnClicked -= OnGameButtonClick;
    }
}
