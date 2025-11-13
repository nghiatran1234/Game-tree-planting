using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public GameObject Item
    {
        get
        {
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }
            return null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragDrop.itemBeingDragged == null) return;

        GameObject itemDaKeo = DragDrop.itemBeingDragged;
        Transform viTriCu = DragDrop.itemBeingDragged.GetComponent<DragDrop>().startParent;
        // Nếu ô trống thì đặt item vào
        if (!Item)
        {
            if (SoundManager.Instance != null && SoundManager.Instance.dropItemSound != null)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.dropItemSound);
            }

            itemDaKeo.transform.SetParent(transform);
            itemDaKeo.transform.localPosition = Vector2.zero;

            UpdateItemState(itemDaKeo);
        }
        else
        {
            GameObject itemTrongO = Item;

            InventoryItem draggedScript = itemDaKeo.GetComponent<InventoryItem>();
            InventoryItem existingScript = itemTrongO.GetComponent<InventoryItem>();

            // Nếu cùng loại item -> cố gắng gộp vào stack hiện có
            if (draggedScript != null && existingScript != null && draggedScript.itemID == existingScript.itemID)
            {
                int space = existingScript.maxStackSize - existingScript.quantity;
                if (space > 0)
                {
                    int transfer = Mathf.Min(space, draggedScript.quantity);
                    existingScript.quantity += transfer;
                    existingScript.UpdateQuantityText();

                    draggedScript.quantity -= transfer;

                    // Nếu kéo hết stack thì hủy object kéo
                    if (draggedScript.quantity <= 0)
                    {
                        Destroy(itemDaKeo);
                    }
                    else
                    {
                        // Trả lại item kéo về vị trí cũ (startParent)
                        itemDaKeo.transform.SetParent(viTriCu);
                        itemDaKeo.transform.localPosition = Vector2.zero;
                        UpdateItemState(itemDaKeo);
                        draggedScript.UpdateQuantityText();
                    }

                    if (SoundManager.Instance != null && SoundManager.Instance.dropItemSound != null)
                    {
                        SoundManager.Instance.PlaySound(SoundManager.Instance.dropItemSound);
                    }
                }
                else
                {
                    // Nếu không còn chỗ thì đổi chỗ như bình thường
                    itemTrongO.transform.SetParent(viTriCu);
                    itemTrongO.transform.localPosition = Vector2.zero;
                    UpdateItemState(itemTrongO);

                    itemDaKeo.transform.SetParent(transform);
                    itemDaKeo.transform.localPosition = Vector2.zero;
                    UpdateItemState(itemDaKeo);

                    if (SoundManager.Instance != null && SoundManager.Instance.dropItemSound != null)
                    {
                        SoundManager.Instance.PlaySound(SoundManager.Instance.dropItemSound);
                    }
                }
            }
            else
            {
                // Khác loại -> đổi chỗ
                itemTrongO.transform.SetParent(viTriCu);
                itemTrongO.transform.localPosition = Vector2.zero;
                UpdateItemState(itemTrongO);

                itemDaKeo.transform.SetParent(transform);
                itemDaKeo.transform.localPosition = Vector2.zero;
                UpdateItemState(itemDaKeo);

                if (SoundManager.Instance != null && SoundManager.Instance.dropItemSound != null)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.dropItemSound);
                }
            }
        }
        
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.ReCalculateList();
        }
    }

    private void UpdateItemState(GameObject item)
    {
        if (item == null) return;
        
        InventoryItem itemScript = item.GetComponent<InventoryItem>();
        if (itemScript != null)
        {
            itemScript.isInsideQuickSlot = transform.CompareTag("QuickSlot");
        }
    }
}