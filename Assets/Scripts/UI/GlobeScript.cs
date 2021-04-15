using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeScript : MonoBehaviour
{
    public float rotateSpeed;
    public float smoothSpeed;
    public Transform targetTransform;

    [HideInInspector] public bool canTurn;

    private Vector2 previousMousePosition;
    private bool inverseControl;

    void Start()
    {
        canTurn = true;
        targetTransform.position = transform.position;
        targetTransform.rotation = transform.rotation;
    }

    void Update()
    {
        GetMouseMovement();
        MoveGlobe();
    }

    private void MoveGlobe()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, smoothSpeed);
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

                targetTransform.Rotate(Vector3.up, -mouseMovement.x, Space.Self);
                targetTransform.Rotate(Vector3.right, mouseMovement.y, Space.World);

                //targetTransform.rotation = Quaternion.Euler(Mathf.Clamp(GetNormAngle(targetTransform.rotation.eulerAngles.x), -90, 90), targetTransform.rotation.eulerAngles.y, Mathf.Clamp(GetNormAngle(targetTransform.rotation.eulerAngles.z), -90, 90));
                //targetTransform.LookAt(targetTransform.position + targetTransform.forward, Vector3.up);
                previousMousePosition = Input.mousePosition;
            }
        }
    }


    private float GetNormAngle(float angle)
    {
        float newAngle = angle;

        if (angle > 180)
        {
            newAngle = angle - 360;
        }

        if (angle <= -180)
        {
            newAngle = angle + 360;
        }
        return newAngle;
    }
}