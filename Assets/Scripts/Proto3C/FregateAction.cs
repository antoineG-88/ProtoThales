using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FregateAction : BatimentAction
{
    private FregateMovement fregateMovement;

    public override void Start()
    {
        base.Start();
        fregateMovement = (FregateMovement)batimentMovement;
    }

    void Update()
    {
        
    }

    public void UseHelicopter()
    {
        Debug.Log("aoiuzfb");
    }
}
