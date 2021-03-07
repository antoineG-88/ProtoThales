using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullSonarBehavior : MonoBehaviour
{
    public GameObject sonarDisplay;

    private void Start()
    {
        
    }

    private void Update()
    {
        SonarEnable();
    }

    private void SonarEnable()
    {
        sonarDisplay.SetActive(true);
        sonarDisplay.transform.position = new Vector3(transform.position.x, sonarDisplay.transform.position.y, transform.position.z);
    }

    private void SonarDisable()
    {
        sonarDisplay.SetActive(false);
    }
}
