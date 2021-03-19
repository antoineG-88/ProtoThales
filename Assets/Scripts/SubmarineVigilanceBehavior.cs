using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SubmarineVigilanceBehavior : MonoBehaviour
{
    public float vigilanceValue = 0f;
    public enum VigilanceState { Calme, Inquiet, Panique };
    public VigilanceState submarineState;
    public float detectionRangeCalme, detectionRangeInquiet, detectionRangePanique;
    private float currentRange;
    private bool reachInquietState;

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
    private float timer1;
    private float timer2;
    private float timer3;

    private void Start()
    {
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
        if (vigilanceValue >= 0 && vigilanceValue < 40)
        {
            submarineState = VigilanceState.Calme;
        }
        else if (vigilanceValue >= 40 && vigilanceValue < 80)
        {
            submarineState = VigilanceState.Inquiet;

            if (!reachInquietState)
            {  
                reachInquietState = true;
            }
        }
        else if (vigilanceValue >= 80 && vigilanceValue <= 100)
        {
            submarineState = VigilanceState.Panique;
        }

        if (vigilanceValue >= 100)
        {
            vigilanceValue = 100;
        }
        if (vigilanceValue <= 40 && reachInquietState)
        {
            vigilanceValue = 40;
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
                IncreaseVigilanceBarIfFregateMoveAbove(2);
            }
            else
            {
                IncreaseVigilanceBarIfFregateIdleAbove(0.5f);
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
                IncreaseVigilanceBarIfSonobuoyAbove(2);
            }
        }
    }

    private void IncreaseVigilanceBarIfSonobuoyAbove(float valuePerSecond)
    {
        if (timer1 >= 1)
        {
            vigilanceValue += valuePerSecond;
            timer1 = 0;
        }
        else
        {
            timer1 += Time.deltaTime;
        }
    }

    private void IncreaseVigilanceBarIfFregateIdleAbove(float valuePerSecond)
    {
        if (timer2 >= 1)
        {
            vigilanceValue += valuePerSecond;
            timer2 = 0;
        }
        else
        {
            timer2 += Time.deltaTime;
        }
    }

    private void IncreaseVigilanceBarIfFregateMoveAbove(float valuePerSecond)
    {
        if (timer3 >= 1)
        {
            vigilanceValue += valuePerSecond;
            timer3 = 0;
        }
        else
        {
            timer3 += Time.deltaTime;
        }
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
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);
            Touch touch3 = Input.GetTouch(2);
            Touch touch4 = Input.GetTouch(3);
            Touch touch5 = Input.GetTouch(4);

            if (touch1.phase == TouchPhase.Began && touch2.phase == TouchPhase.Began && touch3.phase == TouchPhase.Began && touch4.phase == TouchPhase.Began && touch5.phase == TouchPhase.Began)
            {
                enableRangeDisplay = !enableRangeDisplay;
            }
        }
    }
}
