using UnityEngine;
using UnityEngine.AI;

public class BehaviourChanger : MonoBehaviour
{
    private FetchBehaviour fetchBehaviour;
    private NavMeshAgent agent;
    private GameObject puppy;
    private GameObject ball;
    private GameObject food;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAttributes(FetchBehaviour fetchBehaviour, NavMeshAgent agent, GameObject puppy, GameObject ball, GameObject food)
    {
        this.fetchBehaviour = fetchBehaviour;
        this.agent = agent;
        this.puppy = puppy;
        this.ball = ball;
        this.food = food;
    }
}
