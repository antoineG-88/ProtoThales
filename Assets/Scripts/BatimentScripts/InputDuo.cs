using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputDuo : MonoBehaviour
{
    public static Touch touch;
    public static bool tapDown;
    public static bool tapUp;
    public static bool tapHold;

    public static Camera mainCamera;

    private bool isTouching;
    private void Start()
    {
        mainCamera = Camera.main;
        touch.phase = TouchPhase.Canceled;
    }

    void Update()
    {
        if(Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
        }

        tapDown = (touch.phase == TouchPhase.Began) || Input.GetButtonDown("LeftClick");
        //tapUp = touch.phase == TouchPhase.Ended || Input.GetButtonUp("LeftClick");
        tapHold = touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved || Input.GetButton("LeftClick");

        if(tapUp)
        {
            tapUp = false;
        }

        if(isTouching)
        {
            if(!tapHold)
            {
                isTouching = false;
                tapUp = true;
            }
        }
        else
        {
            isTouching = tapHold;
        }
    }

    public static RaycastHit SeaRaycast(LayerMask mask, bool isTouch)
    {
        RaycastHit[] touchHits;
        Ray screenRay;
        if (!isTouch)
        {
            screenRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            screenRay = mainCamera.ScreenPointToRay(touch.position);
        }


        touchHits = Physics.RaycastAll(screenRay, 200f, mask);
        if (touchHits.Length > 0)
        {
            return touchHits[0];
        }

        return new RaycastHit();
    }
}
