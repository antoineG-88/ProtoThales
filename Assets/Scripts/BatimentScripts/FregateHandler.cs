using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FregateHandler : MonoBehaviour
{
    public float deepSonarChargeTime;
    public float[] deepSonarDistanceSteps;
    public Sprite[] deepSonarDistanceStepImages;
    public GameObject sonarEffectPrefab;
    public Image deepSonarCharge;
    public Image hullSonarImage;
    public Color hullSonarActivatedColor;
    public Transform submarine;
    public PinHandler pinHandler;

    [HideInInspector] public Zone currentZone;
    private float currentSonarCharge;
    private BatimentController batimentController;
    private Color hullSonarBaseColor;
    private Fregate fregate;

    private bool deepSonar;
    private bool isUsinghullSonar;
    

    void Start()
    {
        fregate = GetComponent<Fregate>();
        hullSonarBaseColor = hullSonarImage.color;
    }

    void Update()
    {
        currentZone = ZoneHandler.GetCurrentZone(fregate.currentPosition);

        if (!deepSonar)
        {
            deepSonarCharge.fillAmount = 0;
        }
        else if (deepSonar)
        {
            currentSonarCharge += Time.deltaTime;
            if (currentSonarCharge > deepSonarChargeTime)
            {
                UseSonar();
            }
            deepSonarCharge.fillAmount = currentSonarCharge / deepSonarChargeTime;
        }
    }


    private void UseSonar()
    {
        int distanceStep = 1;
        float distance = Vector3.Distance(transform.position, submarine.transform.position);
        string direction = "unknown";

        for (int i = 0; i < deepSonarDistanceSteps.Length; i++)
        {
            if (distance > deepSonarDistanceSteps[i])
            {
                distanceStep++;
            }
        }

        float angle = Vector2.SignedAngle(fregate.currentDirection, SeaCoord.Planify(submarine.position - fregate.transform.position));
        if (Mathf.Abs(angle) >= 135)
        {
            direction = "derrière";
        }
        if (Mathf.Abs(angle) < 45)
        {
            direction = "devant";
        }
        if (Mathf.Abs(angle) >= 45 && Mathf.Abs(angle) < 135)
        {
            if (angle > 0)
            {
                direction = "bâbord";
            }
            else
            {
                direction = "tribord";
            }
        }

        pinHandler.CreateDeepSonarPin(distanceStep, direction, fregate.currentDirection, fregate.currentPosition);
        Instantiate(sonarEffectPrefab, fregate.transform.position + Vector3.up * 0.1f, Quaternion.identity);
        currentSonarCharge = 0;

        deepSonar = false;
    }

    public void ActivateDeepSonar()
    {
        deepSonar = true;
    }

    public void ActivateHullSonar()
    {
        if (!isUsinghullSonar)
        {
            isUsinghullSonar = true;
            hullSonarImage.color = hullSonarActivatedColor;

        }
        else if (isUsinghullSonar)
        {
            isUsinghullSonar = false;
            hullSonarImage.color = hullSonarBaseColor;
        }
    }
}
