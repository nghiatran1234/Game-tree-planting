using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAIController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Animator animator;

    public LayerMask whatIsGround, whatIsPlayer;

    // Đã đổi AIState.Wander thành AIState.Patrol
    public enum AIState { Patrol, Chase, Attack }
    public AIState currentState;

    [Header("Patrol")] // Đã đổi tiêu đề từ Wander sang Patrol
    public Transform[] patrolPoints; // 👈 Mảng chứa các điểm tuần tra (Game Object)
    public float idleTimeAtPoint = 5f; // Thời gian dừng (Idle) tại mỗi điểm
    private int currentPointIndex = 0;
    private bool isIdlingAtPoint = false;
    private float idleTimer;
    public float patrolSpeed = 30f; // Tốc độ di chuyển khi tuần tra

    [Header("Chase")]
    public float chaseSpeed = 30f; // Tốc độ khi truy đuổi

    [Header("Detection")]
    public float sightRange = 15f;
    public float attackRange = 2f;
    private bool playerInSightRange;
    private bool playerInAttackRange;

    [Header("Attack")]
    public float timeBetweenAttacks = 2f;
    private bool alreadyAttacked;
    public float attackDamage = 10f;

    [Header("Quest & Stats")]
    public string enemyID = "Bear";
    public float health = 100f;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Thêm thiết lập tốc độ và độ mượt khi khởi tạo
        agent.angularSpeed = 500f; 
        agent.acceleration = 20f;
        
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        currentState = AIState.Patrol; // Bắt đầu ở trạng thái Patrol
        idleTimer = idleTimeAtPoint; // Khởi tạo timer

        // Kiểm tra mảng điểm tuần tra, vô hiệu hóa nếu rỗng
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("Patrol Points array is empty! Assign Transform Game Objects in the Inspector.");
            this.enabled = false;
            return;
        }
    }

    private void Update()
    {
        // ... (Giữ nguyên logic Death và kiểm tra Player)

        if (health <= 0) { 
            if (GetComponent<Collider>().enabled) Die();
            return;
        }
        if (player == null) return;

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // --- STATE MACHINE (Giữ nguyên, đã đúng) ---
        if (playerInSightRange && playerInAttackRange)
        {
            currentState = AIState.Attack;
}
        else if (playerInSightRange)
        {
            currentState = AIState.Chase;
        }
        else // Khi không thấy Player
        {
            currentState = AIState.Patrol; // Quay lại Patrol
        }

        // --- EXECUTE STATE ---
        switch (currentState)
        {
            case AIState.Patrol:
                Patrol(); 
                break;
            case AIState.Chase:
                Chase();
                break;
            case AIState.Attack:
                Attack();
                break;
        }

        // --- ANIMATOR CONTROL (ĐÃ SỬA) ---
        if (animator != null)
        {
            // isMoving chỉ đúng khi đang không ở trạng thái Idle trong Patrol
            bool isMoving = agent.velocity.magnitude > 0.1f && !isIdlingAtPoint; 
            animator.SetBool("isWalking", isMoving);

            // ⚠️ Dòng animator.SetTrigger("isAttack"); đã được xóa khỏi đây!
            // Trigger chỉ được gọi 1 lần trong hàm Attack().
        }
    }

    // --- LOGIC MOVEMENT (Patrol) ---
    private void Patrol()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        agent.isStopped = false;
        agent.speed = patrolSpeed;

        // 1. Kiểm tra xem đã đến điểm tuần tra chưa
        // Dùng agent.remainingDistance vì nó chính xác khi Agent đang tính toán đường đi.
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !isIdlingAtPoint)
        {
            // Đã đến điểm, chuyển sang trạng thái IDLE
            isIdlingAtPoint = true;
            agent.isStopped = true; // Dừng Agent
        }

        if (isIdlingAtPoint)
        {
            // 2. Quản lý thời gian dừng (Idle)
            idleTimer -= Time.deltaTime;

            // Chuyển sang điểm tiếp theo khi hết giờ
            if (idleTimer <= 0)
            {
                // Chuyển sang điểm tiếp theo (Loop)
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
                isIdlingAtPoint = false; // Bắt đầu di chuyển
                idleTimer = idleTimeAtPoint; // Reset timer
                
                // Bắt đầu di chuyển tới điểm mới ngay lập tức
                agent.isStopped = false;
                agent.SetDestination(patrolPoints[currentPointIndex].position);
            }
        }
        else if (agent.remainingDistance <= agent.stoppingDistance) // Nếu đã hoàn tất đường đi trước
        {
             // 3. Di chuyển đến điểm nếu chưa có đường đi
             agent.SetDestination(patrolPoints[currentPointIndex].position);
        }
    }
    
    // --- LOGIC MOVEMENT (Chase) ---
    private void Chase()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        isIdlingAtPoint = false;
agent.isStopped = false;
        agent.speed = chaseSpeed; // Tăng tốc độ lên tốc độ đuổi
        
        // Thiết lập điểm đến là vị trí của người chơi
        agent.SetDestination(player.position);
    }
    
    // --- LOGIC MOVEMENT (Attack) ---
    private void Attack()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return; 

        isIdlingAtPoint = false; 
        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;

        // Xoay mặt về phía Player
        Vector3 directionToPlayer = (player.position - transform.position);
        directionToPlayer.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToPlayer); 

        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            animator.SetTrigger("isAttack"); // Dùng Trigger cho Attack 1 lần
            
            // Call damage immediately (or prefer Animation Event)
            DealDamage(); 

            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    
    // ... (Các hàm DealDamage, ResetAttack, TakeDamage, Die giữ nguyên)
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
    private void DealDamage() 
    {
        if (Physics.CheckSphere(transform.position, attackRange, whatIsPlayer))
        {
            Debug.Log("Enemy dealt " + attackDamage + " damage.");
            if (PlayerState.Instance != null)
            {
                PlayerState.Instance.TakeDamage(attackDamage);
            }
              // Thêm logic gây sát thương thực tế vào đây
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        // Bắt đầu di chuyển lại khi cần
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
             // Đặt lại isStopped = false sẽ được xử lý trong Chase/Patrol tiếp theo
             // nhưng có thể đặt ở đây để phản ứng nhanh hơn nếu cần
        }
    }
    
    private void Die()
    {
        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        animator.SetTrigger("isDead");
        GetComponent<Collider>().enabled = false;
        this.enabled = false;
    }
    
    // Hàm RandomNavSphere đã bị loại bỏ vì không cần Wander nữa

    // --- OnDrawGizmosSelected để vẽ phạm vi và điểm tuần tra ---
    private void OnDrawGizmosSelected()
    {
        // Vẽ phạm vi phát hiện và tấn công
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Vẽ các điểm tuần tra
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
if (patrolPoints[i] != null) 
                {
                    Vector3 currentPos = patrolPoints[i].position; 
                    Gizmos.DrawSphere(currentPos, 0.5f);
                    
                    if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                    {
                        // Vẽ đường nối giữa các điểm
                        Gizmos.DrawLine(currentPos, patrolPoints[i + 1].position);
                    }
                }
            }
            // Vẽ đường nối từ điểm cuối về điểm đầu (loop)
            if (patrolPoints.Length > 1 && patrolPoints[patrolPoints.Length - 1] != null && patrolPoints[0] != null)
            {
                 Gizmos.DrawLine(patrolPoints[patrolPoints.Length - 1].position, patrolPoints[0].position);
            }
        }
    }
}