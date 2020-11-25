using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    public GameObject[] buttonPrefabs;
    public Text scoreLabel;
    public string gameDataFileName;
    public float gameSpeed;

    private int gameScore = 0;
    private int roundedButtonCount;
    public Stopwatch gameTimer = new Stopwatch();

    private SortedList<float, ButtonItem> gameButtons = new SortedList<float, ButtonItem>();

    // Use this for initialization
    void Start()
    {
        if (!this.LoadGameData())
        {
            return;
        }
        
        ButtonController.OnClicked += OnGameButtonClick;

        this.roundedButtonCount = this.ButtonCountInitializer();
    }

    public void StartTimer()
    {
        this.gameTimer.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isGameStarted)
        {
            return;
        }
        if (this.gameButtons.Count > 0 && this.gameTimer.ElapsedMilliseconds > this.gameButtons.Keys[0])
        {
            int buttonNum = 4 - System.Math.Abs(this.roundedButtonCount) % 4;
            float keyTime = this.gameButtons.Keys[0];

            this.CreateButton(gameTimer.ElapsedMilliseconds, this.gameButtons[keyTime].position,
                this.gameButtons[keyTime].isDrag, this.gameButtons[keyTime].endPosition, buttonNum);

            if (this.gameButtons[keyTime].isDrag)
            {
                this.roundedButtonCount--;
            }

            this.gameButtons.Remove(keyTime);
            this.roundedButtonCount--;
        }
        else if (gameButtons.Count == 0)
        {

        }
    }

    private bool LoadGameData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);

            ButtonData buttonData = JsonUtility.FromJson<ButtonData>(dataAsJson);

            for (int i = 0; i < buttonData.buttons.Count; ++i)
            {
                this.gameButtons.Add(buttonData.buttons[i].time, buttonData.buttons[i]);
            }

            return true;
        }

        return false;
    }

    public void CreateButton(float startTime, float[] startPos, bool isDrag, float[] endPos, int buttonNum)
    {
        GameObject button = Instantiate(buttonPrefabs[Random.Range(0,1)], new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        button.transform.SetParent(GameObject.FindGameObjectWithTag("GameController").transform, false);
        ButtonController buttonController = button.GetComponent<ButtonController>();

        buttonController.startButtonText.text = buttonNum.ToString();
        if (isDrag)
        {
            buttonController.endButtonText.text = (buttonNum + 1).ToString();
        }

        buttonController.duration = gameSpeed;
        buttonController.InitializeButton(startTime, startPos[0], startPos[1], isDrag, endPos[0], endPos[1]);
    }

    bool isFirstButton = true;
    public void OnGameButtonClick(ButtonController button)
    {
        UnityEngine.Debug.Log("ButtonClick");
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

        this.gameScore += (Mathf.RoundToInt((button.buttonScore * 1000) / 100) * 100);
        this.UpdateScoreLabel(gameScore);
    }

    private void UpdateScoreLabel(int scoreValue)
    {
        this.scoreLabel.text = scoreValue.ToString();
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
