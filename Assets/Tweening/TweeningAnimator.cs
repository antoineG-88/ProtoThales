using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TweeningAnimator
{
    public TweeningAnim anim;
    public RectTransform rectTransform;
    [HideInInspector] public CanvasGroup canvasGroup;
    [HideInInspector] public Vector2 originalPos;

    public void GetCanvasGroup()
    {
        canvasGroup = rectTransform.GetComponent<CanvasGroup>();
    }
}
