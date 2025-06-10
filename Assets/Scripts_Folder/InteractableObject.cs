using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string ItemName = "Interactable Object";
    public Canvas canvas;
    public string GetItemName()
    {
        return ItemName;
    }

    public virtual void OnInteract()
    {
        Debug.Log("Interacted with " + ItemName);

        

        if (canvas != null)
        {
            canvas.enabled = !canvas.enabled; 
        }
    }
    public void Start()
    {
    }

    public void Update()
    {
        GameObject arrow_Pare = GameObject.FindWithTag("arrowBoi");
        if (gameObject.CompareTag("NPC"))
        {
            arrow_Pare.SetActive(true);
            Transform myGameObject = transform;
            arrow_Pare.GetComponent<arrow_Pare>().target = myGameObject;
            foreach (Transform child in arrow_Pare.transform)
            {
                child.gameObject.SetActive(true);
            }
            Debug.Log("NPC"); 
        }
    }

    public virtual void VisibleFalse()
    {
        Debug.Log("Interacted with " + ItemName);
        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }
}
