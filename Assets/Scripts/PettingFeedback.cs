using UnityEngine;

public class PettingFeedback : MonoBehaviour
{
    public PettingBehaviour pettingBehaviour;
    public AudioSource happySound;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pettingBehaviour.OnPettingStarted += ReactToPetting;
        pettingBehaviour.OnPettingEnded += StopReaction;
    }

    void ReactToPetting()
    {
        happySound?.Play();
    }

    void StopReaction()
    {
        
    }
}
