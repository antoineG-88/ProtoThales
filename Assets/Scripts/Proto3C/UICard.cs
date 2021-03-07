using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICard : MonoBehaviour
     , IPointerClickHandler
     , IDragHandler
     , IPointerEnterHandler
     , IPointerExitHandler
     , IDropHandler
     , IEndDragHandler
{
    public static bool pointerFocusedOnCard;
    private static List<UICard> allCards = new List<UICard>();
    public static TweeningAnimator darkBackAnim;

    [HideInInspector] public bool isDragged;
    [HideInInspector] public bool isDropped;
    [HideInInspector] public bool isHovered;
    [HideInInspector] public bool isFocused;
    [HideInInspector] public bool isClicked;
    public TweeningAnimator dragAnim;
    public TweeningAnimator holdAnim;
    public bool darkenBackWhileHold;

    private int dropCount;
    private int clickCount;
    private bool dragFlag;
    private float holdTime;
    [HideInInspector] public bool descriptionOpened;

    public static void UpdateFocusCard()
    {
        pointerFocusedOnCard = false;
        for (int i = 0; i < allCards.Count; i++)
        {
            if(allCards[i].isFocused)
                pointerFocusedOnCard = allCards[i].isFocused;
        }
    }

    private void Start()
    {
        dragFlag = true;
        allCards.Add(this);
    }

    private void Update()
    {
        isHovered = isHovered && Input.touchCount > 0;
        isFocused = isHovered || isDragged;

        if (dropCount > 0)
        {
            dropCount--;
            isDropped = true;
        }
        else
        {
            isDropped = false;
        }

        if (clickCount > 0)
        {
            clickCount--;
            isClicked = true;
        }
        else
        {
            isClicked = false;
        }

        if(isHovered)
        {
            holdTime += Time.deltaTime;
        }
        if(Input.touchCount == 0)
        {
            holdTime = 0;
        }

        if(holdTime > 0.8f && !descriptionOpened)
        {
            descriptionOpened = true;
            StopAllCoroutines();

            if(darkenBackWhileHold)
                StartCoroutine(darkBackAnim.anim.Play(darkBackAnim.rectTransform, darkBackAnim.canvasGroup));

            if (holdAnim.rectTransform != null)
                StartCoroutine(holdAnim.anim.Play(holdAnim.rectTransform, null));
        }

        if(holdTime <= 0 && descriptionOpened)
        {
            descriptionOpened = false;

            if (darkenBackWhileHold)
                StartCoroutine(darkBackAnim.anim.PlayBackward(darkBackAnim.rectTransform, darkBackAnim.canvasGroup, true));

            if (holdAnim.rectTransform != null)
                StartCoroutine(holdAnim.anim.PlayBackward(holdAnim.rectTransform, null, true));
        }


        if(dragFlag && isHovered && !descriptionOpened)
        {
            if (dragAnim.rectTransform != null)
                StartCoroutine(dragAnim.anim.Play(dragAnim.rectTransform, null));
            dragFlag = false;
        }
    }

    #region   InterfaceEvents
    public void OnPointerClick(PointerEventData eventData)
    {
        clickCount = 2;
    }

    public void OnDrag(PointerEventData eventData)
    {
        isDragged = true;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (!dragFlag && !isDragged)
        {
            dragFlag = true;
            if(!descriptionOpened)
            {
                if (dragAnim.rectTransform != null)
                    StartCoroutine(dragAnim.anim.PlayBackward(dragAnim.rectTransform, null, true));
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragged = false;
        if(!dragFlag)
        {
            dragFlag = true;
            if (dragAnim.rectTransform != null)
                StartCoroutine(dragAnim.anim.PlayBackward(dragAnim.rectTransform, null, true));
        }
        dropCount = 2;
    }
#endregion
}
