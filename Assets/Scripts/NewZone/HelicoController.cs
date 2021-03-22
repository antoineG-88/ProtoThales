using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelicoController : BatimentAction
{
    public Transform upPropeller;
    public Transform sidePropeller;
    public float spinSpeed;
    public float focusZoom;
    public float prepareTime;
    public float maxActiveTime;
    public float flashChargeTime;
    public float timeTresholdBeforeStartFlashCharge;
    public float flashCooldown;
    public float flashRange;
    public GameObject flashEffect;
    public Image flashCooldownDisplay;
    public RectTransform releaseFlashPanel;
    public RectTransform releaseFlashPreview;
    public Image releaseFlashChargeDisplay;
    public Vector2 releaseFlashDisplayOffset;
    public Image activeFill;
    public Image bigActiveFill;
    public Color activeColor;
    public Color preparingColor;
    public GameObject helicoDisplay;
    public GameObject helicoOnFregateDisplay;
    public CameraController cameraController;
    public FregateMovement fregateMovement;
    public Transform submarine;

    [HideInInspector] public bool isSelected;
    private float currentCharge;
    private float currentActiveTimeRemaining;
    private bool isPreparing;
    private float previousZoom;
    private bool isActive;
    private bool isHelicoOnFregate;
    private HelicoMovement helicoMovement;
    private float currentFlashCharge;
    private float timeImmobile;
    private float currentFlashCooldownRemaining;
    private Camera mainCamera;
    private Vector3 viewPortPos;
    public override void Start()
    {
        base.Start();
        mainCamera = Camera.main;
        helicoMovement = GetComponent<HelicoMovement>();
        isHelicoOnFregate = true;
        releaseFlashPreview.gameObject.SetActive(false);
        bigActiveFill.fillAmount = 0;
        activeFill.fillAmount = 0;
    }

    public override void Update()
    {
        SpinPropellers();
        Behavior();

        if (isDoingAction && !doingActionFlag)
        {
            doingActionFlag = true;
            currentActionNumber++;
        }
        else if (!isDoingAction && doingActionFlag)
        {
            doingActionFlag = false;
            currentActionNumber--;
        }

        helicoDisplay.SetActive(!isHelicoOnFregate);
        helicoOnFregateDisplay.SetActive(isHelicoOnFregate);
        helicoMovement.isControllable = !isHelicoOnFregate;
        if (isHelicoOnFregate)
        {
            helicoMovement.currentPosition = fregateMovement.currentPosition;
        }
    }

    void SpinPropellers()
    {
        upPropeller.rotation = Quaternion.Euler(upPropeller.rotation.eulerAngles.x, upPropeller.rotation.eulerAngles.y + spinSpeed * Time.deltaTime, upPropeller.rotation.eulerAngles.z);
        sidePropeller.rotation *= Quaternion.AngleAxis(spinSpeed * Time.deltaTime, Vector3.up);
    }



    void Behavior()
    {
        if (currentFlashCooldownRemaining > 0)
        {
            currentFlashCooldownRemaining -= Time.deltaTime;
            flashCooldownDisplay.fillAmount = 1 - (currentFlashCooldownRemaining / flashCooldown);
        }
        else
        {
            flashCooldownDisplay.fillAmount = 1;
        }

        if (isActive)
        {
            if (isSelected)
            {
                cameraController.currentZoom = focusZoom;


                if (!UICard.anyCardSelected && InputDuo.tapHold && !InputDuo.tapDown)
                {
                    isHelicoOnFregate = false;
                    cameraController.MoveCameraWithEdge();
                    isDoingAction = true;
                    helicoMovement.currentDestination = SeaCoord.Planify(InputDuo.SeaRaycast(helicoMovement.seaMask, !GameManager.useMouseControl).point);

                    if(currentFlashCooldownRemaining <= 0)
                    {
                        if (helicoMovement.reachedDest)
                        {
                            timeImmobile += Time.deltaTime;
                            if (timeImmobile > timeTresholdBeforeStartFlashCharge)
                            {
                                releaseFlashPreview.gameObject.SetActive(true);
                                viewPortPos = mainCamera.WorldToViewportPoint(SeaCoord.GetFlatCoord(helicoMovement.currentPosition));
                                releaseFlashPreview.anchoredPosition = new Vector2((viewPortPos.x - 0.5f) * releaseFlashPanel.sizeDelta.x,
                                       (viewPortPos.y - 0.5f) * releaseFlashPanel.sizeDelta.y);
                                releaseFlashPreview.anchoredPosition += releaseFlashDisplayOffset;

                                currentFlashCharge += Time.deltaTime;
                                releaseFlashChargeDisplay.fillAmount = currentFlashCharge / flashChargeTime;

                                if (currentFlashCharge > flashChargeTime)
                                {
                                    currentFlashCooldownRemaining = flashCooldown;
                                    DropFlashSonic();
                                }
                            }
                        }
                        else
                        {
                            releaseFlashPreview.gameObject.SetActive(false);
                            timeImmobile = 0;
                            currentFlashCharge = 0;
                        }
                    }
                    else
                    {
                        releaseFlashPreview.gameObject.SetActive(false);
                    }
                }
                else
                {
                    releaseFlashPreview.gameObject.SetActive(false);
                    isDoingAction = false;
                }
            }
            else
            {
                isDoingAction = false;
                releaseFlashPreview.gameObject.SetActive(false);
            }

            if (currentActiveTimeRemaining > 0)
            {
                currentActiveTimeRemaining -= Time.deltaTime;
                activeFill.color = activeColor;
                activeFill.fillAmount = currentActiveTimeRemaining / maxActiveTime;
                bigActiveFill.color = activeColor;
                bigActiveFill.fillAmount = currentActiveTimeRemaining / maxActiveTime;
            }
            else
            {
                isActive = false;
                activeFill.fillAmount = 0;
                bigActiveFill.fillAmount = 0;
            }
        }
        else
        {
            releaseFlashPreview.gameObject.SetActive(false);
            isDoingAction = false;
            if (!isHelicoOnFregate && !isPreparing)
            {
                helicoMovement.currentDestination = fregateMovement.currentPosition;
                helicoMovement.UpdateHasReachedDest();
                if(helicoMovement.reachedDest)
                {
                    isHelicoOnFregate = true;
                }
            }
            else if (isPreparing)
            {
                if (currentCharge < prepareTime)
                {
                    activeFill.color = preparingColor;
                    activeFill.fillAmount = currentCharge / prepareTime;
                    bigActiveFill.color = preparingColor;
                    bigActiveFill.fillAmount = currentCharge / prepareTime;
                    currentCharge += Time.deltaTime;
                }
                else
                {
                    activeFill.fillAmount = 0;
                    bigActiveFill.fillAmount = 0;
                    isActive = true;
                    isPreparing = false;
                    currentActiveTimeRemaining = maxActiveTime;
                }
            }
            else
            {
                activeFill.fillAmount = 0;
                bigActiveFill.fillAmount = 0;
            }
        }
    }

    public void PrepareHelicopter()
    {
        if (!isActive && !isPreparing && isHelicoOnFregate)
        {
            isPreparing = true;
            currentCharge = 0;
        }
    }

    private void DropFlashSonic()
    {
        float distanceSubmarine = Vector2.Distance(SeaCoord.Planify(submarine.position), helicoMovement.currentPosition);

        Instantiate(flashEffect, SeaCoord.GetFlatCoord(helicoMovement.currentPosition), Quaternion.identity);
        if (distanceSubmarine < flashRange)
        {
            Debug.Log("You Win");
            GameManager.Win();
        }
    }
}
