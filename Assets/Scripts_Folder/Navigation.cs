using UnityEngine;
using UnityEngine.AI;
public class Navigation : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;
    public GameObject Path;
    public Transform[] PathPoints;

    public float minDistance = 10;
    public int index = 0;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        PathPoints = new Transform[Path.transform.childCount];
        for (int i = 0; i < PathPoints.Length; i++)
        {
            PathPoints[i] = Path.transform.GetChild(i);
        }
    }

    void Update()
    {
        move(); 
    }
    void move()
    {
        if (PathPoints.Length == 0) 
        {
            return;
        }

        if (Vector3.Distance(transform.position, PathPoints[index].position) < minDistance)
        {
            index++; 

            if (index >= PathPoints.Length) 
            {
                index = 0; 
            }
        }
        agent.SetDestination(PathPoints[index].position);
    }
}
