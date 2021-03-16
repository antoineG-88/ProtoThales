using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatMarAction : BatimentAction
{
    public SonobuoyBehavior sonobuoyPrefab;
    public UICard sonobuoyCard;
    public UICard madCard;
    public TweeningAnimator madDescriptionAnim;
    public GameObject dropPosPreview;
    public LayerMask seaMask;

    private PatMarMovement patMarMovement;
    private MadBehavior madBehavior;
    private bool isDroppingSonobuoy;
    private Vector2 droppingPos;
    private bool madDescriptionOpened;
    private bool isChoosingDropPos;

    public override void Start()
    {
        base.Start();
        patMarMovement = (PatMarMovement)batimentMovement;
        madBehavior = GetComponent<MadBehavior>();
        madDescriptionAnim.canvasGroup = madDescriptionAnim.rectTransform.GetComponent<CanvasGroup>();
    }

    void Update()
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
        }
        else if(!madCard.isHovered && madDescriptionOpened)
        {
            madDescriptionOpened = false;
            StartCoroutine(madDescriptionAnim.anim.PlayBackward(madDescriptionAnim.rectTransform, madDescriptionAnim.canvasGroup, true));
        }
    }

    public void SonobuoyUpdate()
    {
        if(isDroppingSonobuoy)
        {
            if(Vector2.Distance(patMarMovement.currentPosition, droppingPos) < 0.1f)
            {
                isDroppingSonobuoy = false;
                SonobuoyBehavior newSonobuoy = Instantiate(sonobuoyPrefab.gameObject, SeaCoord.GetFlatCoord(droppingPos), Quaternion.identity).GetComponent<SonobuoyBehavior>();
                madBehavior.sonobuoys.Add(newSonobuoy);
                newSonobuoy.madScript = madBehavior;
            }
            if(droppingPos != patMarMovement.currentDestination)
            {
                isDroppingSonobuoy = false;
            }

            dropPosPreview.transform.position = SeaCoord.GetFlatCoord(droppingPos);
        }

        isDraggingAction = sonobuoyCard.isFocused;


        if (sonobuoyCard.isDropped && !sonobuoyCard.isHovered && !sonobuoyCard.descriptionOpened && isChoosingDropPos)
        {
            isChoosingDropPos = false;
            isDroppingSonobuoy = true;
            droppingPos = SeaCoord.Planify(InputDuo.SeaRaycast(seaMask, true).point);
            patMarMovement.currentDestination = droppingPos;
        }

        if (((sonobuoyCard.isDragged && !sonobuoyCard.isHovered) || sonobuoyCard.isDropped) && !sonobuoyCard.descriptionOpened)
        {
            isChoosingDropPos = true;
            dropPosPreview.SetActive(true);
            dropPosPreview.transform.position = SeaCoord.GetFlatCoord(InputDuo.SeaRaycast(seaMask, true).point);
        }
        else if (!isDroppingSonobuoy)
        {
            isChoosingDropPos = false;
            dropPosPreview.SetActive(false);
        }
    }
}
