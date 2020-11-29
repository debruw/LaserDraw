using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public LineRenderer LineRenderer, BeamEffect;
    public GameObject Sparks;
    private float counter;
    private float dist;

    public Transform[] LinePoints;

    public float lineDrawSpeed;
    [HideInInspector]
    public float lineDefaultSpeed;
    public Light pointLight;
    public Gradient[] RainbowColors;

    private void Start()
    {
        lineDefaultSpeed = lineDrawSpeed;
        LineRenderer = GetComponent<LineRenderer>();
        LineRenderer.SetPosition(0, LinePoints[0].position);
        LineRenderer.SetPosition(1, LinePoints[0].position);

        dist = Vector3.Distance(LinePoints[0].position, LinePoints[1].position);
    }

    public int i;
    [HideInInspector]
    public Vector3 pointA, pointB, pointAlongLine;
    bool isLast;

    private void Update()
    {
        if (!GameManager.Instance.isDrawStarted || GameManager.Instance.isGameOver)
        {
            return;
        }
        if (!Sparks.activeSelf)
        {
            Sparks.SetActive(true);
        }
        //Debug.Log(counter + " >? " + dist);
        if (counter < dist)
        {
            //Debug.Log("1");
            //Debug.Log(lineDrawSpeed * Time.deltaTime);
            counter += lineDrawSpeed * Time.deltaTime;

            float x = Mathf.Lerp(0, dist, counter);

            pointA = LinePoints[i].position;
            if (!isLast)
            {
                pointB = LinePoints[i + 1].position;
            }

            pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;
            LineRenderer.SetPosition(i + 1, pointAlongLine);

            Sparks.transform.position = pointAlongLine;
            BeamEffect.SetPosition(1, pointAlongLine);
        }
        else
        {
            //Time.timeScale = 0;
            //Debug.Log("2");
            LineRenderer.SetPosition(i + 1, pointB);
            if (pointLight.intensity < 4.25f)
            {
                pointLight.intensity += (0.75f / LinePoints.Length);
            }
            if (LinePoints.Length - 1 > i + 1)
            {
                counter = 0;
                i++;
                dist = Vector3.Distance(LinePoints[i].position, LinePoints[i + 1].position);
                LineRenderer.positionCount++;
                LineRenderer.SetPosition(LineRenderer.positionCount - 1, LineRenderer.GetPosition(LineRenderer.positionCount - 2));
            }
            else if (!isLast)
            {
                //counter = 0;
                //i++;
                //dist = Vector3.Distance(LinePoints[i].position, LinePoints[0].position);
                //LineRenderer.positionCount++;
                //LineRenderer.SetPosition(LineRenderer.positionCount - 1, LineRenderer.GetPosition(LineRenderer.positionCount - 2));
                isLast = true;
            }
            else
            {
                LineRenderer.colorGradient = RainbowColors[0];
                Sparks.SetActive(false);
                BeamEffect.gameObject.SetActive(false);
                GameManager.Instance.inputController.gameTimer.Stop();
                StartCoroutine(WaitAndChangeColor());
                GameManager.Instance.isDrawStarted = false;
                GameManager.Instance.ExplodeFireworks();
            }
        }
    }

    int currentColor;
    IEnumerator WaitAndChangeColor()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);
            currentColor++;
            if (currentColor > RainbowColors.Length - 1)
            {
                currentColor = 0;
            }
            LineRenderer.colorGradient = RainbowColors[currentColor];
        }
    }
}
