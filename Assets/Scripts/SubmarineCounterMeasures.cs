using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineCounterMeasures : MonoBehaviour
{
    public SubmarineVigilanceBehavior submarineVigilanceScript;
    public SubmarineMovementBehavior submarineMovementScript;
    public MadBehavior madScript;
    [HideInInspector] public bool submarineDetectByDAM;
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
        }

        LureMovement();
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
}
