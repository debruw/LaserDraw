using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(FadeAway());
    }

    public IEnumerator FadeAway()
    {

        Color originalColor = GetComponent<Text>().color;
        Color finalColor = new Color(GetComponent<Text>().color.r, GetComponent<Text>().color.g, GetComponent<Text>().color.b, 0);

        float ElapsedTime = 0.0f;
        float TotalTime = 1f;
        while (ElapsedTime < TotalTime)
        {
            ElapsedTime += Time.deltaTime;
            transform.position += new Vector3(0, Time.deltaTime * 50, 0);
            GetComponent<Text>().color = Color.Lerp(originalColor, finalColor, (ElapsedTime / TotalTime));

            yield return null;
        }
        Destroy(gameObject);
    }
}
