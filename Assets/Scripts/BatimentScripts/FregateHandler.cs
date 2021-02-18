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
    public float helicopterDestinationTime;
    public float helicopterCooldown;
    public float[] deepSonarDistanceSteps;
    public Sprite[] deepSonarDistanceStepImages;
    public Color equipmentEnable;
    public Color equipmentCooldown;
    public GameObject sonarEffectPrefab;
    public Image deepSonarCharge;
    public Image hullSonarActivation;
    public Image helicopterDestination;

    public GameObject releaseInfo;
    public GameObject standardActionInfo;
    public GameObject helicopter;
    public GameObject selectionHelicopter;
    public Transform submarine;
    public PinHandler pinHandler;
    public BatimentController batimentScript;

    [HideInInspector] public Zone currentZone;
    private float currentSonarCharge;
    private float currentActivationTime;
    private Fregate fregate;

    private bool isUsingDeepSonar;
    [HideInInspector] public bool isUsingHullSonar;
    private bool isUsingHelicopter;
    private bool deepSonarCoolingDown;
    private bool hullSonarCoolingDown;
    private bool helicopterCoolingDown;

    private bool resetSelect;


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
            if (!hullSonarCoolingDown)
            {
                UseHullSonar();
                currentActivationTime += Time.deltaTime;

                if (currentActivationTime > hullSonarActivationTime)
                {                  
                    hullSonarCoolingDown = true;
                }

                hullSonarActivation.fillAmount = currentActivationTime / hullSonarActivationTime;
                hullSonarActivation.color = equipmentEnable;
            }
            else
            {             
                if (hullSonarActivation.fillAmount <= 0)
                {
                    currentActivationTime = 0;
                    isUsingHullSonar = false;
                    hullSonarCoolingDown = false;
                }

                hullSonarActivation.fillAmount -= 1f / hullSonarCooldown * Time.deltaTime;
                hullSonarActivation.color = equipmentCooldown;
            }        
        }

        //Deep Sonar
        if (!isUsingDeepSonar)
        {
            deepSonarCharge.fillAmount = 0;
        }
        else if (isUsingDeepSonar)
        {
            if (!deepSonarCoolingDown)
            {
                currentSonarCharge += Time.deltaTime;

                if (currentSonarCharge > deepSonarChargeTime)
                {
                    UseDeepSonar();
                    deepSonarCoolingDown = true;
                }

                deepSonarCharge.fillAmount = currentSonarCharge / deepSonarChargeTime;
                deepSonarCharge.color = equipmentEnable;
            }
            else
            {
                if (deepSonarCharge.fillAmount <= 0)
                {
                    currentSonarCharge = 0;
                    isUsingDeepSonar = false;
                    deepSonarCoolingDown = false;
                }

                deepSonarCharge.fillAmount -= 1f / deepSonarCooldown * Time.deltaTime;
                deepSonarCharge.color = equipmentCooldown;
            }  
        }

        //Helicopter
        if (!isUsingHelicopter)
        {
            helicopterDestination.fillAmount = 0;
        }
        else
        {
            if (helicopter.GetComponent<Helicopter>().inMovement)
            {
                if (!resetSelect)
                {
                    resetSelect = true;
                    batimentScript.batimentSelected = GetComponent<Fregate>();
                    selectionHelicopter.SetActive(false);
                }

                if (!helicopterCoolingDown)
                {
                    releaseInfo.SetActive(false);
                    standardActionInfo.SetActive(true);
                    helicopterDestination.color = equipmentEnable;

                    if (helicopterDestination.fillAmount >= 1)
                    {
                        helicopterCoolingDown = true;
                    }

                    helicopterDestination.fillAmount += 1f / helicopter.GetComponent<Helicopter>().timeBetweenPoints * Time.deltaTime;
                }               
            }
            if (helicopterCoolingDown)
            {
                if (helicopterDestination.fillAmount <= 0)
                {
                    helicopterCoolingDown = false;
                    isUsingHelicopter = false;
                    resetSelect = false;
                }

                helicopterDestination.fillAmount -= 1f / helicopterCooldown * Time.deltaTime;
                helicopterDestination.color = equipmentCooldown;

            }
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
    }

    public void UseHullSonar()
    {
        //put hull sonar behavior here
    }

    public void ActivateDeepSonar()
    {
        isUsingDeepSonar = true;
    }

    public void ActivateHullSonar()
    {
        isUsingHullSonar = true;
    }

    public void StartHelicopterRelease()
    {
        isUsingHelicopter = true;

        batimentScript.batimentSelected = helicopter.GetComponent<Helicopter>();
        selectionHelicopter.SetActive(true);

        releaseInfo.SetActive(true);
        standardActionInfo.SetActive(false);
    }

    public void StopHelicopterRelease()
    {
        isUsingHelicopter = false;

        batimentScript.batimentSelected = GetComponent<Fregate>();
        selectionHelicopter.SetActive(false);

        releaseInfo.SetActive(false);
        standardActionInfo.SetActive(true);
    }
}
