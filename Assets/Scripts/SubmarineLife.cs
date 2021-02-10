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

    [Header("Feedback")]
    public float maxDistance;
    public Color colorFar;
    public Color colorClose;
    [Space]
    public GameObject colorFeedback;
    public GameObject fregate;

    [Header("UI")]
    public Image lifeBar;
    public GameObject UIParent;
    public RectTransform startBarPoint;
    public GameObject line;

    private float distanceFromFregate;
    private SubmarineTriggerZone submarineTriggerScript;

    private void Start()
    {
        colorFeedback.SetActive(false);

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
        if (submarineTriggerScript.fregateIsAbove)
        {
            timer += Time.deltaTime;

            if (indexStep != timeDecreaseEachStep.Length && timer >= timeDecreaseEachStep[indexStep])
            {
                DamageOverTime();
                timer = 0;
                indexStep++;
            }

            lifeBar.fillAmount = currentLife / maxLife;

            if (currentLife > maxLife)
            {
                currentLife = maxLife;
            }
            if (currentLife < 0)
            {
                currentLife = 0;
            }
        }
        else
        {
            timer = 0;
        }

        distanceFromFregate = Vector3.Distance(transform.position, fregate.transform.position);

        if (distanceFromFregate <= maxDistance)
        {
            ChangeColorOverDistance();
        }
    }

    private void DamageOverTime()
    {
        currentLife -= (maxLife / numberSteps);
    }

    private void ChangeColorOverDistance()
    {
        colorFeedback.SetActive(true);

        colorFeedback.transform.position = new Vector3(fregate.transform.position.x + 2, fregate.transform.position.y + 2, fregate.transform.position.z + 2);

        colorFeedback.GetComponent<SpriteRenderer>().color = Color.Lerp(colorClose, colorFar, distanceFromFregate / maxDistance);
    }
}
