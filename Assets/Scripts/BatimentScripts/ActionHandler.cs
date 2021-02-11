using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionHandler : MonoBehaviour
{
    public GameObject actionMenu;
    public GameObject patMarMenu;
    public PatMar patMar;
    public PatMarHandler patMarHandler;
    public GameObject fregateMenu;
    public Fregate fregate;
    public Transform submarine;


    private BatimentController batimentController;

    void Start()
    {
        batimentController = GetComponent<BatimentController>();
        fregateMenu.SetActive(false);
        patMarMenu.SetActive(false);
    }

    void Update()
    {
        actionMenu.SetActive(batimentController.batimentSelected != null);

        fregateMenu.SetActive(batimentController.batimentSelected == fregate);
        patMarMenu.SetActive(batimentController.batimentSelected == patMar);
    }
}
