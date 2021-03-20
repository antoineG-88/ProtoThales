using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmarineHackingBehavior : MonoBehaviour
{
    [Header("UI")]
    public Image lifeBar;
    public GameObject losePanel;
    private int numberOfWaypoints;
    private int currentWaypointsHacked;

    public SubmarineMovementBehavior submarineMovementScript;

    private void Start()
    {
        numberOfWaypoints = submarineMovementScript.submarineWaypoints;
        lifeBar.fillAmount = 0;
    }

    private void Update()
    {
        if (submarineMovementScript.waypointHacked)
        {
            currentWaypointsHacked++;
            lifeBar.fillAmount += 1f / numberOfWaypoints;
        }

        if(currentWaypointsHacked == submarineMovementScript.submarineWaypoints)
        {
            losePanel.SetActive(true);
        }
    }
}
