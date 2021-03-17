using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineCounterMeasures : MonoBehaviour
{
    public SubmarineVigilanceBehavior submarineVigilanceScript;
    public SubmarineMovementBehavior submarineMovementScript;
    public MadBehavior madScript;
    
    private bool cantUseCounterMeasure;

    [Header("Silence Radio")]
    public float timeBeforeLauchSL;
    public float durationSL;
    public float cooldownTimeSL;
    public float vigilanceValueCostSL;
    public bool usingSilenceRadio;
    [HideInInspector] public bool submarineIsInvisible;

    [Header("Leurre")]
    public float timeBeforeLauchL;
    public float durationL;
    public float cooldownTimeL;
    public float vigilanceValueCostL;
    public float lureAngle;
    private float submarineSpeed;
    public GameObject lurePrefab;
    private GameObject lure;
    private int randomDirection;
    public bool usingLeurre;
    [HideInInspector] public bool decoyAreMoving;
    [HideInInspector] public bool submarineDetectByDAM;

    [Header("Contournement de Bouée")]
    public float timeBeforeLauchCB;
    //public float durationCB;
    public float cooldownTimeCB;
    public float vigilanceValueCostCB;
    public bool usingContournementBouee;
    [HideInInspector] public bool submarineDetectSonobuoy;
    RaycastHit hit;
    public float raycastLenght = 4;
    public LayerMask LayerMask;
    [HideInInspector] public bool raycastTouchObstacle; 

    [Header("Changement de Cap")]
    public float timeBeforeLauchCC;
    //public float durationCC;
    public float cooldownTimeCC;
    public float vigilanceValueCostCC;
    public bool usingChangementDeCap;
    public LayerMask _LayerMask;
    [HideInInspector] public bool submarineDetectFregate;
    [HideInInspector] public bool canAvoidFregate;

    private void Start()
    {
        submarineSpeed = GetComponent<SubmarineMovementBehavior>().submarineSpeed;
    }

    private void Update()
    {
        if (!cantUseCounterMeasure)
        {
            SilenceRadio();
            Leurre();
            //ContournementDeBouee();
            ChangementDeCap();
        }

        LureMovement();

        if (canAvoidFregate)
        {
            submarineMovementAroundObstacle(_LayerMask);
        }
    }

    private void SilenceRadio()
    {
        if (submarineVigilanceScript.vigilanceValue >= 100 && !usingSilenceRadio)
        {
            usingSilenceRadio = true;
            cantUseCounterMeasure = true;
            StartCoroutine(SubmarineIsInvisible());
        }
    }

    private void Leurre()
    {
        if (submarineDetectByDAM && !usingLeurre)
        {
            usingLeurre = true;
            cantUseCounterMeasure = true;
            StartCoroutine(SubmarineCreateDecoy());
        }
    }

    private void ContournementDeBouee()
    {
        if (submarineVigilanceScript.submarineState == SubmarineVigilanceBehavior.VigilanceState.Panique && submarineDetectSonobuoy && !usingContournementBouee)
        {
            usingContournementBouee = true;
        }
    }

    private void ChangementDeCap()
    {
        if ((submarineVigilanceScript.submarineState == SubmarineVigilanceBehavior.VigilanceState.Inquiet || submarineVigilanceScript.submarineState == SubmarineVigilanceBehavior.VigilanceState.Panique) && submarineDetectFregate && !usingChangementDeCap)
        {
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
                transform.position += submarineMovementScript.currentDirection * Time.deltaTime * submarineSpeed;
                lure.transform.position += Quaternion.Euler(0, lureAngle, 0) * submarineMovementScript.currentDirection * Time.deltaTime * submarineSpeed;
            }
            else if (randomDirection == 1)
            {
                transform.position += Quaternion.Euler(0, lureAngle, 0) * submarineMovementScript.currentDirection * Time.deltaTime * submarineSpeed;
                lure.transform.position += submarineMovementScript.currentDirection * Time.deltaTime * submarineSpeed;
            }
        }
    }

    private void submarineMovementAroundObstacle(LayerMask mask)
    {
        float raycastForward = RaySensor(transform.position, submarineMovementScript.currentDirection, 4f, mask);
        float raycastDiagonalRight = RaySensor(transform.position, Quaternion.Euler(0, 45, 0) * submarineMovementScript.currentDirection, 3f, mask);
        float raycastDiagonalLeft = RaySensor(transform.position, Quaternion.Euler(0, -45, 0) * submarineMovementScript.currentDirection, 3f, mask);

        if (raycastDiagonalRight > 0)
        {
            transform.position += Quaternion.Euler(0, -90, 0) * submarineMovementScript.currentDirection * Time.deltaTime * submarineSpeed;
            raycastTouchObstacle = true;
        }
        else if (raycastDiagonalLeft > 0)
        {
            transform.position += Quaternion.Euler(0, 90, 0) * submarineMovementScript.currentDirection * Time.deltaTime * submarineSpeed;
            raycastTouchObstacle = true;
        }
        else
        {
            raycastTouchObstacle = false;
        }
    }

    float RaySensor(Vector3 pos, Vector3 direction, float lenght, LayerMask layer)
    {
        if (Physics.Raycast(pos, direction, out hit, lenght * raycastLenght, layer))
        {
            Debug.DrawRay(pos, hit.distance * direction, Color.Lerp(Color.red, Color.green, (raycastLenght * lenght - hit.distance) / (raycastLenght * lenght)));
            return (raycastLenght * lenght - hit.distance) / (raycastLenght * lenght);
        }
        else
        {
            Debug.DrawRay(pos, direction * raycastLenght * lenght, Color.red);          
            return 0;
        }
    }

    IEnumerator SubmarineIsInvisible()
    {
        yield return new WaitForSeconds(timeBeforeLauchSL);

        submarineIsInvisible = true;

        yield return new WaitForSeconds(durationSL);

        submarineVigilanceScript.vigilanceValue -= vigilanceValueCostSL;
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

        for (int i = 0; i < madScript.sonobuoys.Count; i++)
        {
            madScript.sonobuoys[i].objectsCanBeDetected.Add(lure);

        }

        yield return new WaitForSeconds(durationL);

        for (int i = 0; i < madScript.sonobuoys.Count; i++)
        {
            madScript.sonobuoys[i].objectsCanBeDetected.Remove(lure);
        }
        Destroy(lure);
        submarineVigilanceScript.vigilanceValue -= vigilanceValueCostL;
        decoyAreMoving = false;
        cantUseCounterMeasure = false;

        yield return new WaitForSeconds(cooldownTimeL);

        usingLeurre = false;
    }

    IEnumerator SubmarineChangeTargetWaypoint()
    {
        yield return new WaitForSeconds(timeBeforeLauchCC);

        submarineMovementScript.PickRandomWaypoint();
        canAvoidFregate = true;

        submarineVigilanceScript.vigilanceValue -= vigilanceValueCostCC;
        cantUseCounterMeasure = false;

        yield return new WaitForSeconds(cooldownTimeCC);

        usingChangementDeCap = false;
    }
}
