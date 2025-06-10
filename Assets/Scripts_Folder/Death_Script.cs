using System.Security.Cryptography;
using UnityEngine;

public class Death_Script : MonoBehaviour
{
    public GameObject replacementPrefab;

    SelectionManager selectionManager;
    void Awake() 
    {
        selectionManager = FindObjectOfType<SelectionManager>();
        if (selectionManager == null)
        {
            Debug.LogError("No SelectionManager found in the scene!");
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("YourCar"))
        {
            Debug.Log("Trigger detected");

            if (replacementPrefab != null)
            {
                Instantiate(replacementPrefab, transform.position, transform.rotation);
            }
            else
            {
                Debug.LogWarning("Replacement Prefab is not assigned in the Inspector for " + gameObject.name);
            }
            selectionManager.RewardPileOfCoin(10,10f);
            Destroy(gameObject);
            Debug.Log(gameObject.name + " destroyed.");
        }
    }
}
