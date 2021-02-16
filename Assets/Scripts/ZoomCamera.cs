using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    private Vector3 startTouch;
    public BatimentController batimentControllerScript;

    [Header("Zoom Settings")]
    public float zoomMin;
    public float zoomMax;
    public float rotateMin;
    public float rotateMax;

    [Space]
    public float MouseZoomSpeed;
    public float touchZoomSpeed;
    public float rotationSpeed;

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
        if (batimentControllerScript.batimentSelected == null)
        {
            /// for keyboard
            if (InputDuo.tapDown)
            {
                startTouch = GetSeaPosition(!Input.GetButton("LeftClick"));
            }

            if (InputDuo.tapHold)
            {
                touchMovement = startTouch - GetSeaPosition(!Input.GetButton("LeftClick"));
                mainCamera.transform.position += touchMovement;

                //Limit Camera movement 
                //transform.position = new Vector3(Mathf.Clamp(transform.position.x, -13, 13), Mathf.Clamp(transform.position.y, 33, 47), Mathf.Clamp(transform.position.z, -46, 14));
            }
            /// 


            /*if (Input.touchCount < 2)
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
            }*/

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
        Camera.main.transform.position += new Vector3(0, -1, 0) * increment;
        Camera.main.transform.eulerAngles += new Vector3(-1, 0, 0) * increment * rotationSpeed;

        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Mathf.Clamp(Camera.main.transform.position.y, zoomMin, zoomMax), Camera.main.transform.position.z);
        Camera.main.transform.eulerAngles = new Vector3(Mathf.Clamp(Camera.main.transform.eulerAngles.x, rotateMin, rotateMax), Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);
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
