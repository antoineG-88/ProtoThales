using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineTriggerZone : MonoBehaviour
{
    [HideInInspector] public bool fregateIsAbove;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fregateIsAbove = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fregateIsAbove = false;
        }
    }
}
