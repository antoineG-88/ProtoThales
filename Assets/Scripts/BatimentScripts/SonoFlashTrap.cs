using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonoFlashTrap : MonoBehaviour
{
    public float timeBetweenScans;
    public GameObject scanEffectPrefab;
    public GameObject warnScanEffectPrefab;
    public float alertCooldown;
    public float submarineImmobilizedTime;
    public Transform firstBuoy;
    public Transform secondBuoy;
    [HideInInspector] public Submarine submarine;
    [HideInInspector] public PinHandler pinHandler;
    public float lifeSpan;

    [HideInInspector] public Vector2 firstBuoyPos;
    [HideInInspector] public Vector2 secondBuoyPos;
    private float distanceBetweenBuoy;
    private Vector2 directionFromFirst;
    private float timeRemainingBeforeNextScan;
    private float timeRemainingBeforeNewAlert;
    [HideInInspector] public bool isOperational;
    void Start()
    {
        Activate();
        timeRemainingBeforeNextScan = timeBetweenScans;
        isOperational = false;
        StartCoroutine(AutoDestroy());
    }

    void Update()
    {
        if(isOperational)
        {
            if (timeRemainingBeforeNextScan > 0)
            {
                timeRemainingBeforeNextScan -= Time.deltaTime;
            }
            else
            {
                timeRemainingBeforeNextScan = timeBetweenScans;
                Scan();
            }

            CheckSubmarinePassingThrough();
        }
    }

    public void Activate()
    {
        firstBuoy.position = SeaCoord.GetFlatCoord(firstBuoyPos) - Vector3.up * 0.12f;
        secondBuoy.position = SeaCoord.GetFlatCoord(secondBuoyPos) - Vector3.up * 0.12f;
        distanceBetweenBuoy = Vector2.Distance(firstBuoyPos, secondBuoyPos);
        directionFromFirst = secondBuoyPos - firstBuoyPos;
        directionFromFirst.Normalize();
        isOperational = true;
    }

    private void Scan()
    {
        Instantiate(scanEffectPrefab, SeaCoord.GetFlatCoord(firstBuoyPos) + Vector3.up * 0.01f, Quaternion.identity);
        Instantiate(scanEffectPrefab, SeaCoord.GetFlatCoord(secondBuoyPos) + Vector3.up * 0.01f, Quaternion.identity);
    }

    private void AlertSubmarine()
    {
        Instantiate(warnScanEffectPrefab, SeaCoord.GetFlatCoord(firstBuoyPos) + Vector3.up * 0.01f, Quaternion.identity);
        Instantiate(warnScanEffectPrefab, SeaCoord.GetFlatCoord(secondBuoyPos) + Vector3.up * 0.01f, Quaternion.identity);

        StartCoroutine(submarine.Immobilize(submarineImmobilizedTime));

        pinHandler.CreateSonoFlashAlertPin(firstBuoyPos + directionFromFirst * distanceBetweenBuoy / 2);
    }

    private void CheckSubmarinePassingThrough()
    {

        if(timeRemainingBeforeNewAlert > 0)
        {
            timeRemainingBeforeNewAlert -= Time.deltaTime;
        }
        else
        {
            float scalar1 = Cross.GetScalar(directionFromFirst * distanceBetweenBuoy, firstBuoyPos, submarine.currentDirection * 0.1f, submarine.currentPosition);
            float scalar2 = Cross.GetScalar(submarine.currentDirection * 0.1f, submarine.currentPosition, directionFromFirst * distanceBetweenBuoy, firstBuoyPos);
            if (scalar1 >= 0 && scalar1 <= 1 && scalar2 >= 0 && scalar2 <= 1)
            {
                AlertSubmarine();
                timeRemainingBeforeNewAlert = alertCooldown;
            }
        }
    }

    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(gameObject);
    }
}
