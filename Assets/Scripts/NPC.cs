using UnityEngine;
using UnityEngine.AI; // Bắt buộc phải có để dùng NavMesh

[RequireComponent(typeof(NavMeshAgent))] // Bắt NPC phải có NavMeshAgent
public class NPC : MonoBehaviour
{
    private NavMeshAgent agent;
    
    public float wanderRadius = 10f; // Bán kính đi lang thang
    public float wanderTimer = 5f;   // Thời gian nghỉ giữa mỗi lần đi
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Khi hết thời gian nghỉ...
        if (timer >= wanderTimer)
        {
            // Tìm một vị trí ngẫu nhiên mới trên NavMesh...
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            
            // ...và ra lệnh cho Agent đi đến đó
            agent.SetDestination(newPos);
            
            // Reset đồng hồ
            timer = 0;
        }
    }

    // Hàm tiện ích để tìm một điểm ngẫu nhiên trên NavMesh
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        
        return navHit.position;
    }
}