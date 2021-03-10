using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeScript : MonoBehaviour
{
    public float cameraDistance;
    public float rotateSpeed;
    public float smoothSpeed;

    [HideInInspector] public bool canTurn;

    private Camera mainCamera;
    private Quaternion targetRotation;
    private Vector3 targetEulerRotation;
    private Vector2 previousMousePosition;
    private bool inverseControl;

    void Start()
    {
        canTurn = true;
        targetRotation = transform.rotation;
        targetEulerRotation = transform.rotation.eulerAngles;
        mainCamera = Camera.main;
        mainCamera.transform.localPosition = new Vector3(cameraDistance, 0, 0);
        mainCamera.transform.LookAt(transform.position, Vector3.up);
    }

    void Update()
    {
        GetMouseMovement();
        MoveCamera();
    }

    private void MoveCamera()
    {
        //Vector3 currentRotation = Vector3.Lerp(transform.rotation.eulerAngles, targetEulerRotation, smoothSpeed);
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetEulerRotation), smoothSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothSpeed);
    }

    private void GetMouseMovement()
    {
        if(canTurn)
        {
            if (InputDuo.tapHold)
            {
                if (InputDuo.tapDown)
                {
                    previousMousePosition = Input.mousePosition;
                }
                Vector2 mouseMovement = (Vector2)Input.mousePosition - previousMousePosition;
                mouseMovement *= rotateSpeed;

                targetRotation *= Quaternion.AngleAxis(-mouseMovement.x, Vector3.up);

                targetEulerRotation.y += -mouseMovement.x;

                previousMousePosition = Input.mousePosition;
            }
        }
    }
}