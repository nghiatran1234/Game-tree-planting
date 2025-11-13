using UnityEngine;
using UnityEngine.AI;
using System.Collections; // Cần dùng cho Coroutine nếu có animation chết

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class RabbitAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsPlayer;
    public Animator animator;

    public float health = 30f;
    public string animalID = "Rabbit";
    public GameObject lootPrefab;

    public enum AIState { Wander, Flee }
    public AIState currentState;

    [Header("Wander")]
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    private float timer;

    [Header("Detection")]
    public float fleeRange = 10f;
    private bool playerInFleeRange;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        currentState = AIState.Wander;
    }

    private void Update()
    {
        if (player == null) return;

        playerInFleeRange = Physics.CheckSphere(transform.position, fleeRange, whatIsPlayer);

        if (playerInFleeRange)
        {
            currentState = AIState.Flee;
        }
        else
        {
            // Chỉ quay lại Wander nếu đã dừng hẳn
            if (currentState == AIState.Flee && agent.velocity.magnitude < 0.1f && agent.remainingDistance < 0.5f)
            {
                currentState = AIState.Wander;
            }
        }

        switch (currentState)
        {
            case AIState.Wander:
                Wander();
                break;
            case AIState.Flee:
                Flee();
                break;
        }

        // --- SỬA PHẦN NÀY ---
        if (animator != null)
        {
            // Kiểm tra tốc độ thực tế của agent
            bool isMoving = agent.velocity.magnitude > 0.1f;
            // Đặt parameter bool "IsMoving"
            animator.SetBool("IsMoving", isMoving);
        }
        // --- HẾT PHẦN SỬA ---
    }

    private void Wander()
    {
        agent.speed = 3.5f;
        timer += Time.deltaTime;
        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    private void Flee()
    {
        agent.speed = 8f;

        Vector3 fleeDirection = transform.position - player.position;
        // Hơi ngẫu nhiên hướng chạy một chút để không bị kẹt
        fleeDirection += Random.insideUnitSphere * 2f;
        Vector3 newPos = transform.position + fleeDirection.normalized * 7f;

        NavMeshHit navHit;
        if(NavMesh.SamplePosition(newPos, out navHit, wanderRadius, -1))
        {
            agent.SetDestination(navHit.position);
        }
        // Nếu không tìm được điểm chạy hợp lệ, cố gắng tìm điểm bất kỳ
        else if (agent.remainingDistance < 1f)
        {
             Wander(); // Quay lại đi lang thang tạm
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        // Bắt thỏ chạy ngay khi bị đánh
        currentState = AIState.Flee;
        Flee();

        if (health <= 0)
        {
            Die();
        }
    }

     private void Die()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.UpdateProgress(animalID, 1);
        }

        if (lootPrefab != null)
        {
            Instantiate(lootPrefab, transform.position + Vector3.up * 0.3f, Quaternion.identity);
        }

        Destroy(gameObject);
    }


    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
}