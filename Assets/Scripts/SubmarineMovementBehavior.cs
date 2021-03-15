using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineMovementBehavior : MonoBehaviour
{
    [Header("Submarine Movement")]
    public float submarineSpeed;
    public int submarineWaypoints;
    public float waitTimeAtPoint;

    [Space]
    public List<Transform> allWaypoints;

    [Space]
    public Transform nextPosition;

    private float timer;
    private int countWaypointsAchieved;

    private void Start()
    {
        int random = Random.Range(0, (allWaypoints.Count) + 1);
        nextPosition = allWaypoints[random];
        allWaypoints.RemoveAt(random);
    }

    private void Update()
    {
        MoveSubmarine();
    }

    void MoveSubmarine()
    {
        if (countWaypointsAchieved < submarineWaypoints)
        {
            if (transform.position == nextPosition.position)
            {
                timer += Time.deltaTime;

                if (timer >= waitTimeAtPoint)
                {
                    countWaypointsAchieved++;

                    int random = Random.Range(0, (allWaypoints.Count) + 1);
                    nextPosition = allWaypoints[random];
                    allWaypoints.RemoveAt(random);
                }
            }
            else
            {
                timer = 0;

                transform.position = Vector3.MoveTowards(transform.position, nextPosition.position, Time.fixedDeltaTime * submarineSpeed);
            }
        }       
    }
}
