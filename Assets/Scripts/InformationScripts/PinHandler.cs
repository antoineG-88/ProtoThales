using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinHandler : MonoBehaviour
{
    public RectTransform pinPanel;
    public GameObject deepSonarInfoPanel;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void CreateDeepSonarPin(Pin.Type pinType)
    {
        switch(pinType)
        {
            case Pin.Type.DeepSonar:

                break;
        }
    }
}
