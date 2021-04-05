using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatMarAction : BatimentAction
{
    public SonobuoyBehavior sonobuoyPrefab;
    public int sonobuoyMaxCharge;
    public float sonobuoyRechargeTime;
    public Image sonobuoyRechargeFill;
    public Text currentSonobuoyChargeText;
    public UICard sonobuoyCard;
    public UICard madCard;
    public TweeningAnimator madDescriptionAnim;
    public GameObject dropPosPreview;
    public LayerMask seaMask;
    public AudioClip noChargeSound;
    public AudioClip selectSonobuoySound;
    public AudioClip showInfoSound;

    private PatMarMovement patMarMovement;
    private MadBehavior madBehavior;
    private bool isDroppingSonobuoy;
    private Vector2 droppingPos;
    private bool madDescriptionOpened;
    private bool isChoosingDropPos;
    private int currentSonobuoyCharge;
    private float timeBeforeNextSonobuoy;
    private bool sonoSelectedFlag;

    public override void Start()
    {
        base.Start();
        patMarMovement = (PatMarMovement)batimentMovement;
        madBehavior = GetComponent<MadBehavior>();
        madDescriptionAnim.canvasGroup = madDescriptionAnim.rectTransform.GetComponent<CanvasGroup>();
        currentSonobuoyCharge = sonobuoyMaxCharge;
        timeBeforeNextSonobuoy = 0;
        sonoSelectedFlag = true;
    }

    public override void Update()
    {
        SonobuoyUpdate();
        MadCardUpdate();
    }

    public void MadCardUpdate()
    {
        if(madCard.isHovered && !madDescriptionOpened)
        {
            madDescriptionOpened = true;
            StartCoroutine(madDescriptionAnim.anim.Play(madDescriptionAnim.rectTransform, madDescriptionAnim.canvasGroup));
            BatimentSelection.PlaySound(showInfoSound);
        }
        else if(!madCard.isHovered && madDescriptionOpened)
        {
            madDescriptionOpened = false;
            StartCoroutine(madDescriptionAnim.anim.PlayBackward(madDescriptionAnim.rectTransform, madDescriptionAnim.canvasGroup, true));
        }
    }

    private void DropSonobuoy()
    {
        SonobuoyBehavior newSonobuoy = Instantiate(sonobuoyPrefab.gameObject, SeaCoord.GetFlatCoord(droppingPos), Quaternion.identity).GetComponent<SonobuoyBehavior>();
        madBehavior.sonobuoys.Add(newSonobuoy);
        newSonobuoy.madScript = madBehavior;
        currentSonobuoyCharge--;
    }

    public void SonobuoyUpdate()
    {
        if(isDroppingSonobuoy)
        {
            if(Vector2.Distance(patMarMovement.currentPosition, droppingPos) < 0.1f)
            {
                isDroppingSonobuoy = false;
                DropSonobuoy();
            }
            if(droppingPos != patMarMovement.currentDestination)
            {
                isDroppingSonobuoy = false;
            }

            dropPosPreview.transform.position = SeaCoord.GetFlatCoord(droppingPos);
        }

        isDoingAction = sonobuoyCard.isFocused;

        if (((sonobuoyCard.isSelected && InputDuo.tapUp) || (sonobuoyCard.isDropped && !sonobuoyCard.isCursorOn)) && !sonobuoyCard.descriptionOpened && isChoosingDropPos)
        {
            droppingPos = SeaCoord.Planify(InputDuo.SeaRaycast(seaMask, !GameManager.useMouseControl).point);
            if (TerrainZoneHandler.GetCurrentZone(droppingPos, null).relief != TerrainZone.Relief.Land)
            {
                isChoosingDropPos = false;
                isDroppingSonobuoy = true;
                patMarMovement.currentDestination = droppingPos;
                dropPosPreview.SetActive(true);
                sonobuoyCard.Deselect();
            }
        }

        if(currentSonobuoyCharge > 0)
        {
            if(sonobuoyCard.isSelected || sonobuoyCard.isDragged)
            {
                if(sonoSelectedFlag)
                {
                    sonoSelectedFlag = false;
                    BatimentSelection.PlaySound(selectSonobuoySound);
                }
            }
            else
            {
                sonoSelectedFlag = true;
            }

            sonobuoyCard.canBeSelected = true;
            if (((sonobuoyCard.isSelected && InputDuo.tapHold) || ((sonobuoyCard.isDragged && !sonobuoyCard.isHovered) || sonobuoyCard.isDropped)) && !sonobuoyCard.descriptionOpened)
            {
                isChoosingDropPos = true;
                dropPosPreview.SetActive(true);
                dropPosPreview.transform.position = SeaCoord.GetFlatCoord(InputDuo.SeaRaycast(seaMask, !GameManager.useMouseControl).point);
            }
            else if (!isDroppingSonobuoy)
            {
                isChoosingDropPos = false;
                dropPosPreview.SetActive(false);
            }
        }
        else
        {
            if (sonobuoyCard.isClicked || sonobuoyCard.isDropped)
            {
                BatimentSelection.PlaySound(noChargeSound);
            }
            sonobuoyCard.canBeSelected = false;
            isChoosingDropPos = false;
            dropPosPreview.SetActive(false);
        }

        if(currentSonobuoyCharge < sonobuoyMaxCharge)
        {
            if(timeBeforeNextSonobuoy >= sonobuoyRechargeTime)
            {
                currentSonobuoyCharge++;
                timeBeforeNextSonobuoy = 0;
            }
            else
            {
                timeBeforeNextSonobuoy += Time.deltaTime;
            }
        }

        sonobuoyRechargeFill.fillAmount = currentSonobuoyCharge == sonobuoyMaxCharge ? 1 : (timeBeforeNextSonobuoy / sonobuoyRechargeTime);
        currentSonobuoyChargeText.text = currentSonobuoyCharge.ToString();
    }
}
