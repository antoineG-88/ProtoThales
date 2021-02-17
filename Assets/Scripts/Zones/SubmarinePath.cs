using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarinePath : MonoBehaviour
{
    [HideInInspector] public List<Transform> pathPosition = new List<Transform>();

    void Start()
    {
        pathPosition = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            pathPosition.Add(transform.GetChild(i));
        }
    }

    private void OnDrawGizmos()
    {
        pathPosition.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            pathPosition.Add(transform.GetChild(i));
        }

        if (pathPosition.Count > 0)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < pathPosition.Count - 1; i++)
            {
                Gizmos.DrawLine(pathPosition[i].position, pathPosition[i + 1].position);
            }

            Gizmos.DrawSphere(pathPosition[0].position, 0.3f);
        }
    }
}
