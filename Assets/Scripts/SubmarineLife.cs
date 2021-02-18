using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmarineLife : MonoBehaviour
{
    [Header("Submarine Life")]
    public float maxLife;
    private int numberSteps;
    private float stepDistance;
    public float damageCoef;
    public float currentLife;

    [Space]
    public float[] timeDecreaseEachStep;
    private int indexStep;
    private float timer;

    [Header("Damage")]
    public float damageDistance;
    public GameObject fregate;
    private float fregateDistance;

    [Header("UI")]
    public Image lifeBar;
    public GameObject UIParent;
    public RectTransform startBarPoint;
    public GameObject line;

    private float distanceFromFregate;
    private SubmarineTriggerZone submarineTriggerScript;

    private void Start()
    {
        submarineTriggerScript = GetComponentInChildren<SubmarineTriggerZone>();

        currentLife = maxLife;

        numberSteps = timeDecreaseEachStep.Length;

        float stepRange = 290f / numberSteps;
        stepDistance = stepRange;

        for (int i = 0; i < numberSteps - 1; i++)
        {
            GameObject lineObject =  Instantiate(line, new Vector3(startBarPoint.position.x + stepDistance, startBarPoint.position.y), startBarPoint.rotation);
            stepDistance += stepRange;
            lineObject.transform.SetParent(UIParent.transform);
        }
    }

    private void Update()
    {
        
    }

    private void DamageOverTime()
    {
        currentLife -= (maxLife / numberSteps);
    }
}
