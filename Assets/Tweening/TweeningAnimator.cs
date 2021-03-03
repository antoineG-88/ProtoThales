using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TweeningAnimator
{
    public TweeningAnim anim;
    public RectTransform rectTransform;
    [HideInInspector] public CanvasGroup canvasGroup;
}
