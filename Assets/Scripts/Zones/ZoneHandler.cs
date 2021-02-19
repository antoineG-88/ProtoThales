using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneHandler : MonoBehaviour
{
    public static List<Zone> zones;
    public List<Zone> mapZones;
    public List<RectTransform> mapZonesIcons;
    public RectTransform iconCanvas;
    public CameraController cameraController;
    [Range(0f, 1f)] public float iconDisplayZoomStep;

    private Vector2 viewPortPos;
    private Camera mainCamera;
    private void Awake()
    {
        zones = mapZones;
        mainCamera = Camera.main;
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

    private void Update()
    {
        if(cameraController.currentZoom > iconDisplayZoomStep)
        {
            for (int i = 0; i < mapZonesIcons.Count; i++)
            {
                mapZonesIcons[i].gameObject.SetActive(true);

                viewPortPos = mainCamera.WorldToViewportPoint(SeaCoord.GetFlatCoord(mapZones[i].iconPos.position));

                mapZonesIcons[i].anchoredPosition = new Vector2((viewPortPos.x - 0.5f) * iconCanvas.sizeDelta.x,
                    (viewPortPos.y - 0.5f) * iconCanvas.sizeDelta.y);
            }
        }
        else
        {
            for (int i = 0; i < mapZonesIcons.Count; i++)
            {
                mapZonesIcons[i].gameObject.SetActive(false);
            }
        }
    }
}
