using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool isGameStarted, isDrawStarted, isComboActive;

    public void StartGame()
    {
        isGameStarted = true;
        inputController.StartTimer();
    }

    public void StartDraw()
    {
        isDrawStarted = true;
    }

    public void SpeedUpDraw()
    {
        lineDrawer.lineDrawSpeed += .1f;
    }

    public void SpeedDownDraw()
    {
        lineDrawer.lineDrawSpeed = lineDrawer.lineDefaultSpeed;
    }
}
