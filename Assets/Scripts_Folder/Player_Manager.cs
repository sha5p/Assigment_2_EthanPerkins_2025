using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_Manager : MonoBehaviour,IShopCustomer
{
    public GameObject overgame;
    public CanvasGroup z;
    public TextMeshProUGUI text;
    public void BoughtItem(Items.ItemType itemType)
    {
        Debug.Log("ShopSytem yay");
    }

    public bool TrySpendGoldAmount(int goldAmount)
    {
        throw new System.NotImplementedException();
    }

    void Start()
    {
        Time.timeScale = 1f;
    }

    void Update()
    {
        GameObject[] npcsInScene = GameObject.FindGameObjectsWithTag("NPC");

        if (npcsInScene.Length == 0)
        {
            Debug.Log("No NPCs left in the scene!");
            overgame.SetActive(true);
            z.interactable = true;
            text.text = "All NPCS died you can't deliver in a deliver game you lose";



        }
    }
}
