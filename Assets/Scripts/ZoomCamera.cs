using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    private Vector3 startTouch;

    [Header("Zoom Settings")]
    public float zoomMin;
    public float zoomMax;

    [Space]
    public float MouseZoomSpeed;
    public float touchZoomSpeed;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //startTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            startTouch = GetWorldPoisition(0);
        }

        if (Input.GetMouseButton(0))
        {
            //Vector3 direction = startTouch - Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector3 direction = startTouch - GetWorldPoisition(0);
            Camera.main.transform.position += direction;

            //Limit Camera movement 
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -13, 13), Mathf.Clamp(transform.position.y, 33, 47), Mathf.Clamp(transform.position.z, -46, 14));
        }
        else if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPreviousPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePreviousPos = touchOne.position - touchOne.deltaPosition;

            float previousMagnitude = (touchZeroPreviousPos - touchOnePreviousPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - previousMagnitude;

            zoom(difference * touchZoomSpeed);
        }

        zoom(Input.GetAxis("Mouse ScrollWheel") * MouseZoomSpeed);

        
    }

    //ortographic cam
    void Zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomMin, zoomMax);
    }

    //perspective cam
    void zoom(float increment)
    {
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - increment, zoomMin, zoomMax);
    }

    private Vector3 GetWorldPoisition(float z)
    {
        Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Camera.main.transform.forward, new Vector3(0, 0, z));
        float distance;
        ground.Raycast(mousePos, out distance);
        return mousePos.GetPoint(distance);
    }
}
