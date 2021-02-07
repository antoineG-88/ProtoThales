using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FregateController : MonoBehaviour
{
    public float[] maxSpeeds;
    public float accelerationForce;
    public float deccelerationForce;
    public float turnSpeed;
    public float slowDownDistance;
    [Space]
    public Transform targetTransform;
    public LayerMask surfaceLayer;
    public ParticleSystem thrustParticle;

    private FregateHandler fregateHandler;
    private Camera mainCamera;
    private float currentSpeed;
    [HideInInspector] public Vector2 currentDirection;
    [HideInInspector] public float currentAngle;
    private bool isAccelerating;
    [HideInInspector] public Vector2 targetDirection;
    private int currentTurnSide;
    private bool touchTag;
    private Touch touch;
    private void Start()
    {
        fregateHandler = GetComponent<FregateHandler>();
        mainCamera = Camera.main;
        currentDirection = Vector2.one;
        currentAngle = 45;
        currentSpeed = 0;
    }

    private void Update()
    {


        if ((Input.GetButtonUp("LeftClick") || touch.phase == TouchPhase.Ended) && touchTag)
        {
            touchTag = false;
            if(!Statics.inMenu)
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
                    targetTransform.position = new Vector3(touchHit.point.x, 0, touchHit.point.z);
                }
            }
        }

        if (!Statics.inMenu)
        {
            if(Input.GetButtonDown("LeftClick"))
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


        isAccelerating = Vector3.Distance(Planify(transform.position), Planify(targetTransform.position)) > slowDownDistance;
        targetDirection = new Vector2(targetTransform.position.x - transform.position.x, targetTransform.position.z - transform.position.z);
        if(thrustParticle.isPlaying)
        {
            if(!isAccelerating)
            {
                thrustParticle.Stop();
            }
        }
        else
        {
            if(isAccelerating)
            {
                thrustParticle.Play();
            }
        }
    }

    private void FixedUpdate()
    {
        if(isAccelerating)
        {
            currentSpeed += accelerationForce * Time.fixedDeltaTime;

            if (Vector2.Angle(currentDirection, targetDirection) > Time.fixedDeltaTime * turnSpeed)
            {
                currentTurnSide = Vector2.SignedAngle(currentDirection, targetDirection) > 0 ? 1 : -1;
                currentAngle = Vector2.SignedAngle(Vector2.right, currentDirection) + currentTurnSide * Time.fixedDeltaTime * turnSpeed;
            }
            else
            {
                currentDirection = targetDirection;
            }
        }
        else
        {
            currentSpeed -= deccelerationForce * Time.fixedDeltaTime;
        }


        currentDirection = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, -currentAngle + 90, transform.rotation.eulerAngles.z);

        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeeds[fregateHandler.numberOnVitesse]);
        transform.position = transform.position + Planify(currentDirection) * Time.fixedDeltaTime * currentSpeed;
    }


    #region usefull
    private Vector3 Planify(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    private Vector3 Planify(Vector2 vector)
    {
        return new Vector3(vector.x, 0, vector.y);
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if(Application.isPlaying)
        {
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), 0, Mathf.Sin(currentAngle * Mathf.Deg2Rad)) * 5);
        }
    }
}
