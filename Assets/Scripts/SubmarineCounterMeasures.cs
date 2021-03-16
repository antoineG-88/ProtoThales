using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineCounterMeasures : MonoBehaviour
{
    public SubmarineVigilanceBehavior submarineVigilanceScript;

    [Header("Silence Radio")]
    public float timeBeforeLauchSL;
    public float durationSL;
    public float cooldownTimeSL;
    public float vigilanceValueCostSL;
    public bool usingSilenceRadio;
    [HideInInspector] public bool submarineIsInvisible;

    private void Start()
    {
        
    }

    private void Update()
    {
        //Lauch Silence Radio
        if (submarineVigilanceScript.vigilanceValue >= 100 && !usingSilenceRadio)
        {
            usingSilenceRadio = true;
            StartCoroutine(SubmarineIsInvisible());
        }
    }

    IEnumerator SubmarineIsInvisible()
    {
        yield return new WaitForSeconds(timeBeforeLauchSL);

        submarineIsInvisible = true;

        yield return new WaitForSeconds(durationSL);

        submarineVigilanceScript.vigilanceValue -= vigilanceValueCostSL;
        submarineIsInvisible = false;
        usingSilenceRadio = false;
    }
}
