using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineVigilanceBehavior : MonoBehaviour
{
    public float vigilanceValue = 0f;
    public float submarineDetectionRange;

    [Header("Debug")]
    public bool enableRangeDisplay;
    public GameObject rangeDisplay;
    private SpriteRenderer rangeSprite;

    private bool refreshRange;
    private float timer;

    private void Start()
    {
        rangeDisplay.SetActive(false);
        rangeDisplay.transform.localScale = new Vector2(submarineDetectionRange * 2, submarineDetectionRange * 2);
        rangeSprite = rangeDisplay.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        RefreshEachSecond();
        EnableDebugRange();
    }

    private void RefreshEachSecond()
    {
        if (timer >= 1)
        {
            refreshRange = true;
            timer = 0;
        }
        else
        {
            refreshRange = false;
            timer += Time.deltaTime;
        }
    }

    private void IncreaseVigilanceBar(float valuePerSecond)
    {
        if (refreshRange)
        {
            vigilanceValue += valuePerSecond;
        }
    }

    private void EnableDebugRange()
    {
        if (enableRangeDisplay)
        {
            rangeDisplay.SetActive(true);
        }
        else
        {
            rangeDisplay.SetActive(false);
        }
    } 
}
