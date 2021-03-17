using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicoController : BatimentAction
{
    public Transform upPropeller;
    public Transform sidePropeller;
    public float spinSpeed;

    public override void Start()
    {
        base.Start();
    }

    void Update()
    {
        SpinPropellers();
    }

    void SpinPropellers()
    {
        upPropeller.rotation = Quaternion.Euler(upPropeller.rotation.eulerAngles.x, upPropeller.rotation.eulerAngles.y + spinSpeed * Time.deltaTime, upPropeller.rotation.eulerAngles.z);
        sidePropeller.rotation *= Quaternion.AngleAxis(spinSpeed * Time.deltaTime, Vector3.up);
    }
}
