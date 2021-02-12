using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    private Vector3 startTouch;
    public BatimentController batimentControllerScript;

    [Header("Zoom Settings")]
    public float zoomMin;
    public float zoomMax;

    [Space]
    public float MouseZoomSpeed;
    public float touchZoomSpeed;

    private Touch touch;
    private Camera mainCamera;
    private Vector3 touchMovement;

    private bool downTag;
    private bool startTouchRegistered;
    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if(batimentControllerScript.batimentSelected == null)
        {
            if(Input.touchCount < 2)
            {
                if (downTag)
                {
                    downTag = false;
                    startTouchRegistered = true;
                    startTouch = GetSeaPosition(!Input.GetButton("LeftClick"));
                }

                if (InputDuo.tapDown)
                {
                    downTag = true;
                }

                if (InputDuo.tapHold)
                {
                    if (startTouchRegistered)
                    {
                        touchMovement = startTouch - GetSeaPosition(!Input.GetButton("LeftClick"));
                        mainCamera.transform.position += touchMovement;

                        //Limit Camera movement 
                        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -13, 13), Mathf.Clamp(transform.position.y, 33, 47), Mathf.Clamp(transform.position.z, -46, 14));
                    }
                }
                else
                {
                    startTouchRegistered = false;
                }
            }
            else
            {
                startTouchRegistered = false;
            }

            Zoom(Input.GetAxis("Mouse ScrollWheel") * MouseZoomSpeed);


            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPreviousPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePreviousPos = touchOne.position - touchOne.deltaPosition;

                float previousMagnitude = (touchZeroPreviousPos - touchOnePreviousPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - previousMagnitude;

                Zoom(difference * touchZoomSpeed);
            }
        }
        else
        {
            downTag = false;
            startTouchRegistered = false;
        }
    }

    //perspective cam
    void Zoom(float increment)
    {
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - increment, zoomMin, zoomMax);
    }

    private Vector3 GetSeaPosition(bool isTouch)
    {
        Ray touchRay;
        if(isTouch && Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            touchRay = mainCamera.ScreenPointToRay(touch.position);
        }
        else
        {
           touchRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        }
        Plane ground = new Plane(Vector3.up, new Vector3(0, 0, 0));
        float distance;
        ground.Raycast(touchRay, out distance);
        return touchRay.GetPoint(distance);
    }
}
