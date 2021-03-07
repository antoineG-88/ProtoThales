using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullSonarBehavior : MonoBehaviour
{
    [Header ("Sweep")]
    public float sweepSpeed;
    public float sonarDistanceDetection;
    public LayerMask surfaceLayer;

    [Space]
    public GameObject sonarDisplay;
    public Transform sweep;
    public Transform sweepEndPoint;
    public Transform ping;

    private List<Collider> colliderObjectSonar;
    private Vector3 collision = Vector3.zero;

    private void Start()
    {
        colliderObjectSonar = new List<Collider>();
    }

    private void Update()
    {
        SonarEnable();
    }

    private void SonarEnable()
    {
        sonarDisplay.SetActive(true);
        sonarDisplay.transform.position = new Vector3(transform.position.x, sonarDisplay.transform.position.y, transform.position.z);

        SweepSonnar();
    }

    private void SonarDisable()
    {
        sonarDisplay.SetActive(false);
    }

    private void SweepSonnar()
    {
        float previousRotation = (sweep.eulerAngles.y % 360) - 180;
        sweep.eulerAngles -= new Vector3(0, 0, Time.deltaTime * sweepSpeed);
        float currentRotation = (sweep.eulerAngles.y % 360) - 180;

        if (previousRotation < 0 && currentRotation >= 0)
        {
            colliderObjectSonar.Clear();
        }

        Vector3 direction = (sweepEndPoint.position - sonarDisplay.transform.position).normalized;
        //Debug.DrawLine(sonarDisplay.transform.position, sonarDisplay.transform.position + direction * sonarDistanceDetection, Color.red);

        RaycastHit[] raycastHitArray = Physics.RaycastAll(sonarDisplay.transform.position, direction, sonarDistanceDetection, surfaceLayer);
        foreach(RaycastHit raycastHit in raycastHitArray)
        {
            if(raycastHit.collider != null)
            {
                if (!colliderObjectSonar.Contains(raycastHit.collider))
                {
                    colliderObjectSonar.Add(raycastHit.collider);
                    //collision = hit.point;
                    Instantiate(ping, raycastHit.point, ping.rotation);
                }
            }            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(collision, 0.2f);
    }
}
