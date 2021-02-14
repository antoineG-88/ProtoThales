using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cross : MonoBehaviour
{
    public List<Transform> vectorPositions;
    public Color vector1Color;
    public Color vector2Color;

    private Vector2 vector1;
    private Vector2 vector2;

    private Vector2[] positions;
    private Vector2 inter1;
    private Vector2 inter2;
    private float scalar1;
    private float scalar2;
    private bool doIntersect;

    void Update()
    {
        Refresh();
        scalar1 = GetScalar(vector1, positions[0], vector2, positions[2]);
        scalar2 = GetScalar(vector2, positions[2], vector1, positions[0]);
        inter1 = positions[0] + vector1 * scalar1;
        inter2 = positions[2] + vector2 * scalar2;

        doIntersect = scalar1 >= 0 && scalar1 <= 1 && scalar2 >= 0 && scalar2 <= 1;

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(inter1);
        }
    }

    public static float GetScalar(Vector2 firstVector, Vector2 firstVectorStartPos, Vector2 secondVector, Vector2 secondVectorStartPos)
    {
        float result = CrossOwn(secondVectorStartPos - firstVectorStartPos, secondVector) / CrossOwn(firstVector, secondVector);

        return result;
    }

    private void Refresh()
    {
        vector1 = SeaCoord.Planify(vectorPositions[1].position) - SeaCoord.Planify(vectorPositions[0].position);
        vector2 = SeaCoord.Planify(vectorPositions[3].position) - SeaCoord.Planify(vectorPositions[2].position);
        positions = new Vector2[4];
        for (int i = 0; i < vectorPositions.Count; i++)
        {
            positions[i] = SeaCoord.Planify(vectorPositions[i].position);
            //Debug.Log("Position " + i + " : (" + positions[i].x + ")(" + positions[i].y + ")");
        }
    }

    private static float CrossOwn(Vector2 first, Vector2 second)
    {
        return first.x * second.y - first.y * second.x; 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = vector1Color;
        Gizmos.DrawLine(vectorPositions[0].position, vectorPositions[1].position);


        Gizmos.color = vector2Color;
        Gizmos.DrawLine(vectorPositions[2].position, vectorPositions[3].position);

        if(doIntersect)
        {
            Gizmos.color = vector1Color;
            Gizmos.DrawSphere(SeaCoord.GetFlatCoord(inter1), 0.4f);

            Gizmos.color = vector2Color;
            Gizmos.DrawSphere(SeaCoord.GetFlatCoord(inter2), 0.3f);
        }
    }
}
