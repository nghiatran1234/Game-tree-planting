using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
 
    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;
 
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
 
    Vector3 velocity;
    bool isGrounded;
    public bool isMoving;
 
    private bool isWalkingSoundPlaying = false;

    void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive) return;
        if (StorageManager.Instance != null && StorageManager.Instance.isOpen) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
 
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
 
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
 
        Vector3 move = transform.right * x + transform.forward * z;
 
        controller.Move(move * speed * Time.deltaTime);
 
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
 
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Kiểm tra di chuyển bằng input thay vì vị trí
        isMoving = move.magnitude > 0.1f;

        if (isMoving && isGrounded)
        {
            if (!isWalkingSoundPlaying && SoundManager.Instance != null && SoundManager.Instance.grassWalkSound != null)
            {
                SoundManager.Instance.grassWalkSound.Play();
                isWalkingSoundPlaying = true;
            }
        }
        else
        {
            if (isWalkingSoundPlaying && SoundManager.Instance != null && SoundManager.Instance.grassWalkSound != null)
            {
                SoundManager.Instance.grassWalkSound.Stop();
                isWalkingSoundPlaying = false;
            }
        }
    }
}