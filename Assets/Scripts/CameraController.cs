using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 startTouch;
    //public BatimentController batimentControllerScript;
    public BatimentSelection batimentSelection;
    public Transform camTargetTransform;

    [Header("Zoom Settings")]
    public float camMinVerticalDistance;
    public float camMaxVerticalDistance;
    public float camIsometricMaxOffset;
    public float camIsometricMinOffset;
    public Vector2 minMaxVerticalBounds;
    public Vector2 minMaxHorizontalBounds;

    [Space]
    public float mouseZoomSpeed;
    public float touchZoomSpeed;
    public float lerpMoveRatio;

    private Touch touch;
    private Camera mainCamera;
    private Vector3 touchMovement;

    private bool downTag;
    private bool startTouchRegistered;
    [HideInInspector] public Vector2 camSeaFocusPoint;
    [HideInInspector] public float currentZoom;
    [HideInInspector] public Vector2 currentFocusPoint;

    private void Start()
    {
        mainCamera = Camera.main;
        camSeaFocusPoint = Vector2.zero;
        currentZoom = camMaxVerticalDistance;
        seaPlane = new Plane(Vector3.up, new Vector3(0, 0, 0));
    }

    void Update()
    {
        if (Input.touchCount < 2 && !UICard.pointerFocusedOnCard)
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
                    camSeaFocusPoint = new Vector2(Mathf.Clamp(camSeaFocusPoint.x, minMaxHorizontalBounds.x, minMaxHorizontalBounds.y), Mathf.Clamp(camSeaFocusPoint.y, minMaxVerticalBounds.x, minMaxVerticalBounds.y));
                    //Limit Camera movement 
                    //transform.position = new Vector3(Mathf.Clamp(transform.position.x, -13, 13), transform.position.y, Mathf.Clamp(transform.position.z, -46, 14));
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
            downTag = false;
        }

        RefreshCamPos(Input.GetAxis("Mouse ScrollWheel") * mouseZoomSpeed);


        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPreviousPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePreviousPos = touchOne.position - touchOne.deltaPosition;

            float previousMagnitude = (touchZeroPreviousPos - touchOnePreviousPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - previousMagnitude;

            RefreshCamPos(difference * touchZoomSpeed * 0.001f);
        }
    }

    void RefreshCamPos(float zoomIncrement)
    {

        currentZoom -= zoomIncrement;

        currentZoom = Mathf.Clamp(currentZoom, 0f, 1f);


        //currentFocusPoint = Vector2.Lerp(currentFocusPoint, camSeaFocusPoint, lerpMoveRatio * Time.deltaTime);

        mainCamera.transform.position = SeaCoord.GetFlatCoord(camSeaFocusPoint) + Vector3.up * (camMinVerticalDistance + (currentZoom * (camMaxVerticalDistance - camMinVerticalDistance))) + new Vector3(0,0, -(camIsometricMinOffset + (currentZoom * (camIsometricMaxOffset - camIsometricMinOffset))));
        //camTargetTransform.position = SeaCoord.GetFlatCoord(camSeaFocusPoint) + Vector3.up * (camMinVerticalDistance + (currentZoom * (camMaxVerticalDistance - camMinVerticalDistance))) + new Vector3(0, 0, -(camIsometricMinOffset + (currentZoom * (camIsometricMaxOffset - camIsometricMinOffset))));
        //camTargetTransform.transform.LookAt(SeaCoord.GetFlatCoord(camSeaFocusPoint));
        mainCamera.transform.LookAt(SeaCoord.GetFlatCoord(camSeaFocusPoint));
        //mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, camTargetTransform.position, lerpMoveRatio * Time.deltaTime);
        //mainCamera.transform.rotation = camTargetTransform.rotation;
    }

    private Plane seaPlane;

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
        float distance;
        seaPlane.Raycast(touchRay, out distance);
        return touchRay.GetPoint(distance);
    }
}
