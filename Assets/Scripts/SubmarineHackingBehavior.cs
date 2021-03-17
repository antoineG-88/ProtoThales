using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmarineHackingBehavior : MonoBehaviour
{
    [Header("UI")]
    public Image lifeBar;
    private int numberOfWaypoints;

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
            lifeBar.fillAmount += 1f / numberOfWaypoints;
        }
    }
}
