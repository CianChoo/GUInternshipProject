using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ThrowDetector : MonoBehaviour
{
    public float throwForce = 1.5f;
    public event System.Action OnBallThrown;

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        
        grabInteractable.selectExited.AddListener(_ => CheckThrow());
    }

    void CheckThrow()
    {
        StartCoroutine(DelayedCheck());
    }

    private IEnumerator DelayedCheck()
    {
        yield return null;
        if (rb.linearVelocity.magnitude > throwForce)
        {
            OnBallThrown?.Invoke();
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
