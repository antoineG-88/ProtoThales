using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmarineVigilanceBehavior : MonoBehaviour
{
    public float vigilanceValue = 0f;
    public enum VigilanceState { Calme, Inquiet, Panique };
    public VigilanceState submarineState;
    public Sprite calmeSprite, inquietSprite, paniqueSprite;
    public Image vigilanceStateImage;
    public float detectionRangeCalme, detectionRangeInquiet, detectionRangePanique;
    private float currentRange;
    private bool reachInquietState;
    public float vigilanceIncreaseRatioInFlat;

    [Header("Objects")]
    public FregateMovement fregateMovementScript;
    public SubmarineCounterMeasures submarineCounterMeasuresScript;
    public MadBehavior madBehaviorScript;
    public List<GameObject> sonobuoys;
    public List<float> sonobuoysDistance;

    [Header("Debug")]
    public bool enableRangeDisplay;
    public GameObject rangeDisplay;
    private SpriteRenderer rangeSprite;

    private float timer;
    private SubmarineMovementBehavior submarineMovementBehavior;

    private void Start()
    {
        submarineMovementBehavior = GetComponent<SubmarineMovementBehavior>();
        sonobuoys = new List<GameObject>();
        rangeDisplay.SetActive(false);
        rangeDisplay.transform.localScale = new Vector2(detectionRangeCalme * 2, detectionRangeCalme * 2);
        rangeSprite = rangeDisplay.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        ChangeState();

        ChangeSubmarineRange();

        DetectFregate();

        DetectSonobuoy();

        //Debug
        EnableDebugRange();
        DisplaySubmarineDebug();
    }

    private void ChangeState()
    {
        if (vigilanceValue >= 0 && vigilanceValue < 40 && !reachInquietState)
        {
            submarineState = VigilanceState.Calme;
            vigilanceStateImage.sprite = calmeSprite;
        }
        else if ((vigilanceValue >= 40 && vigilanceValue < 80) || (vigilanceValue >= 0 && vigilanceValue < 40 && reachInquietState))
        {
            submarineState = VigilanceState.Inquiet;
            vigilanceStateImage.sprite = inquietSprite;
            reachInquietState = true;
        }
        else if (vigilanceValue >= 80 && vigilanceValue <= 100)
        {
            submarineState = VigilanceState.Panique;
            vigilanceStateImage.sprite = paniqueSprite;
        }

        if (vigilanceValue >= 100)
        {
            vigilanceValue = 100;
        }
    }

    private void ChangeSubmarineRange()
    {
        if (submarineState == VigilanceState.Calme)
        {
            rangeDisplay.transform.localScale = new Vector2(detectionRangeCalme * 2, detectionRangeCalme * 2);
            currentRange = detectionRangeCalme;
        }
        else if (submarineState == VigilanceState.Inquiet)
        {
            rangeDisplay.transform.localScale = new Vector2(detectionRangeInquiet* 2, detectionRangeInquiet * 2);
            currentRange = detectionRangeInquiet;
        }
        else if (submarineState == VigilanceState.Panique)
        {
            rangeDisplay.transform.localScale = new Vector2(detectionRangePanique * 2, detectionRangePanique * 2);
            currentRange = detectionRangePanique;
        }
    }

    private void DetectFregate()
    {
        float distanceFromFregate = Vector3.Distance(transform.position, fregateMovementScript.transform.position);

        if (distanceFromFregate < currentRange)
        {
            submarineCounterMeasuresScript.submarineDetectFregate = true;

            if (fregateMovementScript.isMoving)
            {
                IncreaseVigilance(2);
            }
            else
            {
                IncreaseVigilance(0.5f);
            }
        }
        else
        {
            submarineCounterMeasuresScript.submarineDetectFregate = false;
        }
    }

    private void DetectSonobuoy()
    {
        sonobuoys.Clear();

        for (int i = 0; i < madBehaviorScript.sonobuoys.Count; i++)
        {
            sonobuoys.Add(madBehaviorScript.sonobuoys[i].gameObject);
        }
        sonobuoysDistance = new List<float>(new float[sonobuoys.Count]);
        for (int x = 0; x < sonobuoys.Count; x++)
        {
            sonobuoysDistance[x] = Vector3.Distance(transform.position, sonobuoys[x].transform.position);

            if(sonobuoysDistance[x] < currentRange)
            {
                IncreaseVigilance(2);
            }
        }
    }

    private void IncreaseVigilance(float valuePerSecond)
    {
        vigilanceValue += valuePerSecond * Time.deltaTime * (submarineMovementBehavior.submarineZone.relief == TerrainZone.Relief.Flat ? vigilanceIncreaseRatioInFlat : 1);
    }

    private void EnableDebugRange()
    {
        if (enableRangeDisplay)
        {
            rangeDisplay.SetActive(true);
        }
        else
        {
            rangeDisplay.SetActive(false);
        }
    }

    private void DisplaySubmarineDebug()
    {
        if (Input.touchCount > 4)
        {
            Touch touch = Input.GetTouch(5);

            if (touch.phase == TouchPhase.Began)
            {
                enableRangeDisplay = !enableRangeDisplay;
            }
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            enableRangeDisplay = !enableRangeDisplay;
        }
    }
}
