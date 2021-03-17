using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatimentAction : MonoBehaviour
{
    [HideInInspector] public BatimentMovement batimentMovement;
    [HideInInspector] public bool isDoingAction;
    [HideInInspector] public bool doingActionFlag;
    public static int currentActionNumber;

    private void Awake()
    {
        batimentMovement = GetComponent<BatimentMovement>();
    }

    public virtual void Start()
    {

    }

    public virtual void Update()
    {
        
    }
}
