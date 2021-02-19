using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullSonar : MonoBehaviour
{

    public GameObject hullSonarMapDisplay;
    public float sonarMaxRange;
    [Range(0,100)] public float sonarMidRangeRatio;
    public float refreshRate;
    public Submarine submarine;
    public LayerMask underWaterMask;
    public List<GameObject> sonarLongHiglights;
    public List<GameObject> sonarShortHiglights;

    private FregateHandler fregateHandler;
    private Fregate fregate;
    private Vector2 directionToSubmarine;
    private float distanceToSubmarine;
    private float angleToSubmarine;

    // uw =  Under Water Object
    private Vector2 uwPosition;
    private Vector2 uwDirection;
    private float uwAngle;
    private float uwDistance;
    private bool[] shortQuartersActivated;
    private bool[] longQuartersActivated;
    private int nextQuarterToScan;
    private int nextQuarterToDisable;
    private float timeRemainingBeforeNextScan;

    void Start()
    {
        fregate = GetComponent<Fregate>();
        fregateHandler = GetComponent<FregateHandler>();
        shortQuartersActivated = new bool[4];
        longQuartersActivated = new bool[4];
    }

    void Update()
    {
        UpdateHullSonar();
    }

    private void UpdateHullSonar()
    {
        if(fregateHandler.isUsingHullSonar)
        {
            hullSonarMapDisplay.SetActive(true);
            hullSonarMapDisplay.transform.position = SeaCoord.GetFlatCoord(fregate.currentPosition) + Vector3.up * 0.001f;
            hullSonarMapDisplay.transform.localScale = Vector3.one * sonarMaxRange * 2;

            if(timeRemainingBeforeNextScan > 0)
            {
                timeRemainingBeforeNextScan -= Time.deltaTime;
            }
            else
            {
                ResetQuarter(nextQuarterToDisable);
                RefreshQuarter(nextQuarterToScan);
                timeRemainingBeforeNextScan = (1 / refreshRate) / 4;
                nextQuarterToScan++;
                if(nextQuarterToScan > 3)
                {
                    nextQuarterToScan = 0;
                }

                nextQuarterToDisable++;
                if (nextQuarterToDisable > 3)
                {
                    nextQuarterToDisable = 0;
                }
            }
        }
        else
        {
            ResetQuaters();
            nextQuarterToScan = 0;
            nextQuarterToDisable = 3;
            timeRemainingBeforeNextScan = 0;
            hullSonarMapDisplay.SetActive(false);
        }
    }

    private void RefreshAllQuarters()
    {
        ResetQuaters();
        Collider[] colliders = Physics.OverlapSphere(SeaCoord.GetFlatCoord(fregate.currentPosition), sonarMaxRange, underWaterMask);
        for (int i = 0; i < colliders.Length; i++)
        {

            /*directionToSubmarine = submarine.currentPosition - fregate.currentPosition;

            angleToSubmarine = Vector2.SignedAngle(Vector2.right, directionToSubmarine);
            distanceToSubmarine = Vector2.Distance(submarine.currentPosition, fregate.currentPosition);*/

            uwPosition = SeaCoord.Planify(colliders[i].transform.position);
            uwDirection = uwPosition - fregate.currentPosition;
            uwAngle = Vector2.SignedAngle(Vector2.right, uwDirection);
            uwDistance = Vector2.Distance(fregate.currentPosition, uwPosition);

            if (uwDistance <= sonarMaxRange)
            {
                int direction = 0;
                if (Mathf.Abs(uwAngle) >= 135)
                {
                    direction = 2;
                }
                if (Mathf.Abs(uwAngle) < 45)
                {
                    direction = 1;
                }
                if (Mathf.Abs(uwAngle) >= 45 && Mathf.Abs(uwAngle) < 135)
                {
                    if (uwAngle > 0)
                    {
                        direction = 1;
                    }
                    else
                    {
                        direction = 3;
                    }
                }
                bool isNear = uwDistance < sonarMaxRange * sonarMidRangeRatio / 100;

                if (isNear)
                {
                    shortQuartersActivated[direction] = true;
                }
                else
                {
                    Debug.Log("long");
                    longQuartersActivated[direction] = true;
                }
            }
        }

        for (int y = 0; y < 4; y++)
        {
            sonarShortHiglights[y].SetActive(shortQuartersActivated[y]);
            sonarLongHiglights[y].SetActive(longQuartersActivated[y]);
        }
    }


    private void RefreshQuarter(int quarter)
    {
        ResetQuarter(quarter);
        Collider[] colliders = Physics.OverlapSphere(SeaCoord.GetFlatCoord(fregate.currentPosition), sonarMaxRange, underWaterMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            bool isTargetValid = true;
            if (colliders[i].GetComponentInParent<Submarine>() != null)
            {
                if (submarine.isUnderThermocline)
                {
                    isTargetValid = false;
                }
            }

            if(isTargetValid)
            {
                uwPosition = SeaCoord.Planify(colliders[i].transform.position);
                uwDirection = uwPosition - fregate.currentPosition;
                uwAngle = Vector2.SignedAngle(Vector2.right, uwDirection);
                uwDistance = Vector2.Distance(fregate.currentPosition, uwPosition);

                if (uwDistance <= sonarMaxRange)
                {
                    int direction = 0;
                    if (Mathf.Abs(uwAngle) >= 135)
                    {
                        direction = 2;
                    }
                    if (Mathf.Abs(uwAngle) < 45)
                    {
                        direction = 0;
                    }
                    if (Mathf.Abs(uwAngle) >= 45 && Mathf.Abs(uwAngle) < 135)
                    {
                        if (uwAngle > 0)
                        {
                            direction = 1;
                        }
                        else
                        {
                            direction = 3;
                        }
                    }

                    if (direction == quarter)
                    {
                        bool isNear = uwDistance < sonarMaxRange * sonarMidRangeRatio / 100;

                        if (isNear)
                        {
                            shortQuartersActivated[direction] = true;
                        }
                        else
                        {
                            longQuartersActivated[direction] = true;
                        }
                    }
                }
            }
        }

        sonarShortHiglights[quarter].SetActive(shortQuartersActivated[quarter]);
        sonarLongHiglights[quarter].SetActive(longQuartersActivated[quarter]);
    }

    private void ResetQuarter(int quarter)
    {
        sonarShortHiglights[quarter].SetActive(false);
        sonarLongHiglights[quarter].SetActive(false);
        shortQuartersActivated[quarter] = false;
        longQuartersActivated[quarter] = false;
    }

    private void ResetQuaters()
    {
        for (int i = 0; i < 4; i++)
        {
            sonarShortHiglights[i].SetActive(false);
            sonarLongHiglights[i].SetActive(false);
            shortQuartersActivated[i] = false;
            longQuartersActivated[i] = false;
        }
    }
}
