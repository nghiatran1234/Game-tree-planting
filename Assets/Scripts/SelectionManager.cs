using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance{ get; set; }
 
    public GameObject interaction_Info_UI;
    public Text interaction_text;
    public bool onTarget;

    public GameObject seclectedObject;

    public GameObject selectedNPC; 
 
    public Image centerDotImage;
    public Image handIcon;

    public bool handIsVisible;

    public GameObject selectedTree;
    public GameObject chopHolder;

    private void Start()
    {
        onTarget = false;
        if (interaction_text == null)
        {
             interaction_text = interaction_Info_UI.GetComponentInChildren<Text>();
        }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;

            InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();
            ChoppableTree choppableTree = selectionTransform.GetComponent<ChoppableTree>();
            NPC_Voice npcVoice = selectionTransform.GetComponent<NPC_Voice>();
            EnemyAIController enemy = selectionTransform.GetComponent<EnemyAIController>(); 
            RabbitAI rabbit = selectionTransform.GetComponent<RabbitAI>();
            StorageBox storage = selectionTransform.GetComponent<StorageBox>();
            WaterSource waterSource = selectionTransform.GetComponent<WaterSource>(); 

            if (choppableTree && choppableTree.playerInRange)
            {
                selectedTree = choppableTree.gameObject;
                choppableTree.canBeChopped = true;
                chopHolder.gameObject.SetActive(true);
            }
            else
            {
                if (selectedTree!= null)
                {
                    selectedTree.gameObject.GetComponent<ChoppableTree>().canBeChopped = false;
                    selectedTree = null;
                    chopHolder.gameObject.SetActive(false);
                }
            }
            // Vật tương tác chung
            if (interactable && interactable.playerInRange)
            {
                onTarget = true;
                seclectedObject = interactable.gameObject;
                interaction_text.text = interactable.GetItemName();
                interaction_Info_UI.SetActive(true);

                if (interactable.CompareTag("pickable"))
                {
                    centerDotImage.gameObject.SetActive(false);
                    handIcon.gameObject.SetActive(true);
                    handIsVisible = true;
                }
                else
                {
                    centerDotImage.gameObject.SetActive(true);
                    handIcon.gameObject.SetActive(false);
                    handIsVisible = false;
                }
                selectedNPC = null;
            }
            // NPC
            else if (npcVoice && npcVoice.playerInRange)
            {
                onTarget = true;
                selectedNPC = npcVoice.gameObject;
                seclectedObject = null;

                interaction_Info_UI.SetActive(true);

                centerDotImage.gameObject.SetActive(false);
                handIcon.gameObject.SetActive(true);
                handIsVisible = true;
            }
            // Kẻ thù
            else if (enemy != null)
            {
                onTarget = true;
                seclectedObject = enemy.gameObject;
                selectedNPC = null;

                interaction_text.text = enemy.enemyID + " (" + enemy.health + ")";

                interaction_Info_UI.SetActive(true);

                centerDotImage.gameObject.SetActive(true);
                handIcon.gameObject.SetActive(false);
                handIsVisible = false;
            }
            // Thỏ
            else if (rabbit != null)
            {
                onTarget = true;
                seclectedObject = rabbit.gameObject;
                selectedNPC = null;

                interaction_text.text = rabbit.animalID + " (" + rabbit.health + ")";

                interaction_Info_UI.SetActive(true);

                centerDotImage.gameObject.SetActive(true);
                handIcon.gameObject.SetActive(false);
                handIsVisible = false;
            }
            // Hòm đồ
            else if (storage != null && storage.playerInRange)
            {
                onTarget = true;
                seclectedObject = storage.gameObject;
                selectedNPC = null;

                interaction_text.text = "Open Storage [E]";
                interaction_Info_UI.SetActive(true);

                centerDotImage.gameObject.SetActive(false);
                handIcon.gameObject.SetActive(true);
                handIsVisible = true;
            }
            // Nguồn nước
            else if (waterSource != null && waterSource.playerInRange)
            {
                onTarget = true;
                seclectedObject = waterSource.gameObject;
                selectedNPC = null;
                interaction_text.text = "Drink [E]"; // Text tương tác
                interaction_Info_UI.SetActive(true);
                centerDotImage.gameObject.SetActive(false); // Dùng icon tay
                handIcon.gameObject.SetActive(true);
                handIsVisible = true;
            }
            else
            {
                onTarget = false;
                interaction_Info_UI.SetActive(false);
                centerDotImage.gameObject.SetActive(true);
                handIcon.gameObject.SetActive(false);

                handIsVisible = false;
                selectedNPC = null;
            }
        }
        else
        {
            onTarget = false;
            interaction_Info_UI.SetActive(false);
            centerDotImage.gameObject.SetActive(true);
            handIcon.gameObject.SetActive(false);
            handIsVisible = false;
            
            selectedNPC = null; 
            seclectedObject = null; 
            
             if (selectedTree!= null)
             {
                selectedTree.gameObject.GetComponent<ChoppableTree>().canBeChopped = false;
                selectedTree = null;
                chopHolder.gameObject.SetActive(false);
             }
        }
    }

    public void DisableSelection()
    {
        handIcon.enabled = false;
        centerDotImage.enabled = false;
        interaction_Info_UI.SetActive(false);
        
        seclectedObject = null;
        selectedNPC = null; 
    }
    public void EnableSelection()
    {
        handIcon.enabled = true;
        centerDotImage.enabled = true;
        interaction_Info_UI.SetActive(true);
    }
    
}