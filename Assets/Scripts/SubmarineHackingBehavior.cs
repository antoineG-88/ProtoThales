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
    [HideInInspector] public int currentWaypointsHacked;
    public AudioClip progressHackSound;
    public AudioClip defeatSound;

    public SubmarineMoveHandler submarineMoveHandler;
    private bool looseFlag;

    private void Start()
    {
        numberOfWaypoints = submarineMoveHandler.allWaypoints.Count;
        lifeBar.fillAmount = 0;
    }

    private void Update()
    {
        if(currentWaypointsHacked == submarineMoveHandler.allWaypoints.Count && !looseFlag)
        {
            looseFlag = false;
            losePanel.SetActive(true);
            BatimentSelection.PlaySound(defeatSound);
        }
    }

    public void FinishHack()
    {
        currentWaypointsHacked++;
        lifeBar.fillAmount += 1f / numberOfWaypoints;
        BatimentSelection.PlaySound(progressHackSound);
    }
}
