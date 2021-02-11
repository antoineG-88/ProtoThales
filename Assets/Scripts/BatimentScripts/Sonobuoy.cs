using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonobuoy : MonoBehaviour
{
    public float timeBetweenScans;
    public float scanRange;
    public GameObject scanEffectPrefab;
    public GameObject warnScanEffectPrefab;
    public Transform submarine;

    private float timeRemainingBeforeNextScan;

    void Start()
    {
        timeRemainingBeforeNextScan = timeBetweenScans;
    }


    void Update()
    {
        if(timeRemainingBeforeNextScan > 0)
        {
            timeRemainingBeforeNextScan -= Time.deltaTime;
        }
        else
        {
            timeRemainingBeforeNextScan = timeBetweenScans;
            Scan();
        }
    }

    private void Scan()
    {
        float distance = Vector2.Distance(SeaCoord.Planify(submarine.position), SeaCoord.Planify(transform.position));
        if(distance > scanRange)
        {
            Instantiate(scanEffectPrefab, SeaCoord.GetFlatCoord(transform.position), Quaternion.identity);
        }
        else
        {
            Instantiate(warnScanEffectPrefab, SeaCoord.GetFlatCoord(transform.position), Quaternion.identity);
        }

    }
}
