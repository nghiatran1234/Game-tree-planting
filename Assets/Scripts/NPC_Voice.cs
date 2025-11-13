using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(QuestGiver))]
public class NPC_Voice : MonoBehaviour
{
    private AudioSource audioSource;
    public List<AudioClip> voiceLines; 
    
    public float talkCooldown = 5f;
    private bool canTalk = true;
    private QuestGiver questGiver;

    // 1. Thêm biến public bool playerInRange
    public bool playerInRange; 

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        questGiver = GetComponent<QuestGiver>();

        SphereCollider detectionZone = GetComponent<SphereCollider>();
        detectionZone.isTrigger = true;
    }

    // Hàm này được gọi khi có ai đó đi vào vùng trigger
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem đó có phải là Player không
        if (other.CompareTag("Player"))
        {
            // Đặt cờ playerInRange (giống ChoppableTree.cs)
            playerInRange = true; 
        }
    }

    // 2. Thêm hàm OnTriggerExit
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Đặt cờ playerInRange (giống ChoppableTree.cs)
            playerInRange = false; 
        }
    }

    // 3. Di chuyển logic vào Update()
    private void Update()
    {
        if (canTalk && Input.GetKeyDown(KeyCode.E) && 
        SelectionManager.instance != null && 
        SelectionManager.instance.selectedNPC ==this.gameObject)
        {
            if (questGiver != null)
            {
                questGiver.Interact();
            }
            else
            {
                PlayRandomVoiceLine();
            }
        }
    }

    private void PlayRandomVoiceLine()
    {
        if (voiceLines == null || voiceLines.Count == 0)
            return;

        canTalk = false;

        int index = Random.Range(0, voiceLines.Count);
        AudioClip clipToPlay = voiceLines[index];

        audioSource.PlayOneShot(clipToPlay);

        Invoke(nameof(ResetCooldown), talkCooldown);
    }

    private void ResetCooldown()
    {
        canTalk = true;
    }
}