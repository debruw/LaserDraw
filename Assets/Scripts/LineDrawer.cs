using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public LineRenderer LineRenderer;
    public GameObject Sparks;
    private float counter;
    private float dist;

    public Transform[] LinePoints;

    public float lineDrawSpeed;
    [HideInInspector]
    public float lineDefaultSpeed;
    public Light pointLight;
    public Gradient RainbowColor;

    private void Start()
    {
        lineDefaultSpeed = lineDrawSpeed;
        LineRenderer = GetComponent<LineRenderer>();
        LineRenderer.SetPosition(0, LinePoints[0].position);
        LineRenderer.SetPosition(1, LinePoints[0].position);

        dist = Vector3.Distance(LinePoints[0].position, LinePoints[1].position);
    }

    int i;
    Vector3 pointA, pointB, pointAlongLine;
    bool isLast;

    private void Update()
    {
        if (!GameManager.Instance.isDrawStarted)
        {
            return;
        }
        if (!Sparks.activeSelf)
        {
            Sparks.SetActive(true);
        }

        if (counter < dist)
        {
            counter += lineDrawSpeed / 100;

            float x = Mathf.Lerp(0, dist, counter);

            pointA = LinePoints[i].position;
            if (!isLast)
            {
                pointB = LinePoints[i + 1].position;
            }

            pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;
            LineRenderer.SetPosition(i + 1, pointAlongLine);

            Sparks.transform.position = pointAlongLine;
        }
        else
        {
            LineRenderer.SetPosition(i + 1, pointB);
            if (pointLight.intensity < 4.25f)
            {
                pointLight.intensity += (4.25f / LinePoints.Length);
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
                LineRenderer.colorGradient = RainbowColor;
                Sparks.SetActive(false);
                GameManager.Instance.inputController.gameTimer.Stop();
                GameManager.Instance.isDrawStarted = false;
                GameManager.Instance.ExplodeFireworks();
            }
        }
    }
}
