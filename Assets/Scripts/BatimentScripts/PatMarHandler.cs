using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PatMarHandler : MonoBehaviour
{
    public bool useChargingRelease;
    public BatimentController batimentController;
    [Space]
    public int maxSonoCapacity;
    public float sonobuoyReleaseChargeTime;
    public GameObject sonobuoyPrefab;
    public Image sonobuoyReleaseChargeImage;
    public Text sonobuoyRemainingText;
    public Transform submarine;
    public GameObject releaseInfo;
    public GameObject standardActionInfo;
    public float distanceToRelease;
    public LayerMask surfaceMask;
    public GameObject releasePosPreview;

    private int currentSonoRemaining;
    private float currentReleaseCharge;
    private bool releaseCharging;
    private PatMar patMar;
    private Vector2 releasePos;
    [HideInInspector] public bool isWaitingForReleasePosChoice;
    private bool isReleasingSonobuoy;

    [Header("Airport Settings")]
    public float flyTimeAvailable;
    public float timeToReloadAtAirport;
    public float rangeAirport;
    public GameObject airport;
    public Image fuelLevel;
    private bool EnoughFarFromAirport;
    private bool dontBackToAirport;
    private bool hadComeBackManually;

    void Start()
    {
        patMar = GetComponent<PatMar>();
        currentSonoRemaining = maxSonoCapacity;
        fuelLevel.fillAmount = 1;

        patMar.currentPosition = SeaCoord.Planify(airport.transform.position);
        patMar.currentDestination = SeaCoord.Planify(airport.transform.position);
    }


    void Update()
    {
        sonobuoyRemainingText.text = currentSonoRemaining.ToString();

        #region ChargingRelease
        if (releaseCharging)
        {
            if (currentReleaseCharge < sonobuoyReleaseChargeTime)
            {
                currentReleaseCharge += Time.deltaTime;
                sonobuoyReleaseChargeImage.fillAmount = currentReleaseCharge / sonobuoyReleaseChargeTime;
            }
            else
            {
                ReleaseSonobouy();
            }
        }
        else
        {
            sonobuoyReleaseChargeImage.fillAmount = 0;
        }
        #endregion

        if(isWaitingForReleasePosChoice)
        {
            if(batimentController.batimentSelected != patMar)
            {
                StopSonobuoyRelease();
            }
            else
            {
                if (InputDuo.tapDown && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    RaycastHit touchHit = InputDuo.SeaRaycast(surfaceMask, !Input.GetButton("LeftClick"));
                    if (touchHit.collider != null)
                    {
                        isReleasingSonobuoy = true;
                        releasePos = SeaCoord.Planify(touchHit.point);
                    }
                }
            }
        }

        if(isReleasingSonobuoy)
        {
            isWaitingForReleasePosChoice = false;
            patMar.currentDestination = releasePos;
            releasePosPreview.SetActive(true);
            releasePosPreview.transform.position = SeaCoord.GetFlatCoord(releasePos);
            if (Vector2.Distance(patMar.currentPosition, releasePos) < distanceToRelease)
            {
                ReleaseSonobouy();
                isReleasingSonobuoy = false;
            }
        }
        else
        {
            releasePosPreview.SetActive(false);
        }

        //Fuel Level
        if (patMar.canFly)
        {          
            fuelLevel.fillAmount -= 1f / flyTimeAvailable * Time.deltaTime;

            if (fuelLevel.fillAmount <= 0)
            {
                BackToAirport();
                EnoughFarFromAirport = false;
            }

            float patmarDistanceAiport = Vector2.Distance(transform.position, airport.transform.position);

            if (patmarDistanceAiport > rangeAirport * 2)
            {
                EnoughFarFromAirport = true;
            }
            if (EnoughFarFromAirport && patmarDistanceAiport < rangeAirport)
            {
                if (!dontBackToAirport)
                {
                    hadComeBackManually = true;
                    BackToAirport();
                }
            }
        }
        if (patMar.patmarIsReloading)
        {
            ReloadPatmar();
        }

    }


    public void StartSonobuoyRelease()
    {
        if (currentSonoRemaining > 0 && !isWaitingForReleasePosChoice && !isReleasingSonobuoy && !releaseCharging)
        {
            if(useChargingRelease)
            {
                releaseCharging = true;
            }
            else
            {
                isWaitingForReleasePosChoice = true;
                releaseInfo.SetActive(true);
                standardActionInfo.SetActive(false);
            }
        }
    }

    public void StopSonobuoyRelease()
    {
        releaseInfo.SetActive(false);
        standardActionInfo.SetActive(true);
        isWaitingForReleasePosChoice = false;
        isReleasingSonobuoy = false;
    }

    private void ReleaseSonobouy()
    {
        StopSonobuoyRelease();
        currentSonoRemaining--;
        currentReleaseCharge = 0;
        releaseCharging = false;

        Sonobuoy releasedSonobuoy = Instantiate(sonobuoyPrefab, SeaCoord.GetFlatCoord(patMar.currentPosition), Quaternion.identity).GetComponent<Sonobuoy>();
        releasedSonobuoy.submarine = submarine;
    }

    private void BackToAirport()
    {
        patMar.currentDestination = SeaCoord.Planify(airport.transform.position);
        patMar.canChangeDestination = false;

        if (patMar.arrivedAtDestination)
        {
            dontBackToAirport = true;
            EnoughFarFromAirport = false;
            patMar.canFly = false;
            patMar.patmarIsReloading = true;          
        }
    }

    private void ReloadPatmar()
    {
        patMar.currentDestination = SeaCoord.Planify(airport.transform.position);

        if (hadComeBackManually)
        {
            fuelLevel.fillAmount += 1f / (timeToReloadAtAirport / 2) * Time.deltaTime;
        }
        else
        {
            fuelLevel.fillAmount += 1f / timeToReloadAtAirport * Time.deltaTime;
        }

        if (fuelLevel.fillAmount >= 1)
        {
            dontBackToAirport = false;
            patMar.canChangeDestination = true;
            patMar.patmarIsReloading = false;
            hadComeBackManually = false;
        }
    }
}
