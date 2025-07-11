using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.AI;

public class DecisionA : MonoBehaviour
{
    public FetchBehaviour fetchBehaviour;
    public NavMeshAgent agent;
    public void OnButtonPress()
    {
        fetchBehaviour.SetFetch(false);
        agent.speed -= 1f;
        Debug.Log("Fetch Disabled");
    }
    
}
