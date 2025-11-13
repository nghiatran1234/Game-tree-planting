using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EquipableItem : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) &&
            InventorySystem.Instance != null && !InventorySystem.Instance.isOpen &&
            CraftingSystem.Instance != null && !CraftingSystem.Instance.isOpen &&
            SelectionManager.instance != null && !SelectionManager.instance.handIsVisible)
        {
            StartCoroutine(SwingSoundDelay());
            animator.SetTrigger("Hit");
        }
    }

    // HÀM NÀY SẼ ĐƯỢC GỌI BẰNG ANIMATION EVENT
    public void GetHit()
    {
        if (SelectionManager.instance == null) return;

        GameObject selectedTree = SelectionManager.instance.selectedTree;
        if (selectedTree != null)
        {
            if (SoundManager.Instance != null && SoundManager.Instance.chopSound != null)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.chopSound);
            }

            ChoppableTree tree = selectedTree.GetComponent<ChoppableTree>();
            if (tree != null)
            {
                tree.GetHit();
            }
            
            return; 
        }

        GameObject selectedObject = SelectionManager.instance.seclectedObject; 
        if (selectedObject != null)
        {
            EnemyAIController enemy = selectedObject.GetComponent<EnemyAIController>();
            if (enemy != null)
            {
                enemy.TakeDamage(25f); 
                return;
            }

            RabbitAI rabbit = selectedObject.GetComponent<RabbitAI>();
            if (rabbit != null)
            {
                rabbit.TakeDamage(25f); 
                return;
            }
        }
    }

    IEnumerator SwingSoundDelay()
    {
        yield return new WaitForSeconds(0.2f);
        if (SoundManager.Instance != null && SoundManager.Instance.toolSwingSound != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.toolSwingSound);
        }
    }
}