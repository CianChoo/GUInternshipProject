using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.AI;

public class DecisionB : MonoBehaviour
{
    public FetchBehaviour fetchBehaviour;
    public NavMeshAgent agent;
    public void OnButtonPress()
    {
        fetchBehaviour.SetFeed(false);
        agent.speed -= 0.5f;
    }
}
