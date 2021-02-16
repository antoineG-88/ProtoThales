using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneHandler : MonoBehaviour
{
    public static List<Zone> zones;
    public List<Zone> mapZones;


    private void Start()
    {
        zones = mapZones;
    }
    public static Zone GetCurrentZone(Vector2 position)
    {
        Zone currentZone = null;
        for (int i = 0; i < zones.Count; i++)
        {
            if (zones[i].IsElementInZone(position))
            {
                currentZone = zones[i];
            }
        }

        return currentZone;
    }
}
