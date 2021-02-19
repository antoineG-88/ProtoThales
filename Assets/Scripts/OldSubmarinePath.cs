using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldSubmarinePath : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        if(!Application.isPlaying)
        {
            for (int i = 0; i < transform.childCount - 1; i++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
            }
        }
    }
}
