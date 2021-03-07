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

    private void Start()
    {
        fregateActionPanelAnim.canvasGroup = fregateActionPanelAnim.rectTransform.GetComponent<CanvasGroup>();
        patMarActionPanelAnim.canvasGroup = patMarActionPanelAnim.rectTransform.GetComponent<CanvasGroup>();
        SelectBatiment(true);
    }

    private void Update()
    {
        selectionDisplay.position = SeaCoord.GetFlatCoord(batimentSelected.batimentMovement.currentPosition);
        if(fregateSelectCard.isHovered)
        {
            SelectBatiment(true);
        }
        if(patMarSelectCard.isHovered)
        {
            SelectBatiment(false);
        }
    }


    public void SelectBatiment(bool selectFregate)
    {
        BatimentAction previousBatimentSelected = batimentSelected;
        batimentSelected = selectFregate ? (BatimentAction)fregateAction : patMarAction;
        cameraController.camSeaFocusPoint = selectFregate ? fregateMovement.currentPosition : patMarMovement.currentPosition;
        if(previousBatimentSelected != batimentSelected)
        {
            if (selectFregate)
            {
                StartCoroutine(fregateActionPanelAnim.anim.Play(fregateActionPanelAnim.rectTransform, fregateActionPanelAnim.canvasGroup));
                StartCoroutine(patMarActionPanelAnim.anim.PlayBackward(patMarActionPanelAnim.rectTransform, patMarActionPanelAnim.canvasGroup, true));
                StartCoroutine(fregateSelectAnim.anim.Play(fregateSelectAnim.rectTransform, null));
                StartCoroutine(patMarSelectAnim.anim.PlayBackward(patMarSelectAnim.rectTransform, null, true));
                patMarActionPanelAnim.canvasGroup.blocksRaycasts = false;
                fregateActionPanelAnim.canvasGroup.blocksRaycasts = true;
            }
            else
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
