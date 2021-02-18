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

    void Start()
    {
        patMar = GetComponent<PatMar>();
        currentSonoRemaining = maxSonoCapacity;
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
}
