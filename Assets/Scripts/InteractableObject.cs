using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public bool playerInRange;
    public string ItemName; 

    public string GetItemName()
    {
        return ItemName;
    }

    void Update()
    {
    if (SelectionManager.instance == null || InventorySystem.Instance == null) return;

        if (Input.GetKeyDown(KeyCode.E) && playerInRange && SelectionManager.instance.onTarget && SelectionManager.instance.seclectedObject == gameObject)
        {
            if (InventorySystem.Instance.CheckSlotAvailable(1))
            {
                InventorySystem.Instance.AddToInventory(ItemName);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Inventory is full");
            }
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
}