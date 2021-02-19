using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public enum Relief { Flat, Hilly};
    public enum Depth { Land, Coast, Deep};
    public enum Weather { ClearSky, Wind, Storm};

    public Relief relief;
    public Depth depth;
    public Weather currentWeather;
    public Color zoneEdgeColor;
    public float innerEdgesOffset;
    public float outlineSeaOffset;
    public Transform iconPos;

    private List<Transform> edges;
    private Transform edgesParent;
    private Vector2 testVector;
    private LineRenderer edgeLine;

    private void Awake()
    {
        testVector = Vector2.one;
        GetEdges();
        CreateEdgeLine();
    }

    private void GetEdges()
    {
        edges = new List<Transform>();
        edgesParent = transform.GetChild(0);
        for (int i = 0; i < edgesParent.childCount; i++)
        {
            edges.Add(edgesParent.GetChild(i));
        }
    }

    private void CreateEdgeLine()
    {
        edgeLine = GetComponent<LineRenderer>();
        edgeLine.positionCount = edges.Count + 1;
        Vector3[] lineEdgePos = new Vector3[edges.Count + 1];
        for (int i = 0; i < edges.Count; i++)
        {
            lineEdgePos[i] = GetInnerEdge(i) + Vector3.up * outlineSeaOffset;
        }
        lineEdgePos[edges.Count] = GetInnerEdge(0) + Vector3.up * outlineSeaOffset;
        edgeLine.SetPositions(lineEdgePos);
        edgeLine.endColor = zoneEdgeColor;
        edgeLine.startColor = zoneEdgeColor;
    }

    private Vector3 GetInnerEdge(int edgeIndex)
    {
        int previousIndex = edgeIndex - 1;
        if(previousIndex < 0)
        {
            previousIndex = edges.Count - 1;
        }
        int nextIndex = edgeIndex + 1;
        if(nextIndex > edges.Count - 1)
        {
            nextIndex = 0;
        }
        Vector2 firstVector = SeaCoord.Planify(edges[previousIndex].position) - SeaCoord.Planify(edges[edgeIndex].position);
        Vector2 secondVector = SeaCoord.Planify(edges[nextIndex].position) - SeaCoord.Planify(edges[edgeIndex].position);

        float angle = Vector2.SignedAngle(firstVector, secondVector);
        float firstPointAngle = Vector2.SignedAngle(Vector2.right, firstVector) + angle / 2;
        float secondPointAngle = Vector2.SignedAngle(Vector2.right, firstVector) - (360 - angle) / 2;
        Vector2 pointVector1 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * firstPointAngle), Mathf.Sin(Mathf.Deg2Rad * firstPointAngle));
        Vector2 pointVector2 = new Vector2(Mathf.Cos(Mathf.Deg2Rad * secondPointAngle), Mathf.Sin(Mathf.Deg2Rad * secondPointAngle));

        Vector2 point1 = SeaCoord.Planify(edges[edgeIndex].position) + pointVector1 * innerEdgesOffset;
        Vector2 point2 = SeaCoord.Planify(edges[edgeIndex].position) + pointVector2 * innerEdgesOffset;

        if(IsElementInZone(point1))
        {
            return SeaCoord.GetFlatCoord(point1);
        }
        else
        {
            return SeaCoord.GetFlatCoord(point2);
        }
    }

    public void ChangeWeather(Weather newWeather)
    {
        currentWeather = newWeather;
    }

    public bool IsElementInZone(Vector2 elementPosition)
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
        edges = new List<Transform>();
        edgesParent = transform.GetChild(0);
        for (int i = 0; i < edgesParent.childCount; i++)
        {
            edges.Add(edgesParent.GetChild(i));
        }

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
    }
}
