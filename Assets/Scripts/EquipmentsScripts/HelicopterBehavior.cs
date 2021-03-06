using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterBehavior : MonoBehaviour
{
    public float speed;
    public float helicopterRange;

    public Transform targetPoint;
    public Transform fregate;
    public GameObject rangeDisplay;
    public LayerMask surfaceLayer;

    private bool touchTag;
    private bool isMoving;
    private bool backToFregate;
    private bool cantControl;

    private Touch touch;
    private Camera mainCamera;

    private void Start()
    {
        targetPoint.position = transform.position;
        mainCamera = Camera.main;
        cantControl = fregate.GetComponent<FregateController>().cantControl;

        rangeDisplay.transform.localScale = new Vector2(helicopterRange * 2, helicopterRange * 2);
        rangeDisplay.SetActive(false);
    }

    private void Update()
    {
        //Debug for switch between fregate & helicopter
        switchControlDebug();

        if (cantControl && !isMoving)
        {
            rangeDisplay.SetActive(true);
            TouchPoint();
        }
        else
        {
            rangeDisplay.SetActive(false);
        }

        MoveHelicopter();

        rangeDisplay.transform.position = new Vector3(fregate.position.x, rangeDisplay.transform.position.y, fregate.position.z);
    }

    private void switchControlDebug()
    {
        if (Input.touchCount > 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);
            Touch touch3 = Input.GetTouch(2);

            if (touch1.phase == TouchPhase.Began && touch2.phase == TouchPhase.Began && touch3.phase == TouchPhase.Began)
            {
                cantControl = !cantControl;
                fregate.GetComponent<FregateController>().cantControl = cantControl;
            }
        }
    }

    private void TouchPoint()
    {
        if ((Input.GetButtonUp("LeftClick") || touch.phase == TouchPhase.Ended) && touchTag)
        {
            touchTag = false;
            if (!Statics.inMenu)
            {
                RaycastHit touchHit;
                Ray screenRay;
                if (Input.GetButtonUp("LeftClick"))
                {
                    screenRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                }
                else
                {

                    screenRay = mainCamera.ScreenPointToRay(touch.position);
                }

                if (Physics.Raycast(screenRay, out touchHit, 200f, surfaceLayer))
                {
                    float distance = Vector2.Distance(SeaCoord.Planify(new Vector3(touchHit.point.x, 0, touchHit.point.z)), SeaCoord.Planify(transform.position));

                    if (distance < helicopterRange)
                    {
                        targetPoint.position = new Vector3(touchHit.point.x, 0, touchHit.point.z);
                    }
                }
            }
        }

        if (!Statics.inMenu)
        {
            if (Input.GetButtonDown("LeftClick"))
                touchTag = true;

            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    touchTag = true;
                }
            }
        }
    }

    private void MoveHelicopter()
    {
        if (transform.position != targetPoint.position)
        {
            isMoving = true;
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, Time.deltaTime * speed);
            transform.LookAt(targetPoint.position);           
        }
        else
        {           
            backToFregate = true;

            if (isMoving)
            {
                Debug.Log("reach Destination Point");
                //Launch here Sonar flash for finish the game
            }
        }

        if (backToFregate)
        {
            targetPoint.position = fregate.position;
        }

        if (transform.position == fregate.position)
        {
            backToFregate = false;
            isMoving = false;
        }

        if (!isMoving)
        {
            transform.position = fregate.position;
            targetPoint.position = fregate.position;
        }
    }
}
