using UnityEngine;
using System.Collections.Generic;

public class StorageBox : MonoBehaviour
{
    public bool playerInRange;
    public List<string> items = new List<string>();

    void Update()
    {
        if (SelectionManager.instance == null) return;
        
        if (Input.GetKeyDown(KeyCode.E) && playerInRange && 
            SelectionManager.instance.onTarget && 
            SelectionManager.instance.seclectedObject == gameObject)
        {
            if (StorageManager.Instance != null)
            {
                StorageManager.Instance.OpenStorage(this);
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