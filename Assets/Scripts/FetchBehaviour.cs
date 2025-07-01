using System.Buffers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class FetchBehaviour : MonoBehaviour
{
    [Header("Public Fields")] 
    public Transform player;
    public ThrowDetector ball;
    public Transform mouthHold;
    public float pickUpDistance;
    public float returnDistance;
    
    [Header("Wandering Fields")]
    public float minIdleTime = 2f;
    public float maxIdleTime = 6f;
    
    public float minWanderTime = 4f;
    public float maxWanderTime = 12f;
    
    public float wanderRadius;
    
    enum State {Idle, Wander, Chasing, Returning}
    State state = State.Idle;
    
    private NavMeshAgent agent;
    private Rigidbody ballRb;
    private Collider ballCollider;
    
    private float timer;
    private float currentWanderDuration;
    private float currentIdleDuration;
    private bool destinationSet = false;
    
    private bool waitingToMove;
    private float waitAfterArrival = 1f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       agent = GetComponent<NavMeshAgent>();    
       ballRb = ball.GetComponent<Rigidbody>();
       ballCollider = ball.GetComponent<Collider>();

       currentIdleDuration = Random.Range(minIdleTime, maxIdleTime);
       
       ball.OnBallThrown += () => { state = State.Chasing; };
    }
    
    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Idle:
                IdleBehaviour();
                break;
            case State.Wander:
                WanderBehvaiour();
                break;
            case State.Chasing:
                ChaseBehaviour();
                break;
            case State.Returning:
                ReturnBehaviour();
                break;
        }
    }
    
    // --------------------
    // Idle Behaviour
    // --------------------

    void IdleBehaviour()
    {
        timer += Time.deltaTime;
        if (timer >= currentIdleDuration)
        {
            timer = 0f;
            state = State.Wander;
            currentWanderDuration = Random.Range(minWanderTime, maxWanderTime);
            destinationSet = false;
        }
        
    }

    // --------------------
    // Wander Behaviour
    // --------------------
    
    void WanderBehvaiour()
    {
        timer += Time.deltaTime;

        if (!destinationSet || (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance))
        {
            Vector3 newPosition = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPosition);
            destinationSet = true;
        }

        if (timer >= currentWanderDuration)
        {
            timer = 0f;
            state = State.Idle;
            currentIdleDuration = Random.Range(minIdleTime, maxIdleTime);
        }
    }
    
    private static Vector3 RandomNavSphere(Vector3 origin, float dist, int layerMask) 
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * dist;
        randomDirection += origin;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, dist, layerMask);
        return hit.position;
    }
    
    // --------------------
    // Chase Behaviour
    // --------------------

    void ChaseBehaviour()
    {
        agent.SetDestination(ball.transform.position);

        float distance = Vector3.Distance(transform.position, ball.transform.position);

        if (distance <= pickUpDistance)
        {
            PickUpBall();
        }    
        
    }

    // --------------------
    // Return Behaviour
    // --------------------
    
    void ReturnBehaviour()
    {
        agent.SetDestination(player.position);
        
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= returnDistance)
        {
            DropBall();
        }
    }
    
    void PickUpBall()
    {
        ballRb.isKinematic = true;
        ballCollider.enabled = false;
        ballRb.interpolation = RigidbodyInterpolation.None;
        ball.transform.SetParent(mouthHold, worldPositionStays:true);
        ball.transform.position = mouthHold.position;
        ball.transform.rotation = mouthHold.rotation;
        state = State.Returning;
    }

    void DropBall()
    {
        ball.transform.SetParent(null);
        ballRb.interpolation = RigidbodyInterpolation.Interpolate;
        ball.transform.position = player.position + player.forward * 0.5f;
        ballRb.isKinematic = false;
        ballCollider.enabled = true;
        ballRb.linearVelocity = Vector3.zero;
        state = State.Idle;
    }
    
}



