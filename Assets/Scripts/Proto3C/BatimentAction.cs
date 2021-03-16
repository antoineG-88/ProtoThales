using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatimentAction : MonoBehaviour
{
    [HideInInspector] public BatimentMovement batimentMovement;
    [HideInInspector] public bool isDraggingAction;
    private void Awake()
    {
        batimentMovement = GetComponent<BatimentMovement>();
    }

    public virtual void Start()
    {

    }

    void Update()
    {
        
    }
}
