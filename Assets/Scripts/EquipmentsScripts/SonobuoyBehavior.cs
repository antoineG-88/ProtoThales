﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonobuoyBehavior : MonoBehaviour
{
    [Header("Effect")]
    public GameObject scanEffectPrefab;
    public GameObject warnScanEffectPrefab;
    public SpriteRenderer identifyImage;
    public float effectFrequency;
    private float timerBeforeEffect;

    [Header("Sonobuoy")]
    public float sonobuoyRange;
    private float distance;
    public float sonobuoyLifeTime;
    private float timeBeforeDestroy;

    [Header("Range")]
    public GameObject rangeDisplay;
    private SpriteRenderer rangeSprite;

    public Color sonobuoyNoElement;
    public Color sonobuoyElementDetect;

    public List<GameObject> objectInsideRange = new List<GameObject>();


    private GameObject[] objectsCanBeDetected;
    private Sprite[] objectsCanBeDetectedSprite;

    private bool flagObjectInsideRange;
    private bool madIsAboveSonobuoy;

    private MadBehavior madScript;

    private void Start()
    {
        madScript = GameObject.FindGameObjectWithTag("Mad").GetComponent<MadBehavior>();
        madScript.sonobuoys.Add(gameObject);
        objectsCanBeDetected = madScript.objectsCanBeDetected;
        objectsCanBeDetectedSprite = madScript.objectsCanBeDetectedSprite;

        timeBeforeDestroy = sonobuoyLifeTime;
        timerBeforeEffect = effectFrequency;

        rangeDisplay.transform.localScale = new Vector2(sonobuoyRange * 2, sonobuoyRange * 2);
        rangeSprite = rangeDisplay.GetComponent<SpriteRenderer>();

        identifyImage.sprite = null;
    }

    private void Update()
    {
        LifeTime();
        Scan();
        Effect();
    }

    private void LifeTime()
    {
        if (timeBeforeDestroy > 0)
        {
            timeBeforeDestroy -= Time.deltaTime;
        }
        else
        {
            madScript.sonobuoys.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    private void Scan()
    {
        for (int i = 0; i < objectsCanBeDetected.Length; i++)
        {
            distance = Vector2.Distance(SeaCoord.Planify(objectsCanBeDetected[i].transform.position), SeaCoord.Planify(transform.position));

            if (distance < sonobuoyRange)
            {
                if (!flagObjectInsideRange)
                {
                    flagObjectInsideRange = true;
                    rangeSprite.color = sonobuoyElementDetect;
                    objectInsideRange.Add(objectsCanBeDetected[i]);
                }

                MadAbove();
                if (madIsAboveSonobuoy)
                {
                    identifyImage.sprite = objectsCanBeDetectedSprite[i];
                }
                else
                {
                    identifyImage.sprite = null;
                }
            }
            else
            {
                identifyImage.sprite = null;
                flagObjectInsideRange = false;
                rangeSprite.color = sonobuoyNoElement;
                objectInsideRange.Remove(objectsCanBeDetected[i]);
            }
        }
    }

    private void MadAbove()
    {
        float distanceFromMad = Vector2.Distance(SeaCoord.Planify(madScript.gameObject.transform.position), SeaCoord.Planify(transform.position));

        if (distanceFromMad < sonobuoyRange)
        {
            madIsAboveSonobuoy = true;
        }
        else
        {
            madIsAboveSonobuoy = false;
        }
    }

    private void Effect()
    {
        if (timerBeforeEffect > 0)
        {
            timerBeforeEffect -= Time.deltaTime;
        }
        else
        {
            timerBeforeEffect = effectFrequency;

            if (distance < sonobuoyRange)
            {
                Instantiate(warnScanEffectPrefab, SeaCoord.GetFlatCoord(transform.position), Quaternion.identity);
            }
            else
            {
                Instantiate(scanEffectPrefab, SeaCoord.GetFlatCoord(transform.position), Quaternion.identity);
            }
        }
    }
}
