﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FregateAction : BatimentAction
{
    public UICard helicopterCard;
    public UICard hullSonarCard;
    public TweeningAnimator hullSonarDescriptionAnim;

    private FregateMovement fregateMovement;
    private bool hullSonarDescriptionOpened;
    public override void Start()
    {
        base.Start();
        fregateMovement = (FregateMovement)batimentMovement;
        hullSonarDescriptionAnim.canvasGroup = hullSonarDescriptionAnim.rectTransform.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        hullSonarCardUpdate();
    }

    public void hullSonarCardUpdate()
    {
        if (hullSonarCard.isHovered && !hullSonarDescriptionOpened)
        {
            hullSonarDescriptionOpened = true;
            StartCoroutine(hullSonarDescriptionAnim.anim.Play(hullSonarDescriptionAnim.rectTransform, hullSonarDescriptionAnim.canvasGroup));
        }
        else if (!hullSonarCard.isHovered && hullSonarDescriptionOpened)
        {
            hullSonarDescriptionOpened = false;
            StartCoroutine(hullSonarDescriptionAnim.anim.PlayBackward(hullSonarDescriptionAnim.rectTransform, hullSonarDescriptionAnim.canvasGroup, true));
        }
    }

    public void UseHelicopter()
    {
        Debug.Log("aoiuzfb");
    }
}
