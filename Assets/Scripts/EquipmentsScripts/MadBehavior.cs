using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MadBehavior : MonoBehaviour
{
    [Header("Sonobuys in Scene")]
    public List<SonobuoyBehavior> sonobuoys = new List<SonobuoyBehavior>();

    [Header("Detected objects")]
    public List<GameObject> objectsCanBeDetected;
    public Sprite[] objectsCanBeDetectedSprite;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
