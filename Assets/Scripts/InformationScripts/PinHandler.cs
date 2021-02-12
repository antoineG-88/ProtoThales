using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class PinHandler : MonoBehaviour
{
    public RectTransform pinPanelRectTransform;
    public RectTransform deepSonarInfoPanelRectTransform;
    public GameObject mapPinPrefab;
    public float minScreenDistancePinOpen;

    private List<Pin> pinPlaced;
    private Pin currentPinOpened;
    private Camera mainCamera;
    private Vector2 rectTransformInPinPanel;
    private Touch touch;
    void Start()
    {
        mainCamera = Camera.main;
        pinPlaced = new List<Pin>();
    }

    void Update()
    {
        UpdatePinPos();
        UpdatePinOpen();
    }

    public void CreateDeepSonarPin(int distanceStep, string submarineDirection, Vector2 fregateDirection, Vector2 mapPosition)
    {
        DeepSonarPin deepSonarPin = new DeepSonarPin();
        deepSonarPin.submarineDirection = submarineDirection;
        deepSonarPin.fregateDirection = fregateDirection;
        deepSonarPin.submarineDistanceStep = distanceStep;
        deepSonarPin.type = Pin.Type.DeepSonar;
        deepSonarPin.mapPosition = mapPosition;
        deepSonarPin.rectTransform = Instantiate(mapPinPrefab, pinPanelRectTransform).GetComponent<RectTransform>();
        pinPlaced.Add(deepSonarPin);
    }

    private void UpdatePinPos()
    {
        foreach(Pin pin in pinPlaced)
        {
            pin.viewPortPos = mainCamera.WorldToViewportPoint(SeaCoord.GetFlatCoord(pin.mapPosition));

            pin.rectTransform.anchoredPosition = new Vector2((pin.viewPortPos.x - 0.5f) * pinPanelRectTransform.sizeDelta.x,
                (pin.viewPortPos.y - 0.5f) * pinPanelRectTransform.sizeDelta.y);
        }
    }

    public void UpdatePinOpen()
    {
        if(InputDuo.tapDown)
        {
            foreach (Pin pin in pinPlaced)
            {
                if(Input.GetButton("LeftClick"))
                {
                    if (Vector2.Distance(mainCamera.ScreenToViewportPoint(Input.mousePosition), pin.viewPortPos) < minScreenDistancePinOpen)
                    {
                        OpenPin(pin);
                    }
                }
                else
                {
                    touch = Input.GetTouch(0);
                    if (Vector2.Distance(mainCamera.ScreenToViewportPoint(touch.position), pin.viewPortPos) < minScreenDistancePinOpen)
                    {
                        OpenPin(pin);
                    }
                }
            }
        }
    }

    private void OpenPin(Pin newPin)
    {
        currentPinOpened = newPin;
        switch (newPin.type)
        {
            case Pin.Type.DeepSonar:
                DeepSonarPin sonarPin = newPin as DeepSonarPin;
                break;
        }
    }

    [System.Serializable]
    public class Pin
    {
        [System.Serializable]
        public enum Type { DeepSonar };

        public Type type;
        public Vector2 mapPosition;
        public RectTransform rectTransform;
        public Vector2 viewPortPos;
    }

    [System.Serializable]
    public class DeepSonarPin : Pin
    {
        public int submarineDistanceStep;
        public string submarineDirection;
        public Vector2 fregateDirection;
    }
}
