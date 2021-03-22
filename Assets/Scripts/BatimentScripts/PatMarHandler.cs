using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PatMarHandler : MonoBehaviour
{
    public BatimentController batimentController;
    public PinHandler pinHandler;
    [Space]
    public int maxSonoCapacity;
    public GameObject sonoFlashTrapPrefab;
    public Text sonobuoyRemainingText;
    public Submarine submarine;
    public GameObject releaseInfoPanel;
    public GameObject releaseInfo1;
    public GameObject releaseInfo2;
    public GameObject standardActionInfo;
    public float distanceToRelease;
    public float maxDistanceBetweenSonoflash;
    public GameObject maxDistanceBetweenBuoyPreview;
    public LayerMask surfaceMask;
    public GameObject releasePos1Preview;
    public GameObject releasePos2Preview;
    public float maxTouchMovementToValidatePos;

    private int currentSonoRemaining;
    private PatMar patMar;
    private Vector2 releasePos1;
    private Vector2 releasePos2;
    [HideInInspector] public bool hasChosen1stPos;
    private bool hasReleased1stBuoy;
    [HideInInspector] public bool isChoosingTrapPositions;
    private bool isReleasingSonoflashTrap;
    private Vector2 startTouchPos;
    private Vector2 touchMovement;
    private SonoFlashTrap lastReleasedTrap;
    private RaycastHit touchHit;

    [Header("Airport Settings")]
    public float flyTimeAvailable;
    public float timeToReloadAtAirport;
    public float rangeAirport;
    public GameObject airport;
    public Image fuelLevel;

    private bool dontBackToAirport;
    private bool hadComeBackManually;
    private float flyTimeRemaining;

    void Start()
    {
        patMar = GetComponent<PatMar>();
        currentSonoRemaining = maxSonoCapacity;
        flyTimeRemaining = flyTimeAvailable;
    }
    void Update()
    {
        UpdateSonoFlashRelease();

        UpdateFuelLevel();
    }

    private void UpdateSonoFlashRelease()
    {
        if (InputDuo.tapDown && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            startTouchPos = InputDuo.touch.position;
            touchHit = InputDuo.SeaRaycast(surfaceMask, !GameManager.useMouseControl);
        }
        else
        {
            touchMovement = Vector2.one * 100;
        }
        if (InputDuo.tapUp)
        {
            touchMovement = startTouchPos - InputDuo.touch.position;
        }
        sonobuoyRemainingText.text = currentSonoRemaining.ToString();

        if (isChoosingTrapPositions)
        {
            if (batimentController.batimentSelected != patMar)
            {
                StopSonoFlashRelease();
            }
            else
            {
                if (hasChosen1stPos)
                {
                    releaseInfo2.SetActive(true);
                    releaseInfo1.SetActive(false);
                    maxDistanceBetweenBuoyPreview.SetActive(true);
                    maxDistanceBetweenBuoyPreview.transform.localScale = Vector3.one * maxDistanceBetweenSonoflash * 2;
                    maxDistanceBetweenBuoyPreview.transform.position = SeaCoord.GetFlatCoord(releasePos1) + Vector3.up * 0.05f;
                }
                else
                {
                    maxDistanceBetweenBuoyPreview.SetActive(false);
                    releaseInfo2.SetActive(false);
                    releaseInfo1.SetActive(true);
                }

                if (InputDuo.tapUp && touchMovement.magnitude < maxTouchMovementToValidatePos)
                {
                    if (touchHit.collider != null)
                    {
                        if (!hasChosen1stPos)
                        {
                            releasePos1 = SeaCoord.Planify(touchHit.point);
                            if(ZoneHandler.GetCurrentZone(releasePos1).depth != Zone.Depth.Land)
                            {
                                hasChosen1stPos = true;
                                releasePos1Preview.SetActive(true);
                                releasePos1Preview.transform.position = SeaCoord.GetFlatCoord(releasePos1) + Vector3.up * 0.05f;
                            }
                        }
                        else
                        {
                            releasePos2 = SeaCoord.Planify(touchHit.point);
                            if (Vector2.Distance(releasePos1, releasePos2) < maxDistanceBetweenSonoflash)
                            {
                                isReleasingSonoflashTrap = true;
                                isChoosingTrapPositions = false;
                                hasReleased1stBuoy = false;
                                releasePos2Preview.SetActive(true);
                                releasePos2Preview.transform.position = SeaCoord.GetFlatCoord(releasePos2) + Vector3.up * 0.05f;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            maxDistanceBetweenBuoyPreview.SetActive(false);
            releaseInfo2.SetActive(false);
            releaseInfo1.SetActive(false);
        }

        if (isReleasingSonoflashTrap)
        {
            hasChosen1stPos = false;
            if (!hasReleased1stBuoy)
            {
                patMar.currentDestination = releasePos1;
                if (Vector2.Distance(patMar.currentPosition, releasePos1) < distanceToRelease)
                {
                    lastReleasedTrap = Instantiate(sonoFlashTrapPrefab, SeaCoord.GetFlatCoord(releasePos1), Quaternion.identity).GetComponent<SonoFlashTrap>();
                    lastReleasedTrap.firstBuoyPos = releasePos1;
                    lastReleasedTrap.submarine = submarine;
                    lastReleasedTrap.pinHandler = pinHandler;
                    hasReleased1stBuoy = true;
                }
            }
            else
            {
                releasePos1Preview.SetActive(false);
                patMar.currentDestination = releasePos2;
                if (Vector2.Distance(patMar.currentPosition, releasePos2) < distanceToRelease)
                {
                    lastReleasedTrap.secondBuoyPos = releasePos2;
                    lastReleasedTrap.Activate();
                    StopSonoFlashRelease();
                    currentSonoRemaining--;
                    isReleasingSonoflashTrap = false;
                }
            }
        }
        else if(!isChoosingTrapPositions)
        {
            releasePos1Preview.SetActive(false);
            releasePos2Preview.SetActive(false);
        }
    }


    public void StartSonoflashTrapRelease()
    {
        if (currentSonoRemaining > 0 && !hasChosen1stPos && !isReleasingSonoflashTrap)
        {
            isChoosingTrapPositions = true;
            hasChosen1stPos = false;
            releaseInfoPanel.SetActive(true);
            standardActionInfo.SetActive(false);
        }
    }

    public void StopSonoFlashRelease()
    {
        releaseInfoPanel.SetActive(false);
        standardActionInfo.SetActive(true);
        hasChosen1stPos = false;
        isReleasingSonoflashTrap = false;
        isChoosingTrapPositions = false;
        maxDistanceBetweenBuoyPreview.SetActive(false);
    }

    private void UpdateFuelLevel()
    {
        if (patMar.canFly)
        {
            flyTimeRemaining -= Time.deltaTime;
            fuelLevel.fillAmount = flyTimeRemaining / flyTimeAvailable;

            if (flyTimeRemaining <= 0)
            {
                flyTimeRemaining = 0;
                BackToAirport();
            }

            if (Vector2.Distance(patMar.currentDestination, SeaCoord.Planify(airport.transform.position)) < rangeAirport)
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

    private void BackToAirport()
    {
        patMar.currentDestination = SeaCoord.Planify(airport.transform.position);
        if(!hadComeBackManually)
        {
            patMar.canChangeDestination = false;
        }

        if (patMar.reachedDest)
        {
            dontBackToAirport = true;
            patMar.canFly = false;
            patMar.patmarIsReloading = true;          
        }
    }

    private void ReloadPatmar()
    {
        patMar.currentPosition = SeaCoord.Planify(airport.transform.position);
        patMar.currentDestination = SeaCoord.Planify(airport.transform.position);
        patMar.canFly = false;
        patMar.canChangeDestination = false;
        currentSonoRemaining = maxSonoCapacity;

        fuelLevel.fillAmount = flyTimeRemaining / flyTimeAvailable;
        if (hadComeBackManually)
        {
            flyTimeRemaining += (1 /timeToReloadAtAirport) * flyTimeAvailable * Time.deltaTime;
        }
        else
        {
            flyTimeRemaining += (1 / timeToReloadAtAirport) * flyTimeAvailable * 2 * Time.deltaTime;
        }

        if (flyTimeRemaining >= flyTimeAvailable)
        {
            flyTimeRemaining = flyTimeAvailable;
            dontBackToAirport = false;
            patMar.canChangeDestination = true;
            patMar.patmarIsReloading = false;
            hadComeBackManually = false;
        }
    }
}
