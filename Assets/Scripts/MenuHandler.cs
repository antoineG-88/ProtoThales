using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    public GameObject fregateMenu;
    public LayerMask elementsLayer;
    public FregateController fregate;

    private Camera mainCamera;
    private Touch touch;
    void Start()
    {
        Statics.inMenu = false;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
        }

        if ((Input.GetButtonDown("LeftClick") || touch.phase == TouchPhase.Began) && !Statics.inMenu)
        {
            RaycastHit touchHit;
            Ray screenRay;
            if (Input.GetButtonUp("LeftClick"))
            {
                screenRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            }
            else
            {

                screenRay = mainCamera.ScreenPointToRay(touch.position);
            }

            if (Physics.Raycast(screenRay, out touchHit, 200f, elementsLayer))
            {
                fregateMenu.SetActive(true);
                Statics.inMenu = true;
            }
        }
    }

    public void SetInMenu(bool inMenu)
    {
        Statics.inMenu = inMenu;
    }
}
