using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmarineActionHandler : MonoBehaviour
{
    [Header("References")]
    public SubmarineMoveHandler submarineMoveHandler;
    public FregateMovement fregateMovementScript;
    public MadBehavior madBehavior;
    [HideInInspector] public List<GameObject> sonobuoys;
    [HideInInspector] public List<float> sonobuoysDistance;


    public float currentVigilance = 0f;
    public enum VigilanceState { Calme, Inquiet, Panique };
    public VigilanceState currentState;
    public Sprite calmeSprite, inquietSprite, paniqueSprite;
    public Image vigilanceStateImage;
    public float detectionRangeCalme, detectionRangeInquiet, detectionRangePanique;
    public GameObject rangeDisplay;
    private bool reachInquietState;
    public float sonobuoyVigiIncr;
    public float fregateMoveVigiIncr;
    public float fregateStationaryVigiIncr;
    private float currentRange;

    private bool cantUseCounterMeasure;

    [Header("Silence Radio")]
    public float timeBeforeLauchSL;
    public float durationSL;
    public float cooldownTimeSL;
    public float vigilanceDescreasePerSecond;
    [HideInInspector] public bool usingSilenceRadio;
    [HideInInspector] public bool submarineIsInvisible;

    [Header("Leurre")]
    public float timeBeforeLauchL;
    public float durationL;
    public float cooldownTimeL;
    public float lureAngle;
    public GameObject lurePrefab;
    private GameObject lure;
    private int randomDirection;
    [HideInInspector] public bool usingLeurre;
    [HideInInspector] public bool decoyAreMoving;
    private bool isIdentified;
    private float identifiedTimeRemaining;
    [HideInInspector] public bool submarineDetectFregate;
    [HideInInspector] public bool submarineDetectSonobuoy;

    [Header("Changement de Cap")]
    public float timeBeforeLauchCC;
    //public float durationCC;
    public float cooldownTimeCC;
    private bool usingChangementDeCap;
    private void Start()
    {

    }

    private void Update()
    {
        UpdateState();

        UpdateSubmarineRange();

        DetectFregate();

        DetectSonobuoy();

        if (!cantUseCounterMeasure)
        {
            //SilenceRadio();
            //Leurre();
            //ChangementDeCap();
        }

        UpdateIdentified();

        LureMovement();
    }
    private void UpdateState()
    {
        if (currentVigilance >= 0 && currentVigilance < 40 && !reachInquietState)
        {
            currentState = VigilanceState.Calme;
            vigilanceStateImage.sprite = calmeSprite;
        }
        else if ((currentVigilance >= 40 && currentVigilance < 80) || (currentVigilance >= 0 && currentVigilance < 40 && reachInquietState))
        {
            currentState = VigilanceState.Inquiet;
            vigilanceStateImage.sprite = inquietSprite;
            reachInquietState = true;
        }
        else if (currentVigilance >= 80 && currentVigilance <= 100)
        {
            currentState = VigilanceState.Panique;
            vigilanceStateImage.sprite = paniqueSprite;
        }

        if (currentVigilance >= 100)
        {
            currentVigilance = 100;
        }

        if(submarineIsInvisible)
        {
            currentVigilance -= Time.deltaTime * vigilanceDescreasePerSecond;
        }
    }

    private void UpdateSubmarineRange()
    {
        if (currentState == VigilanceState.Calme)
        {
            rangeDisplay.transform.localScale = new Vector3(detectionRangeCalme * 2, detectionRangeCalme * 2, 1);
            currentRange = detectionRangeCalme;
        }
        else if (currentState == VigilanceState.Inquiet)
        {
            rangeDisplay.transform.localScale = new Vector3(detectionRangeInquiet * 2, detectionRangeInquiet * 2, 1);
            currentRange = detectionRangeInquiet;
        }
        else if (currentState == VigilanceState.Panique)
        {
            rangeDisplay.transform.localScale = new Vector3(detectionRangePanique * 2, detectionRangePanique * 2, 1);
            currentRange = detectionRangePanique;
        }
    }

    private void DetectFregate()
    {
        float distanceFromFregate = Vector3.Distance(transform.position, fregateMovementScript.transform.position);

        if (distanceFromFregate < currentRange)
        {
            submarineDetectFregate = true;

            if (fregateMovementScript.isMoving)
            {
                IncreaseVigilance(fregateMoveVigiIncr);
            }
            else
            {
                IncreaseVigilance(fregateStationaryVigiIncr);
            }
        }
        else
        {
            submarineDetectFregate = false;
        }
    }

    private void DetectSonobuoy()
    {
        sonobuoys.Clear();

        for (int i = 0; i < madBehavior.sonobuoys.Count; i++)
        {
            sonobuoys.Add(madBehavior.sonobuoys[i].gameObject);
        }
        sonobuoysDistance = new List<float>(new float[sonobuoys.Count]);
        for (int x = 0; x < sonobuoys.Count; x++)
        {
            sonobuoysDistance[x] = Vector3.Distance(transform.position, sonobuoys[x].transform.position);

            if (sonobuoysDistance[x] < currentRange)
            {
                IncreaseVigilance(sonobuoyVigiIncr);
            }
        }
    }

    private void IncreaseVigilance(float valuePerSecond)
    {
        if(!submarineIsInvisible)
        {
            currentVigilance += valuePerSecond * Time.deltaTime;
        }
    }

    private void UpdateIdentified()
    {
        if (identifiedTimeRemaining > 0)
        {
            isIdentified = true;
            identifiedTimeRemaining -= Time.deltaTime;
        }
        else
        {
            isIdentified = false;
        }
    }

    private void SilenceRadio()
    {
        if(currentVigilance >= 100)
        {
            Debug.Log("Le sous marin se rend invisible");
            usingSilenceRadio = true;
            cantUseCounterMeasure = true;
            StartCoroutine(SubmarineIsInvisible());
        }
    }

    private void Leurre()
    {
        if (isIdentified && !usingLeurre)
        {
            Debug.Log("Le sous marin utilise un leurre");
            usingLeurre = true;
            cantUseCounterMeasure = true;
            StartCoroutine(SubmarineCreateDecoy());
        }
    }

    private void ChangementDeCap()
    {
        if ((currentState == VigilanceState.Inquiet || currentState == VigilanceState.Panique) && submarineDetectFregate && !usingChangementDeCap)
        {
            Debug.Log("Le sous marin change de cap");
            usingChangementDeCap = true;
            cantUseCounterMeasure = true;
            StartCoroutine(SubmarineChangeTargetWaypoint());
        }
    }

    private void LureMovement()
    {
        if (decoyAreMoving)
        {
            if (randomDirection == 0)
            {
                transform.position += SeaCoord.GetFlatCoord(submarineMoveHandler.currentDirection) * Time.deltaTime * submarineMoveHandler.submarineSpeed;
                lure.transform.position += Quaternion.Euler(0, lureAngle, 0) * SeaCoord.GetFlatCoord(submarineMoveHandler.currentDirection) * Time.deltaTime * submarineMoveHandler.submarineSpeed;
            }
            else if (randomDirection == 1)
            {
                transform.position += Quaternion.Euler(0, lureAngle, 0) * SeaCoord.GetFlatCoord(submarineMoveHandler.currentDirection) * Time.deltaTime * submarineMoveHandler.submarineSpeed;
                lure.transform.position += SeaCoord.GetFlatCoord(submarineMoveHandler.currentDirection) * Time.deltaTime * submarineMoveHandler.submarineSpeed;
            }
        }
    }

    private void SubmarineMovementAroundObstacle(LayerMask mask)
    {
        submarineMoveHandler.avoidanceLayerMask = mask;
    }

    IEnumerator SubmarineIsInvisible()
    {
        yield return new WaitForSeconds(timeBeforeLauchSL);

        submarineIsInvisible = true;

        yield return new WaitForSeconds(durationSL);

        submarineIsInvisible = false;
        cantUseCounterMeasure = false;

        yield return new WaitForSeconds(cooldownTimeSL);

        usingSilenceRadio = false;
    }

    IEnumerator SubmarineCreateDecoy()
    {
        yield return new WaitForSeconds(timeBeforeLauchL);

        int leftOrRight = Random.Range(0, 2);
        randomDirection = Random.Range(0, 2);
        decoyAreMoving = true;
        if (leftOrRight == 0)
        {
            //keep lureAngle
        }
        else if (leftOrRight == 1)
        {
            lureAngle = -lureAngle;
        }

        lure = Instantiate(lurePrefab, transform.position, Quaternion.identity);

        for (int i = 0; i < madBehavior.sonobuoys.Count; i++)
        {
            madBehavior.sonobuoys[i].objectsCanBeDetected.Add(lure);

        }

        yield return new WaitForSeconds(durationL);

        for (int i = 0; i < madBehavior.sonobuoys.Count; i++)
        {
            madBehavior.sonobuoys[i].objectsCanBeDetected.Remove(lure);
        }
        Destroy(lure);
        decoyAreMoving = false;
        cantUseCounterMeasure = false;

        yield return new WaitForSeconds(cooldownTimeL);

        usingLeurre = false;
    }

    IEnumerator SubmarineChangeTargetWaypoint()
    {
        yield return new WaitForSeconds(timeBeforeLauchCC);

        submarineMoveHandler.PickRandomWaypoint();

        cantUseCounterMeasure = false;

        yield return new WaitForSeconds(cooldownTimeCC);

        usingChangementDeCap = false;
    }

    public void RefreshIdentified()
    {
        identifiedTimeRemaining = 1;
    }
}
