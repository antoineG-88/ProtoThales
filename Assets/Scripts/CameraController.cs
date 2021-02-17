using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 startTouch;
    public BatimentController batimentControllerScript;

    [Header("Zoom Settings")]
    public float camMinVerticalDistance;
    public float camMaxVerticalDistance;
    public float camIsometricMaxOffset;
    public float camIsometricMinOffset;

    [Space]
    public float mouseZoomSpeed;
    public float touchZoomSpeed;
    public float lerpMoveRatio;

    private Touch touch;
    private Camera mainCamera;
    private Vector3 touchMovement;

    private bool downTag;
    private bool startTouchRegistered;
    private Vector2 camSeaFocusPoint;
    private float currentZoom;
    private Vector2 currentFocusPoint;

    private void Start()
    {
        mainCamera = Camera.main;
        camSeaFocusPoint = Vector2.zero;
        currentZoom = camMaxVerticalDistance;
    }

    void Update()
    {
        if (Input.touchCount < 2 && !batimentControllerScript.isDragingDest)
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
                    camSeaFocusPoint += SeaCoord.Planify(touchMovement);

                    //Limit Camera movement 
                    transform.position = new Vector3(Mathf.Clamp(transform.position.x, -13, 13), transform.position.y, Mathf.Clamp(transform.position.z, -46, 14));
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

        Zoom(Input.GetAxis("Mouse ScrollWheel") * mouseZoomSpeed);


        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPreviousPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePreviousPos = touchOne.position - touchOne.deltaPosition;

            float previousMagnitude = (touchZeroPreviousPos - touchOnePreviousPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - previousMagnitude;

            Zoom(difference * touchZoomSpeed * 0.001f);
        }
    }

    void Zoom(float increment)
    {

        currentZoom -= increment;

        currentZoom = Mathf.Clamp(currentZoom, 0f, 1f);


        //currentFocusPoint = Vector3.Lerp(currentFocusPoint, camSeaFocusPoint, lerpMoveRatio * Time.deltaTime);

        mainCamera.transform.position = SeaCoord.GetFlatCoord(camSeaFocusPoint) + Vector3.up * (camMinVerticalDistance + (currentZoom * (camMaxVerticalDistance - camMinVerticalDistance))) + new Vector3(0,0, -(camIsometricMinOffset + (currentZoom * (camIsometricMaxOffset - camIsometricMinOffset))));


        mainCamera.transform.LookAt(SeaCoord.GetFlatCoord(camSeaFocusPoint));
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
