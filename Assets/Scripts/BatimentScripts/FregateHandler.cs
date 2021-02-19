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
    public float deepSonarVigilanceMaxIncrease;
    public float helicopterCooldown;
    public float helicopterFlashRadius;
    public float slowDownTimeSonoFlash;
    public float[] deepSonarDistanceSteps;
    public Sprite[] deepSonarDistanceStepImages;
    public Color equipmentEnable;
    public Color equipmentCooldown;
    public GameObject sonarEffectPrefab;
    public Image deepSonarCharge;
    public Image hullSonarActivation;
    public Image helicopterDestination;

    public GameObject winPannel;
    public GameObject releaseInfo;
    public GameObject standardActionInfo;
    public Helicopter helicopter;
    public GameObject selectionHelicopter;
    public GameObject selectionFregate;
    public Submarine submarine;
    public PinHandler pinHandler;
    public BatimentController batimentScript;

    [HideInInspector] public Zone currentZone;
    private float currentSonarCharge;
    private float currentActivationTime;
    private Fregate fregate;

    private bool isUsingDeepSonar;
    [HideInInspector] public bool isUsingHullSonar;
    [HideInInspector] public bool isUsingHelicopter;
    private bool deepSonarCoolingDown;
    private bool hullSonarCoolingDown;
    [HideInInspector] public bool helicopterCoolingDown;

    private bool resetSelect;


    void Start()
    {
        fregate = GetComponent<Fregate>();
        winPannel.SetActive(false);
        helicopter.gameObject.SetActive(false);
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
            if (!helicopterCoolingDown)
            {
                if (helicopter.inMovement)
                {
                    if (!resetSelect)
                    {
                        resetSelect = true;
                        batimentScript.batimentSelected = fregate;
                        selectionFregate.SetActive(true);
                        selectionHelicopter.SetActive(false);
                    }

                    releaseInfo.SetActive(false);
                    standardActionInfo.SetActive(true);
                    helicopterDestination.color = equipmentEnable;

                    if (helicopterDestination.fillAmount >= 1)
                    {
                        helicopterCoolingDown = true;
                        UseFlashHelicopter();
                    }

                    helicopterDestination.fillAmount += 1f / helicopter.timeBetweenPoints * Time.deltaTime;
                }
                else
                {
                    helicopter.currentPosition = fregate.currentPosition;
                    helicopter.transform.position = SeaCoord.GetFlatCoord(helicopter.currentPosition);
                }
            }         
            else
            {
                if (helicopterDestination.fillAmount <= 0)
                {
                    helicopterCoolingDown = false;
                    isUsingHelicopter = false;
                    resetSelect = false;
                    helicopter.gameObject.SetActive(false);
                }

                helicopterDestination.fillAmount -= 1f / helicopterCooldown * Time.deltaTime;
                helicopterDestination.color = equipmentCooldown;
            }
        }
    }


    private void UseDeepSonar()
    {
        int distanceStep = 1;
        float distance = Vector2.Distance(submarine.currentPosition, fregate.currentPosition);

        for (int i = 0; i < deepSonarDistanceSteps.Length; i++)
        {
            if (distance > deepSonarDistanceSteps[i])
            {
                distanceStep++;
            }
        }

        pinHandler.CreateDeepSonarPin(distanceStep, fregate.currentPosition);
        Instantiate(sonarEffectPrefab, fregate.transform.position + Vector3.up * 0.1f, Quaternion.identity);

        submarine.Alert(deepSonarVigilanceMaxIncrease * Mathf.Clamp((0.2f + (1 - Mathf.Clamp((distance / deepSonarDistanceSteps[deepSonarDistanceSteps.Length - 1]), 0f, 1f))), 0f, 1f));
    }

    public void UseFlashHelicopter()
    {
        float distanceSubmarine = Vector2.Distance(helicopter.currentPosition, submarine.currentPosition);

        if (distanceSubmarine < helicopterFlashRadius)
        {
            winPannel.SetActive(true);
        }
        else
        {
            StartCoroutine(SlowDownSubmarine());
        }
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

        batimentScript.batimentSelected = helicopter;
        helicopter.gameObject.SetActive(true);
        selectionHelicopter.SetActive(true);
        selectionFregate.SetActive(false);
        helicopter.currentPosition = fregate.currentPosition;
        helicopter.transform.position = SeaCoord.GetFlatCoord(helicopter.currentPosition);
        helicopter.currentDestination = helicopter.currentPosition;

        if (!helicopterCoolingDown)
        {
            releaseInfo.SetActive(true);
            standardActionInfo.SetActive(false);
        }
    }

    public void StopHelicopterRelease()
    {
        isUsingHelicopter = false;

        batimentScript.batimentSelected = fregate;
        helicopter.gameObject.SetActive(false);
        selectionFregate.SetActive(true);

        releaseInfo.SetActive(false);
        standardActionInfo.SetActive(true);
    }

    IEnumerator SlowDownSubmarine()
    {
        submarine.currentMaxSpeed = submarine.slowSpeed;
        yield return new WaitForSeconds(slowDownTimeSonoFlash);
        submarine.currentMaxSpeed = submarine.maxSpeed;
    }
}
