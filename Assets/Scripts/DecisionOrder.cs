using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class DecisionOrder : MonoBehaviour
{

    public Step decisionA;
    public Step decisionB;
    public Step decisionC;

    public StepManagerCustom stepManager;
    
    private List<Step> stepList = new List<Step>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stepList.Add(decisionA);
        stepList.Add(decisionB);
        stepList.Add(decisionC);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignList()
    {
        stepManager.SetList(stepList);
    }
}
