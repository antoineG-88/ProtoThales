using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MadBehavior : MonoBehaviour
{
    [Header("Sonobuys in Scene")]
    public List<GameObject> sonobuoys = new List<GameObject>();

    [Header("Detected objects")]
    public GameObject[] objectsCanBeDetected;
    public Sprite[] objectsCanBeDetectedSprite;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
