using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineIA : MonoBehaviour
{
    public SubmarinePath path;
    public float speed;
    public float alertSpeed;
    public float alertDistanceBigSonar;
    public float bigSonarAlertTime;
    public float alertDistanceSonar;
    public float sonarAlertTime;

    private List<Transform> pathPos;
    private Vector3 currentDirection;
    private int nextPathPos;
    private float alertRemainingTime;
    private float currentSpeed;
    void Start()
    {
        pathPos = new List<Transform>();
        for(int i = 0; i < path.transform.childCount; i++)
        {
            pathPos.Add(path.transform.GetChild(i));
        }
        int random = Random.Range(0, pathPos.Count - 1);
        transform.position = pathPos[random].position;
        nextPathPos = random + 1;
        alertRemainingTime = 0;
    }

    void Update()
    {
        if (alertRemainingTime > 0)
        {
            currentSpeed = alertSpeed;
            alertRemainingTime -= Time.deltaTime;
        }
        else
        {
            currentSpeed = speed;
        }

        if (Vector3.Distance(pathPos[nextPathPos].position, transform.position) < Time.fixedDeltaTime * currentSpeed)
        {
            nextPathPos++;
            if(nextPathPos > pathPos.Count -1)
            {
                nextPathPos = 0;
            }
        }
    }

    public void WarnSubmarine(float distance, bool bigEmission)
    {
        if(bigEmission && distance < alertDistanceBigSonar)
        {
            alertRemainingTime += bigSonarAlertTime;
        }


        if (!bigEmission && distance < alertDistanceSonar)
        {
            alertRemainingTime += sonarAlertTime;
        }
    }



    private void FixedUpdate()
    {
        currentDirection = new Vector3(pathPos[nextPathPos].position.x - transform.position.x, 0, pathPos[nextPathPos].position.z - transform.position.z);
        currentDirection.Normalize();
        transform.position = transform.position + currentDirection * Time.fixedDeltaTime * currentSpeed;
    }
}
