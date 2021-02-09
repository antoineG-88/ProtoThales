﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineMovement : MonoBehaviour
{
    [Header("Submarine Movement")]
    public float submarineSpeed;
    public int submarineWaypoints;
    public Transform[] allWaypoints;

    private List<int> randomWaypoints;
    private List<Transform> waypointsToGo;
    
    private Transform nextPosition;
    private int nextPointIndex;

    private void Start()
    {
        randomWaypoints = new List<int>(new int[allWaypoints.Length]);
        waypointsToGo = new List<Transform>();

        for (int i = 0; i < submarineWaypoints; i++)                                    //pick up x random numbers without repeat
        {
            int random = Random.Range(0, (allWaypoints.Length)+1);
            while (randomWaypoints.Contains(random))
            {
                random = Random.Range(0, (allWaypoints.Length)+1);
            }
            randomWaypoints[i] = random;
            waypointsToGo.Add(allWaypoints[random-1]);
        }

        nextPosition = waypointsToGo[0];
    }

    private void FixedUpdate()
    {
        MoveSubmarine();
    }

    void MoveSubmarine()
    {
        if (transform.position == nextPosition.position)
        {
            nextPointIndex++;
            if (nextPointIndex >= waypointsToGo.Count)
            {
                nextPointIndex = 0;
            }
            nextPosition = waypointsToGo[nextPointIndex];
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPosition.position, Time.fixedDeltaTime * submarineSpeed);
        }
    }
}

