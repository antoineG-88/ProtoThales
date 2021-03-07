using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatMarAction : BatimentAction
{
    private PatMarMovement patMarMovement;

    public override void Start()
    {
        base.Start();
        patMarMovement = (PatMarMovement)batimentMovement;
    }


    void Update()
    {
        
    }
}
