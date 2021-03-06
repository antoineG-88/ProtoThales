﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class PinHandler : MonoBehaviour
{
    public RectTransform pinPanelRectTransform;
    public RectTransform openPinRectTransform;
    public GameObject deepSonarInfoPanelRectTransform;
    public GameObject scanAlertInfoPanelRectTransform;
    public GameObject sonoFlashAlertInfoPanelRectTransform;
    public GameObject mapPinPrefab;
    public float minScreenDistancePinOpen;
    public FregateHandler fregateHandler;
    public Image deepSonarDistanceImage;
    public float timeBeforePinAutoDestroy;

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

    public void CreateDeepSonarPin(int distanceStep, Vector2 mapPosition)
    {
        DeepSonarPin deepSonarPin = new DeepSonarPin();
        deepSonarPin.submarineDistanceStep = distanceStep;
        deepSonarPin.type = Pin.Type.DeepSonar;
        deepSonarPin.mapPosition = mapPosition;
        deepSonarPin.rectTransform = Instantiate(mapPinPrefab, pinPanelRectTransform).GetComponent<RectTransform>();
        pinPlaced.Add(deepSonarPin);
        StartCoroutine(AutoDestroyPin(deepSonarPin));
        
    }

    public void CreateScanAlertPin(Vector2 mapPosition)
    {
        Pin newPin = new Pin();
        newPin.type = Pin.Type.ScanAlert;
        newPin.mapPosition = mapPosition;
        newPin.rectTransform = Instantiate(mapPinPrefab, pinPanelRectTransform).GetComponent<RectTransform>();
        pinPlaced.Add(newPin);
        StartCoroutine(AutoDestroyPin(newPin));
    }

    public void CreateSonoFlashAlertPin(Vector2 mapPosition)
    {
        Pin newPin = new Pin();
        newPin.type = Pin.Type.SonoFlashAlert;
        newPin.mapPosition = mapPosition;
        newPin.rectTransform = Instantiate(mapPinPrefab, pinPanelRectTransform).GetComponent<RectTransform>();
        pinPlaced.Add(newPin);
        StartCoroutine(AutoDestroyPin(newPin));
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
        bool atLeastOnePinOpened = false;
        if(InputDuo.tapDown && !EventSystem.current.IsPointerOverGameObject(/*Input.GetTouch(0).fingerId*/))
        {
            foreach (Pin pin in pinPlaced)
            {
                if(Input.GetButton("LeftClick"))
                {
                    if (Vector2.Distance(mainCamera.ScreenToViewportPoint(Input.mousePosition), pin.viewPortPos) < minScreenDistancePinOpen)
                    {
                        atLeastOnePinOpened = true;
                        OpenPin(pin);
                    }
                }
                else
                {
                    touch = Input.GetTouch(0);
                    if (Vector2.Distance(mainCamera.ScreenToViewportPoint(touch.position), pin.viewPortPos) < minScreenDistancePinOpen)
                    {
                        atLeastOnePinOpened = true;
                        OpenPin(pin);
                    }
                }
            }

            if(!atLeastOnePinOpened)
            {
                ClosePin();
            }
        }

        if(currentPinOpened != null)
        {
            openPinRectTransform.anchoredPosition = new Vector2((currentPinOpened.viewPortPos.x - 0.5f) * pinPanelRectTransform.sizeDelta.x,
                   (currentPinOpened.viewPortPos.y - 0.5f) * pinPanelRectTransform.sizeDelta.y);
        }
    }

    private void OpenPin(Pin newPin)
    {
        currentPinOpened = newPin;
        switch (newPin.type)
        {
            case Pin.Type.DeepSonar:
                deepSonarInfoPanelRectTransform.gameObject.SetActive(true);
                openPinRectTransform.anchoredPosition = new Vector2((newPin.viewPortPos.x - 0.5f) * pinPanelRectTransform.sizeDelta.x,
                (newPin.viewPortPos.y - 0.5f) * pinPanelRectTransform.sizeDelta.y);
                DeepSonarPin sonarPin = newPin as DeepSonarPin;
                deepSonarDistanceImage.sprite = fregateHandler.deepSonarDistanceStepImages[sonarPin.submarineDistanceStep - 1];
                break;

            case Pin.Type.ScanAlert:
                scanAlertInfoPanelRectTransform.gameObject.SetActive(true);
                openPinRectTransform.anchoredPosition = new Vector2((newPin.viewPortPos.x - 0.5f) * pinPanelRectTransform.sizeDelta.x,
                (newPin.viewPortPos.y - 0.5f) * pinPanelRectTransform.sizeDelta.y);
                break;

            case Pin.Type.SonoFlashAlert:
                sonoFlashAlertInfoPanelRectTransform.gameObject.SetActive(true);
                openPinRectTransform.anchoredPosition = new Vector2((newPin.viewPortPos.x - 0.5f) * pinPanelRectTransform.sizeDelta.x,
                (newPin.viewPortPos.y - 0.5f) * pinPanelRectTransform.sizeDelta.y);
                break;
        }
    }

    private void ClosePin()
    {
        deepSonarInfoPanelRectTransform.gameObject.SetActive(false);
        scanAlertInfoPanelRectTransform.gameObject.SetActive(false);
        sonoFlashAlertInfoPanelRectTransform.gameObject.SetActive(false);
    }

    public void DestroyOpenedPin()
    {
        ClosePin();
        Destroy(currentPinOpened.rectTransform.gameObject);
        pinPlaced.Remove(currentPinOpened);
    }

    [System.Serializable]
    public class Pin
    {
        [System.Serializable]
        public enum Type { DeepSonar , ScanAlert, SonoFlashAlert};

        public Type type;
        public Vector2 mapPosition;
        public RectTransform rectTransform;
        public Vector2 viewPortPos;
    }

    [System.Serializable]
    public class DeepSonarPin : Pin
    {
        public int submarineDistanceStep;
    }

    public IEnumerator AutoDestroyPin(Pin pin)
    {
        yield return new WaitForSeconds(timeBeforePinAutoDestroy);
        if(pin.rectTransform != null)
        {
            Destroy(pin.rectTransform.gameObject);
            pinPlaced.Remove(pin);
        }
    }
}
