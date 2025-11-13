using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float moveRadius = 15f;   // Bán kính di chuyển
    public float waitTime = 2f;      // Thời gian chờ khi tới nơi

    private Animator animator;
    private NavMeshAgent agent;
    private float waitTimer;
    private bool isChasing = false;
    public float detectRange = 10f;
    public Transform player;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        MoveToRandomPoint();
        waitTimer = waitTime;
        
    }

    void Update()
    {
        // Bật/tắt animation dựa theo tốc độ agent
        animator.SetBool("isMoving", agent.velocity.magnitude > 0.1f);
           float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectRange)
        {
            // Chuyển sang Chase
            isChasing = true;
            agent.SetDestination(player.position);
        }

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // Nếu đã đến nơi
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    waitTimer -= Time.deltaTime;
                    if (waitTimer <= 0f)
                    {
                        MoveToRandomPoint();
                        waitTimer = waitTime;
                    }
                }
            }
        }
    }

    void MoveToRandomPoint()
    {
        // Random vị trí trong bán kính moveRadius
        Vector3 randomDirection = Random.insideUnitSphere * moveRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, moveRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            Debug.Log("Không tìm thấy vị trí hợp lệ trên NavMesh!");
        }
    }
}