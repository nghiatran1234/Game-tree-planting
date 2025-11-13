using UnityEngine;
using UnityEngine.EventSystems;

public class TrashSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = DragDrop.itemBeingDragged;
        if (droppedItem == null) return;

        InventoryItem itemInfo = droppedItem.GetComponent<InventoryItem>();
        
        if (itemInfo != null && itemInfo.isTrashable)
        {
            droppedItem.transform.SetParent(transform);
            
            if (InventorySystem.Instance != null)
            {
                InventorySystem.Instance.ShowDeletionAlert(droppedItem);
            }
        }
    }
}