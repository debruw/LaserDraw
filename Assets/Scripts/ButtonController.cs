using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public float duration;

    private bool beginDragEvent = false;

    public delegate void ButtonClick(ButtonController button);
    public static event ButtonClick OnClicked;
    public GameObject MissedText;

    public Color[] colors;

    private void Start()
    {
        int rand = Random.Range(0, colors.Length);
        startButtonInside.color = colors[rand];
        endButtonInside.color = colors[rand];
        dragRegionInside.color = new Color(colors[rand].r, colors[rand].g, colors[rand].b, .4f);
        DragFill.color = colors[rand];
    }

    public void InitializeButton(float start)
    {
        this.startButton.gameObject.SetActive(true);

        this.buttonTimer = new Stopwatch();
        this.buttonTimer.Start();
        gameObject.SetActive(true);

        StartCoroutine(this.ScaleIndicator());
    }

    bool isFading = false;
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isGameOver)
        {
            return;
        }
        if (this.startButton.gameObject.activeSelf && this.buttonTimer.ElapsedMilliseconds > this.duration && !isFading)
        {
            UnityEngine.Debug.Log("<color=red>Miss the click</color>");
            Instantiate(MissedText, transform.position, Quaternion.identity, GameManager.Instance.inputController.gameObject.transform);
            //TODO miss the click          
            WrongClickAction();

            StartCoroutine(this.FadeAway());
        }
        else if (Input.GetMouseButton(0) && this.beginDragEvent)
        {
            FillButton();
        }
        else if (Input.GetMouseButtonUp(0) && this.beginDragEvent)
        {
            GraphicRaycaster gr = FindObjectOfType<GraphicRaycaster>();
            PointerEventData ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(ped, results);
            foreach (var item in results)
            {
                if (item.gameObject.CompareTag("EndButton") && !isFading)
                {
                    this.buttonTimer.Stop();
                    OnClicked(this);

                    StartCoroutine(this.FadeAway());
                    return;
                }
            }
            if (!isFading)
            {
                //if we cant find true button then its a WRONG click
                UnityEngine.Debug.Log("<color=red>Wrong Click</color>");
                Instantiate(MissedText, transform.position, Quaternion.identity, GameManager.Instance.inputController.gameObject.transform);
                WrongClickAction(); 
            }
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

    public void StartButtonOnPointerDown()
    {
        UnityEngine.Debug.Log("Pointer down");
        this.beginDragEvent = true;
        indicator.gameObject.SetActive(false);
        StartButtonScaleUpper();
        StartCoroutine(StartButtonFadeAway());
    }

    public void WrongClickAction()
    {
        //Stop timer
        this.buttonTimer.Stop();
        //Reset timer
        this.buttonTimer.Reset();
        //Fade away button
        StartCoroutine(this.FadeAway());
        //Reset combo state
        GameManager.Instance.isComboActive = false;
        //Activate wrong click actions
        GameManager.Instance.inputController.ActivateWrongClick();

    }

    private IEnumerator ScaleIndicator()
    {
        Vector3 originalScale = this.indicator.transform.localScale;
        Vector3 destinationScale = new Vector3(0.6f, 0.6f, 0.6f);

        if (this.buttonTimer.IsRunning)
        {
            while (this.buttonTimer.ElapsedMilliseconds < (this.duration))
            {
                this.indicator.transform.localScale = Vector3.Lerp(originalScale, destinationScale, this.buttonTimer.ElapsedMilliseconds / (this.duration));
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
        float TotalTime = 0.5f;
        isFading = true;
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
        GameManager.Instance.inputController.ActivateButton();
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
        float TotalTime = 0.5f;
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
