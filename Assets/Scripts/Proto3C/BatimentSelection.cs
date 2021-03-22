using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatimentSelection : MonoBehaviour
{
    public List<Batiment> batiments;

    public static Batiment batimentSelected;
    public CameraController cameraController;
    public Transform selectionDisplay;
    public float doubleSelectTime;

    public LayerMask batimentMask;
    private HelicoController helicoController;
    private float timeSpendSinceLastDirectSelect;
    private Batiment lastBatimentDirectlySelected;
    private void Start()
    {
        helicoController = (HelicoController)batiments[2].batimentAction;
        for (int b = 0; b < batiments.Count; b++)
        {
            batiments[b].selectButtonAnim.originalPos = batiments[b].selectButtonAnim.rectTransform.anchoredPosition;
            batiments[b].actionPanelAnim.canvasGroup = batiments[b].actionPanelAnim.rectTransform.GetComponent<CanvasGroup>();
            SelectBatiment(batiments[b], true);
        }
        SelectBatiment(batiments[0], true);
    }

    private void Update()
    {
        selectionDisplay.position = SeaCoord.GetFlatCoord(batimentSelected.batimentMovement.currentPosition);

        if (BatimentAction.currentActionNumber == 0)
        {
            for (int b = 0; b < batiments.Count; b++)
            {
                if (batiments[b].selectCard.isHovered)
                {
                    SelectBatiment(batiments[b], true);
                }
            }
        }

        UpdateDirectSelect();


        helicoController.isSelected = batimentSelected == batiments[2];

    }

    RaycastHit hit;

    private void UpdateDirectSelect()
    {
        timeSpendSinceLastDirectSelect += Time.deltaTime;
        if(!UICard.pointerFocusedOnCard && !UICard.anyCardSelected && InputDuo.tapDown)
        {
            hit = InputDuo.SeaRaycast(batimentMask, !GameManager.useMouseControl);
            if(hit.collider != null)
            {
                for (int b = 0; b < batiments.Count; b++)
                {
                    if(hit.collider.transform.parent == batiments[b].batimentAction.transform)
                    {
                        if(lastBatimentDirectlySelected == batiments[b])
                        {
                            if(timeSpendSinceLastDirectSelect < doubleSelectTime)
                            {
                                batiments[b].batimentMovement.destinationCard.Select();
                            }
                        }
                        else
                        {
                            SelectBatiment(batiments[b], false);
                        }
                        lastBatimentDirectlySelected = batiments[b];
                        timeSpendSinceLastDirectSelect = 0;
                    }
                }
            }
        }
    }

    public void SelectBatiment(Batiment batiment, bool refocusCamera)
    {
        Batiment previousBatimentSelected = batimentSelected;
        batimentSelected = batiment;
        if (refocusCamera)
        {
            cameraController.camSeaFocusPoint = batiment.batimentMovement.currentPosition;
        }

        if(previousBatimentSelected != batimentSelected)
        {
            //Debug.Log("Select " + batiment.batimentAction.gameObject.name);
            StartCoroutine(batiment.actionPanelAnim.anim.Play(batiment.actionPanelAnim.rectTransform, batiment.actionPanelAnim.canvasGroup));
            StartCoroutine(batiment.selectButtonAnim.anim.Play(batiment.selectButtonAnim.rectTransform, null, batiment.selectButtonAnim.originalPos));
            batiment.actionPanelAnim.canvasGroup.blocksRaycasts = true;

            for (int b = 0; b < batiments.Count; b++)
            {
                if(batiments[b] != batiment && batiments[b] == previousBatimentSelected)
                {
                    StartCoroutine(batiments[b].actionPanelAnim.anim.PlayBackward(batiments[b].actionPanelAnim.rectTransform, batiments[b].actionPanelAnim.canvasGroup, true));
                    StartCoroutine(batiments[b].selectButtonAnim.anim.PlayBackward(batiments[b].selectButtonAnim.rectTransform, null, batiments[b].selectButtonAnim.originalPos, true));
                    batiments[b].actionPanelAnim.canvasGroup.blocksRaycasts = true;
                }
            }
        }
    }
}
