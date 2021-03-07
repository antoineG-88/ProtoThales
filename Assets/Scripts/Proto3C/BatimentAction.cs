using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatimentAction : MonoBehaviour
{
    [HideInInspector] public BatimentMovement batimentMovement;

    public virtual void Start()
    {
        batimentMovement = GetComponent<BatimentMovement>();
    }

    void Update()
    {
        
    }
}
