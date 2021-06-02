using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    private GameObject target;
    public NavMeshAgent agent;
    private GameObject[] nodes;
    private Hashtable table = new Hashtable();
    private Stack<GameObject> scannedNodes = new Stack<GameObject>();

    float sensitivity = 0.3f;
    bool pathFound = false;

    private Stack<GameObject> reverseSearch = new Stack<GameObject>();
    private enum States {None = 0, RayCast = 1, Evaluate = 3, Move = 4 };

    States state = States.RayCast;


    float timer = 0.0f;
    bool set = false;
    // Start is called before the first frame update
    void Start()
    {
        this.nodes = new GameObject[148];
        for(int i = 0; i < 150; i++)
        {
            int toString = i + 1;
            //this.nodes[i] = GameObject.Find(toString.ToString());
            //this.reverseSearch.Push(this.nodes[i]);
        }
        //Debug.Log(nodes.Length);


    }

    // Update is called once per frame
    void Update()
    {
        
        if (state == States.RayCast)
        {
            this.DoRayCast();
            Debug.Log("RayCast!");
        }
        else if (state == States.Evaluate)
        {
            this.Evaluate();
            Debug.Log("Evaluate!");
        }
        else if (state == States.Move)
        {
            if (this.sensitivity > Vector3.Distance(this.transform.position, this.target.transform.position))
            {
                Debug.Log("Finished");
                this.agent.GetComponent<NavMeshAgent>().isStopped = true;
                this.state = States.RayCast;
                this.target.GetComponent<NodeComponent>().SetVisited(true);
                //this.reverseSearch.Push(this.target);
            }
            //Debug.Log("Move");
        }






        timer += Time.deltaTime;
        //Debug.Log(timer);
        if(timer >= 50.0f && set == false)
        {
            this.state = States.None;
            GameObject obj = new GameObject();
            obj = GameObject.FindGameObjectWithTag("Finish");
            agent.SetDestination(obj.transform.position);
            set = true;
        }
        //fakeRaycast();
    }

    private void DoRayCast()
    {
        this.pathFound = false;

        RaycastHit hit;
        // Ray forward
      
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

            if (hit.transform.tag == "Node" && hit.transform.GetComponent<NodeComponent>().GetVisited() == false)
            {
                if (!this.table.Contains(hit.transform.gameObject))
                {
                    //Debug.Log("Ray forward: " + hit.transform.name);
                    this.scannedNodes.Push(hit.transform.gameObject);
                    this.pathFound = true;
                    this.table.Add(hit.transform.gameObject, true);
                }
            }
        }

        // Ray back
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.back) * hit.distance, Color.red);

            if (hit.transform.tag == "Node" && hit.transform.GetComponent<NodeComponent>().GetVisited() == false)
            {
                if (!this.table.Contains(hit.transform.gameObject))
                {
                    // Debug.Log("Ray back: " + hit.transform.name);
                    this.scannedNodes.Push(hit.transform.gameObject);
                    this.pathFound = true;
                    this.table.Add(hit.transform.gameObject, true);
                    
                }
            }
        }

        // Ray right
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * hit.distance, Color.blue);

            if (hit.transform.tag == "Node" && hit.transform.GetComponent<NodeComponent>().GetVisited() == false)
            {
                if (!this.table.Contains(hit.transform.gameObject))
                {
                    //Debug.Log("Ray right: " + hit.transform.name);
                    this.scannedNodes.Push(hit.transform.gameObject);
                    this.pathFound = true;
                    this.table.Add(hit.transform.gameObject, true);
                }
               
               
            }
        }

        //Ray left
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * hit.distance, Color.green);

            if (hit.transform.tag == "Node" && hit.transform.GetComponent<NodeComponent>().GetVisited() == false)
            {
                if (!this.table.Contains(hit.transform.gameObject))
                {
                    //Debug.Log("Ray left: " + hit.transform.name);
                    this.scannedNodes.Push(hit.transform.gameObject);
                    this.pathFound = true;
                  
                    this.table.Add(hit.transform.gameObject, true);
                }
            }
        }

       

        
        this.state = States.Evaluate;
        /*else
        {
            Debug.Log("Do reverse tracking here!");
            GameObject temp = new GameObject();
            //temp =  this.reverseSearch.Pop();

            //while(temp.GetComponent<NodeComponent>().GetVisited() == true)
                //temp = this.reverseSearch.Pop();


            this.agent.GetComponent<NavMeshAgent>().isStopped = false;
            this.agent.SetDestination(this.target.transform.position);
        }*/

    }

    private void Evaluate()
    {
        float lightestWeight = 500.0f;
        GameObject temp = new GameObject();
        while(this.scannedNodes.Count > 0)
        {
            temp = this.scannedNodes.Pop();
            if (lightestWeight > temp.GetComponent<NodeComponent>().GetWeight())
            {
                this.target = temp;
                this.pathFound = true;

            }
            else
            {
                Debug.Log("reverse Search size push!!");
                //this.reverseSearch.Push(temp);
            }
        }

        if(this.pathFound == true)
        {
            this.agent.GetComponent<NavMeshAgent>().isStopped = false;
            this.agent.SetDestination(this.target.transform.position);
            
        } 
        else
        {
            Debug.Log("Do reverse tracking here!");
            //temp = this.reverseSearch.Pop();

            //while(temp.GetComponent<NodeComponent>().GetVisited() == false)
                //temp = this.reverseSearch.Pop();

            
            this.agent.GetComponent<NavMeshAgent>().isStopped = false;
            this.agent.SetDestination(this.target.transform.position);
            
        }
        //Debug.Log("reverse Search size: " + this.reverseSearch.Count);
        temp = null;
        this.state = States.Move;
    }



    void fakeRaycast()
    {
        RaycastHit hit;
        // Ray forward

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {

            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

            if (hit.transform.tag == "Node" && hit.transform.GetComponent<NodeComponent>().GetVisited() == false)
            {
                if (!this.table.Contains(hit.transform.gameObject))
                {
                    
                }
            }
        }

        // Ray back
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.back) * hit.distance, Color.red);

            if (hit.transform.tag == "Node" && hit.transform.GetComponent<NodeComponent>().GetVisited() == false)
            {
                if (!this.table.Contains(hit.transform.gameObject))
                {
                   

                }
            }
        }

        // Ray right
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * hit.distance, Color.blue);

            if (hit.transform.tag == "Node" && hit.transform.GetComponent<NodeComponent>().GetVisited() == false)
            {
                if (!this.table.Contains(hit.transform.gameObject))
                {
                    
                }


            }
        }

        //Ray left
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * hit.distance, Color.green);

            if (hit.transform.tag == "Node" && hit.transform.GetComponent<NodeComponent>().GetVisited() == false)
            {
                if (!this.table.Contains(hit.transform.gameObject))
                {
                   
                }
            }
        }
    }

}
