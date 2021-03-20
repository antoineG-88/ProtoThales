using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BioMovement : MonoBehaviour
{
    public List<Transform> path;
    public float speed;

    private int nextPointPath;

    void Start()
    {
        transform.position = SeaCoord.GetFlatCoord(path[0].position);
        nextPointPath = 1;
    }
    
    void Update()
    {
        if (Vector2.Distance(SeaCoord.Planify(transform.position), SeaCoord.Planify(path[nextPointPath].position)) < speed * Time.deltaTime)
        {
            nextPointPath++;
            if(nextPointPath >= path.Count)
            {
                nextPointPath = 0;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, SeaCoord.GetFlatCoord(path[nextPointPath].position), speed * Time.deltaTime);
        }
    }
}
