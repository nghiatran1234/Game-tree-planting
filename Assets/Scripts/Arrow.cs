using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Arrow : MonoBehaviour
{
    public float damage = 40f;
    public float lifeTime = 10f; // Thời gian tồn tại trước khi tự hủy
    public float forceMagnitude = 20f; // Lực bắn ban đầu

    private Rigidbody rb;
    private bool hasHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Bỏ qua va chạm với Player (nếu Player có Collider)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider playerCollider = player.GetComponent<Collider>();
            if (playerCollider != null)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), playerCollider);
            }
        }
    }

    void Start()
    {
        // Bắn mũi tên đi
        rb.AddForce(transform.forward * forceMagnitude, ForceMode.Impulse);
        // Tự hủy sau lifeTime giây
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Giữ mũi tên luôn hướng theo chiều di chuyển (trừ khi đã găm vào đâu đó)
        if (!hasHit && rb.linearVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return; // Chỉ xử lý va chạm đầu tiên

        hasHit = true;
        rb.isKinematic = true; // Dừng vật lý để mũi tên găm lại

        // Gắn mũi tên vào vật thể bị bắn trúng
        transform.SetParent(collision.transform);

        // Gây sát thương nếu bắn trúng địch hoặc con mồi
        EnemyAIController enemy = collision.gameObject.GetComponent<EnemyAIController>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            // Có thể thêm hiệu ứng trúng đích
            Destroy(gameObject, 2f); // Hủy mũi tên sau 2 giây nếu trúng địch
            return;
        }

        RabbitAI rabbit = collision.gameObject.GetComponent<RabbitAI>();
        if (rabbit != null)
        {
            rabbit.TakeDamage(damage);
            // Có thể thêm hiệu ứng trúng đích
            Destroy(gameObject, 2f); // Hủy mũi tên sau 2 giây nếu trúng mồi
            return;
        }

        // Nếu bắn trúng các vật khác, để mũi tên găm lại đó lâu hơn
        Destroy(gameObject, lifeTime); // Vẫn dùng lifeTime ban đầu
    }
}