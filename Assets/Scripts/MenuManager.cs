using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public int currentLevel;
    int MaxLevelNumber = 3;
    int rand;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("LevelId"))
        {
            PlayerPrefs.SetInt("LevelId", 1);
        }
        currentLevel = PlayerPrefs.GetInt("LevelId");
        Debug.Log(currentLevel);
    }

    public void StartButtonClick()
    {
        if (currentLevel > MaxLevelNumber)
        {
            if (currentLevel != PlayerPrefs.GetInt("LastRandomLevel") && PlayerPrefs.GetInt("LastRandomLevel") != 0)
            {

            }
            rand = Random.Range(1, MaxLevelNumber);
            if (rand == PlayerPrefs.GetInt("LastRandomLevel"))
            {
                rand = Random.Range(1, MaxLevelNumber);
            }
            SceneManager.LoadScene("Level" + rand);
        }
        else
        {
            SceneManager.LoadScene("Level" + currentLevel);
        }
    }
}
