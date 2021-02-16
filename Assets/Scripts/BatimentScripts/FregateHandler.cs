using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FregateHandler : MonoBehaviour
{
    public float hullSonarActivationTime;
    public float hullSonarCooldown;
    public float deepSonarChargeTime;
    public float deepSonarCooldown;
    public float[] deepSonarDistanceSteps;
    public Sprite[] deepSonarDistanceStepImages;
    public Color equipmentEnable;
    public Color equipmentCooldown;
    public GameObject sonarEffectPrefab;
    public Image deepSonarCharge;
    public Image hullSonarActivation;
    public Transform submarine;
    public PinHandler pinHandler;

    [HideInInspector] public Zone currentZone;
    private float currentSonarCharge;
    private float currentActivationTime;
    private Fregate fregate;

    private bool isUsingDeepSonar;
    private bool isUsingHullSonar;
    

    void Start()
    {
        fregate = GetComponent<Fregate>();
    }

    void Update()
    {
        currentZone = ZoneHandler.GetCurrentZone(fregate.currentPosition);

        //Hull Sonar
        if (!isUsingHullSonar)
        {
            hullSonarActivation.fillAmount = 0;
        }
        else if (isUsingHullSonar)
        {
            currentActivationTime += Time.deltaTime;
            if (currentActivationTime > hullSonarActivationTime)
            {
                UseHullSonar();
            }
            hullSonarActivation.fillAmount = currentActivationTime / hullSonarActivationTime;
        }

        //Deep Sonar
        if (!isUsingDeepSonar)
        {
            deepSonarCharge.fillAmount = 0;
        }
        else if (isUsingDeepSonar)
        {
            currentSonarCharge += Time.deltaTime;
            if (currentSonarCharge > deepSonarChargeTime)
            {
                UseDeepSonar();
            }
            deepSonarCharge.fillAmount = currentSonarCharge / deepSonarChargeTime;
        }  
    }


    private void UseDeepSonar()
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
        isUsingDeepSonar = false;
    }

    public void UseHullSonar()
    {
        //put hull sonar behavior here

        currentActivationTime -= Time.deltaTime;
        if (currentActivationTime > hullSonarCooldown)
        {
            currentActivationTime = 0;
            isUsingHullSonar = false;
        }
        hullSonarActivation.fillAmount = currentActivationTime / hullSonarCooldown;
    }

    public void ActivateDeepSonar()
    {
        isUsingDeepSonar = true;
    }

    public void ActivateHullSonar()
    {
        isUsingHullSonar = true;
    }
}
