using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TweeningAnimator darkBackAnim;

    void Start()
    {
        darkBackAnim.canvasGroup = darkBackAnim.rectTransform.GetComponent<CanvasGroup>();
        UICard.darkBackAnim = darkBackAnim;
    }

    void Update()
    {
        UICard.UpdateFocusCard();
    }
}
