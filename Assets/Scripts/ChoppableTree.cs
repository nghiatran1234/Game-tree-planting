using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ChoppableTree : MonoBehaviour
{
    public bool playerInRange;
    public bool canBeChopped;

    public float treeMaxHealth;
    public float treeHealth;

    public Animator animator;
    public GameObject choppedTreePrefab;
    public GameObject rootTreeObject;

    public float caloriesSpentChoppingWood = 20;

    private void Start()
    {
        treeHealth = treeMaxHealth;

        if (animator == null)
        {
            animator = GetComponentInParent<Animator>();
        }

        if (rootTreeObject == null && transform.parent != null && transform.parent.transform.parent != null)
        {
            rootTreeObject = transform.parent.transform.parent.gameObject;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void GetHit()
    {
        treeHealth -= 10;
        
        if (animator != null)
        {
            animator.SetTrigger("shake");
        }

        if (PlayerState.Instance != null)
        {
            PlayerState.Instance.currentCalories -= caloriesSpentChoppingWood;
        }

        if (treeHealth <= 0)
        {
            TreeIsDead();
        }
    }

    private void TreeIsDead()
    {
        if (SoundManager.Instance != null && SoundManager.Instance.choppabletreeSound != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.choppabletreeSound);
        }

        Vector3 treePosition = transform.position;

        if (rootTreeObject != null)
        {
            Destroy(rootTreeObject);
        }

        canBeChopped = false;

        if (SelectionManager.instance != null)
        {
            if (SelectionManager.instance.chopHolder != null)
            {
                SelectionManager.instance.chopHolder.gameObject.SetActive(false);
            }
            SelectionManager.instance.selectedTree = null;
        }

        if (choppedTreePrefab != null)
        {
            Instantiate(choppedTreePrefab,
                new Vector3(treePosition.x, treePosition.y + 1, treePosition.z), Quaternion.Euler(0, 0, 0));
        }
        else
        {
            Debug.LogError("ChoppedTree Prefab chưa được gán trên " + gameObject.name);
        }
    }

    private void Update()
    {
        if (canBeChopped)
        {
            if (GlobalState.instance != null)
            {
                GlobalState.instance.resourceHealth = treeHealth;
                GlobalState.instance.resourceMaxHealth = treeMaxHealth;
            }
        }
    }
}