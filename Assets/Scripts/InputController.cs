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

    private SortedList<float, ButtonItem> gameButtons = new SortedList<float, ButtonItem>();

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
    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isGameStarted)
        {
            return;
        }

        //if (this.gameButtons.Count > 0 && this.gameTimer.ElapsedMilliseconds > this.gameButtons.Keys[0])
        //{
        //    int buttonNum = 4 - System.Math.Abs(this.roundedButtonCount) % 4;
        //    float keyTime = this.gameButtons.Keys[0];

        //    this.gameButtons.Remove(keyTime);
        //}
        //else if (gameButtons.Count == 0)
        //{

        //}
    }

    public void ActivateButton()
    {
        StartCoroutine(WaitAndActivate());
    }

    ButtonController buttonController;
    IEnumerator WaitAndActivate()
    {
        yield return new WaitForSeconds(2);
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
        UnityEngine.Debug.Log("ButtonClick");
        GameManager.Instance.inputController.ActivateButton();       
        
        //TODO clicked
        if (isFirstButton)
        {
            isFirstButton = false;
            GameManager.Instance.isDrawStarted = true;
        }
        else
        {
            GameManager.Instance.isComboActive = true;
            GameManager.Instance.SpeedUpDraw();
        }
    }

    private int ButtonCountInitializer()
    {
        int count = this.gameButtons.Count;
        int nearestMultiple = (int)System.Math.Round((count / (double)4), System.MidpointRounding.AwayFromZero) * 4;
        return nearestMultiple - 1;
    }

    private void OnDestroy()
    {
        ButtonController.OnClicked -= OnGameButtonClick;
    }
}
