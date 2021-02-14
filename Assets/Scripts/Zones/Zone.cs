using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public Color zoneEdgeColor;
    public Transform element;

    private List<Transform> edges = new List<Transform>();
    private Transform edgesParent;
    private bool isInside;
    private Vector2 testVector;

    void Start()
    {
        testVector = Vector2.one;
        edges = new List<Transform>();
        edgesParent = transform.GetChild(0);
        for(int i = 0; i < edgesParent.childCount; i++)
        {
            edges.Add(edgesParent.GetChild(i));
        }
    }

    void Update()
    {
        isInside = IsElementInZone(SeaCoord.Planify(element.position));
    }

    private bool IsElementInZone(Vector2 elementPosition)
    {
        int numberOfIntersection = 0;
        float scalar1;
        float scalar2;
        for (int i = 0; i < edges.Count; i++)
        {
            int next;
            if (i == edges.Count - 1)
            {
                next = 0;
            }
            else
            {
                next = i + 1;
            }
            scalar1 = Cross.GetScalar(testVector * 200, elementPosition, SeaCoord.Planify(edges[next].position - edges[i].position), SeaCoord.Planify(edges[i].position));
            scalar2 = Cross.GetScalar(SeaCoord.Planify(edges[next].position - edges[i].position), SeaCoord.Planify(edges[i].position), testVector * 200, elementPosition);


            if (scalar1 >= 0 && scalar1 < 1 && scalar2 >= 0 && scalar2 <= 1)
            {
                numberOfIntersection++;
            }

        }
        return numberOfIntersection % 2 != 0;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = zoneEdgeColor;
        for(int i = 0; i < edges.Count; i++)
        {
            if(i == edges.Count - 1)
            {
                Gizmos.DrawLine(edges[i].position, edges[0].position);
            }
            else
            {
                Gizmos.DrawLine(edges[i].position, edges[i + 1].position);
            }
        }

        Gizmos.color = isInside ? Color.cyan : Color.red;
        Gizmos.DrawSphere(element.position, 0.5f);

    }
}
