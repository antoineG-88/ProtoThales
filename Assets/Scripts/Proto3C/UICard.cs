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
    [HideInInspector] public bool isDraging;
    [HideInInspector] public bool isDropped;
    [HideInInspector] public bool isHovered;
    [HideInInspector] public bool isFocused;
    [HideInInspector] public bool isClicked;
    public TweeningAnimator dragAnim;

    private int dropCount;
    private int clickCount;
    private bool dragFlag;

    private void Start()
    {
        dragFlag = true;
    }

    private void Update()
    {
        if(dropCount > 0)
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

        isFocused = isHovered || isDraging;

        if(dragFlag && isHovered)
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
        isDraging = true;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (!dragFlag && !isDraging)
        {
            dragFlag = true;
            if(dragAnim.rectTransform != null)
                StartCoroutine(dragAnim.anim.PlayBackward(dragAnim.rectTransform, null, true));
        }
    }

    public void OnDrop(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDraging = false;
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
