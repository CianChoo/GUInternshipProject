using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class FoodHeldChecker : MonoBehaviour
{
    public FetchBehaviour fetchBehaviour;
    
    private XRGrabInteractable grabInteractable;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        fetchBehaviour.OnFoodHeld(true);
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        fetchBehaviour.OnFoodHeld(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
