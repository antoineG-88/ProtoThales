using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Batiment
{
    public BatimentAction batimentAction;
    public BatimentMovement batimentMovement;
    public TweeningAnimator actionPanelAnim;
    public TweeningAnimator selectButtonAnim;
    public UICard selectCard;
}
