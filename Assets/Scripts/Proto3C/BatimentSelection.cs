using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatimentSelection : MonoBehaviour
{
    public FregateAction fregateAction;
    public PatMarAction patMarAction;
    public FregateMovement fregateMovement;
    public PatMarMovement patMarMovement;
    public static BatimentAction batimentSelected;
    public CameraController cameraController;
    public Transform selectionDisplay;
    public TweeningAnimator fregateActionPanelAnim;
    public TweeningAnimator patMarActionPanelAnim;
    public TweeningAnimator fregateSelectAnim;
    public TweeningAnimator patMarSelectAnim;
    public UICard fregateSelectCard;
    public UICard patMarSelectCard;
    public LayerMask batimentMask;

    private void Start()
    {
        fregateActionPanelAnim.canvasGroup = fregateActionPanelAnim.rectTransform.GetComponent<CanvasGroup>();
        patMarActionPanelAnim.canvasGroup = patMarActionPanelAnim.rectTransform.GetComponent<CanvasGroup>();
        SelectBatiment(fregateAction, true);
    }

    private void Update()
    {
        selectionDisplay.position = SeaCoord.GetFlatCoord(batimentSelected.batimentMovement.currentPosition);
        if(fregateSelectCard.isHovered)
        {
            SelectBatiment(fregateAction, true);
        }
        if(patMarSelectCard.isHovered)
        {
            SelectBatiment(patMarAction, true);
        }

        UpdateDirectSelect();
    }

    RaycastHit hit;

    private void UpdateDirectSelect()
    {
        if(!UICard.pointerFocusedOnCard && !UICard.anyCardSelected && InputDuo.tapDown)
        {
            hit = InputDuo.SeaRaycast(batimentMask, true);
            if(hit.collider != null)
            {
                if(hit.collider.transform.parent == fregateAction.transform)
                {
                    SelectBatiment(fregateAction, false);
                }
                else if (hit.collider.transform.parent == patMarAction.transform)
                {
                    SelectBatiment(patMarAction, false);
                }
            }
        }
    }

    public void SelectBatiment(BatimentAction batiment, bool refocusCamera)
    {
        BatimentAction previousBatimentSelected = batimentSelected;
        batimentSelected = batiment;
        if (refocusCamera)
        {
            cameraController.camSeaFocusPoint = batiment.batimentMovement.currentPosition;
        }

        if(previousBatimentSelected != batimentSelected)
        {
            if (batiment == fregateAction)
            {
                StartCoroutine(fregateActionPanelAnim.anim.Play(fregateActionPanelAnim.rectTransform, fregateActionPanelAnim.canvasGroup));
                StartCoroutine(patMarActionPanelAnim.anim.PlayBackward(patMarActionPanelAnim.rectTransform, patMarActionPanelAnim.canvasGroup, true));
                StartCoroutine(fregateSelectAnim.anim.Play(fregateSelectAnim.rectTransform, null));
                StartCoroutine(patMarSelectAnim.anim.PlayBackward(patMarSelectAnim.rectTransform, null, true));
                patMarActionPanelAnim.canvasGroup.blocksRaycasts = false;
                fregateActionPanelAnim.canvasGroup.blocksRaycasts = true;
            }
            else if(batiment == patMarAction)
            {
                StartCoroutine(patMarActionPanelAnim.anim.Play(patMarActionPanelAnim.rectTransform, patMarActionPanelAnim.canvasGroup));
                StartCoroutine(fregateActionPanelAnim.anim.PlayBackward(fregateActionPanelAnim.rectTransform, fregateActionPanelAnim.canvasGroup, true));
                StartCoroutine(patMarSelectAnim.anim.Play(patMarSelectAnim.rectTransform, null));
                StartCoroutine(fregateSelectAnim.anim.PlayBackward(fregateSelectAnim.rectTransform, null, true));
                patMarActionPanelAnim.canvasGroup.blocksRaycasts = true;
                fregateActionPanelAnim.canvasGroup.blocksRaycasts = false;
            }
        }
    }
}
