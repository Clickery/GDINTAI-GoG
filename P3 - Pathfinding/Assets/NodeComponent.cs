using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeComponent : MonoBehaviour
{
    private float weight;
    private Transform target;
    private bool visited;

    // Start is called before the first frame update
    void Start()
    {
        GameObject target = GameObject.FindGameObjectWithTag("Finish");
        this.target = target.transform;

        this.weight = Vector3.Distance(this.transform.position, this.target.position);

        this.visited = false;
        Debug.Log(this.name + ", " + this.weight);
    }

    public float GetWeight()
    {
        return this.weight;
    }

    public bool GetVisited()
    {
        return this.visited;
    }

    public void SetVisited(bool visited)
    {
        this.visited = visited;
    }
}
