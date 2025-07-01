using System;
using UnityEngine;

public class PettingBehaviour : MonoBehaviour
{
    public System.Action OnPettingStarted;
    public System.Action OnPettingEnded;

    private int handCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PettingHand>())
        {
            handCount++;
            if (handCount == 1)
            {
                OnPettingStarted?.Invoke();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PettingHand>())
        {
            handCount--;
            if (handCount <= 0)
            {
                handCount = 0;
                OnPettingEnded?.Invoke();
            }
        }
    }
    
}
