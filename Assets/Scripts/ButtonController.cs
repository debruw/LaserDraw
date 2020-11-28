using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public Image startButton;
    public Image startButtonInside;
    public Image endButton;
    public Image endButtonInside;
    public Image dragRegion;
    public Image dragRegionInside;
    public Image dragDirection;
    private Stopwatch buttonTimer;
    public Image indicator;
    public IndicatorCollision indicatorCollision;

    public float duration;

    private bool beginDragEvent = false;

    public delegate void ButtonClick(ButtonController button);
    public static event ButtonClick OnClicked;


    public void InitializeButton(float start)
    {
        this.startButton.gameObject.SetActive(true);

        this.buttonTimer = new Stopwatch();
        this.buttonTimer.Start();
        gameObject.SetActive(true);

        StartCoroutine(this.ScaleIndicator());
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isGameOver)
        {
            return;
        }
        if (this.startButton != null && this.startButton.gameObject.activeSelf && this.buttonTimer.ElapsedMilliseconds > this.duration)
        {
            UnityEngine.Debug.Log("not clicked");

            //TODO cant clicked
            GameManager.Instance.inputController.ActivateWrongClick();
            GameManager.Instance.isComboActive = false;
            GameManager.Instance.inputController.ActivateButton();

            this.buttonTimer.Stop();
            this.buttonTimer.Reset();
            //OnClicked(this);

            StartCoroutine(this.FadeAway());
        }
        else if (Input.GetMouseButton(0) && this.beginDragEvent && this.indicatorCollision.isHit)
        {
            //StartCoroutine(this.MoveIndicator());
            FillButton();
            if (startButton.gameObject.activeSelf)
            {
                StartCoroutine(StartButtonFadeAway());
            }
        }
        else if (Input.GetMouseButtonUp(0) && this.beginDragEvent)
        {
            this.buttonTimer.Stop();
            OnClicked(this);

            StartCoroutine(this.FadeAway());
        }
    }

    public Image DragFill;
    float dis1, dis2;
    public void FillButton()
    {
        dis1 = Vector3.Distance(startButton.transform.position, Input.mousePosition);
        dis2 = Vector3.Distance(startButton.transform.position, endButton.transform.position);
        DragFill.fillAmount = dis1 / dis2;
    }

    public void ButtonClicked()
    {
        this.beginDragEvent = true;
        indicator.gameObject.SetActive(false);
        StartButtonScaleUpper();
    }

    public float CalcPerfectTime()
    {
        return ((this.duration) / 2f);
    }

    public float CalcColor()
    {
        if (((this.buttonTimer.ElapsedMilliseconds) / CalcPerfectTime()) <= 1f)
        {
            return (this.buttonTimer.ElapsedMilliseconds) / CalcPerfectTime();
        }
        else if ((duration - this.buttonTimer.ElapsedMilliseconds) / CalcPerfectTime() <= 1)
        {
            return (duration - this.buttonTimer.ElapsedMilliseconds) / CalcPerfectTime();
        }
        return 0;

    }

    private IEnumerator ScaleIndicator()
    {
        Vector3 originalScale = this.indicator.transform.localScale;
        Vector3 destinationScale = new Vector3(0.7f, 0.7f, 0.7f);

        if (this.buttonTimer.IsRunning)
        {
            while (this.buttonTimer.ElapsedMilliseconds < (this.duration / 2f))
            {
                this.indicator.transform.localScale = Vector3.Lerp(originalScale, destinationScale, this.buttonTimer.ElapsedMilliseconds / (this.duration / 2f));
                yield return null;
            }
        }
    }

    public void StartButtonScaleUpper()
    {
        StartCoroutine(ButtonScaleUpper());
    }

    public IEnumerator ButtonScaleUpper()
    {
        Vector3 originalScale = this.startButton.transform.localScale;
        Vector3 destinationScale = new Vector3(1.2f, 1.2f, 1.2f);

        if (this.buttonTimer.IsRunning)
        {
            while (this.buttonTimer.ElapsedMilliseconds < (this.duration / 2f))
            {
                this.startButton.transform.localScale = Vector3.Lerp(originalScale, destinationScale, this.buttonTimer.ElapsedMilliseconds / (this.duration / 2f));
                yield return null;
            }
        }
    }

    public IEnumerator FadeAway()
    {
        Collider2D buttonCollider = this.indicator.GetComponent<CircleCollider2D>();
        if (buttonCollider != null)
        {
            buttonCollider.enabled = false;
        }

        Color originalColor = this.startButton.color;
        Color finalColor = new Color(this.startButton.color.r, this.startButton.color.g, this.startButton.color.b, 0);

        float ElapsedTime = 0.0f;
        float TotalTime = 0.6f;
        while (ElapsedTime < TotalTime)
        {
            ElapsedTime += Time.deltaTime;
            if (startButton.gameObject.activeSelf)
            {
                this.startButton.color = Color.Lerp(originalColor, finalColor, (ElapsedTime / TotalTime));
                this.startButtonInside.color = Color.Lerp(originalColor, finalColor, (ElapsedTime / TotalTime)); 
            }
            this.endButton.color = Color.Lerp(originalColor, finalColor, (ElapsedTime / TotalTime));
            this.endButtonInside.color = Color.Lerp(originalColor, finalColor, (ElapsedTime / TotalTime));
            this.dragRegion.color = Color.Lerp(originalColor, finalColor, (ElapsedTime / TotalTime));
            this.dragRegionInside.color = Color.Lerp(originalColor, finalColor, (ElapsedTime / TotalTime));
            this.dragDirection.color = Color.Lerp(originalColor, finalColor, (ElapsedTime / TotalTime));
            this.indicator.color = Color.Lerp(originalColor, finalColor, (ElapsedTime / TotalTime));
            this.DragFill.color = Color.Lerp(originalColor, finalColor, (ElapsedTime / TotalTime));

            yield return null;
        }

        Destroy(this.gameObject);
    }

    public IEnumerator StartButtonFadeAway()
    {
        Collider2D buttonCollider = this.indicator.GetComponent<CircleCollider2D>();
        if (buttonCollider != null)
        {
            buttonCollider.enabled = false;
        }

        Color originalColor = this.startButton.color;
        Color finalColor = new Color(this.startButton.color.r, this.startButton.color.g, this.startButton.color.b, 0);

        float ElapsedTime = 0.0f;
        float TotalTime = 0.6f;
        while (ElapsedTime < TotalTime)
        {
            ElapsedTime += Time.deltaTime;
            this.startButton.color = Color.Lerp(originalColor, finalColor, (ElapsedTime / TotalTime));
            this.startButtonInside.color = Color.Lerp(originalColor, finalColor, (ElapsedTime / TotalTime));

            yield return null;
        }
        startButton.gameObject.SetActive(false);
    }

}
