using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmarineLife : MonoBehaviour
{
    [Header("Submarine Life")]
    public float maxLife;
    public float damageCoef;
    public float currentLife;

    [Header("Feedback")]
    public float maxDistance;
    public Color colorFar;
    public Color colorClose;
    [Space]
    public GameObject colorFeedback;
    public GameObject fregate;

    [Header("UI")]
    public Image lifeBar;

    private float distanceFromFregate;
    private SubmarineTriggerZone submarineTriggerScript;

    private void Start()
    {
        colorFeedback.SetActive(false);

        submarineTriggerScript = GetComponentInChildren<SubmarineTriggerZone>();

        currentLife = maxLife;
    }

    private void Update()
    {
        if (submarineTriggerScript.fregateIsAbove)
        {
            DamageOverTime();

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

        distanceFromFregate = Vector3.Distance(transform.position, fregate.transform.position);

        if (distanceFromFregate <= maxDistance)
        {
            ChangeColorOverDistance();
        }
    }

    private void DamageOverTime()
    {
        currentLife -= damageCoef * Time.deltaTime;
    }

    private void ChangeColorOverDistance()
    {
        colorFeedback.SetActive(true);

        colorFeedback.transform.position = new Vector3(fregate.transform.position.x + 2, fregate.transform.position.y + 2, fregate.transform.position.z + 2);

        colorFeedback.GetComponent<SpriteRenderer>().color = Color.Lerp(colorClose, colorFar, distanceFromFregate / maxDistance);
    }
}
