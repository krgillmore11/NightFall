
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    private Animator animator;

    private float velocity = 3.5f;
    private int veclocityNum;

    public LayerMask whatIsGround, whatIsPlayer;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //States
    public float sightRange;
    public bool playerInSightRange;

    private AudioSource source;
    private bool chasing = false;

    private void Awake()
    {
        //player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        veclocityNum = Animator.StringToHash("Velocity");
        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        
        if (!playerInSightRange) Patroling();
        if (playerInSightRange)
        {
            if (!chasing)
            {
                source.Play();
                chasing = true;
            }
            ChasePlayer();
        }
    }

    private void Patroling()
    {
        chasing = false;
        source.Stop();
        agent.speed = 3.5f;
        velocity = 0.3f;
        animator.SetFloat(veclocityNum, velocity);
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f))
            walkPointSet = true;
    }
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        agent.speed = 5f;
        velocity = 0.8f;
        animator.SetFloat(veclocityNum, velocity);
    }
}