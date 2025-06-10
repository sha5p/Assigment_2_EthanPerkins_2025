using System;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Items;

public class Shop : MonoBehaviour
{
    private Transform container;
    private Transform shopItemTemplate;
    private IShopCustomer shopCustomer;
    public GameObject Customer;

    public TextMeshProUGUI counter;
    private void Awake()
    {
        container = transform.Find("container");
        shopItemTemplate = container.Find("shopItemTemplate");
        shopItemTemplate.gameObject.SetActive(false);
    }
    private void Start()
    {
        if (!PlayerPrefs.HasKey("CountDollar"))
        {
            PlayerPrefs.SetInt("CountDollar", 0);
        }
        IShopCustomer sh = Customer.GetComponent<IShopCustomer>();
        Show(sh);

        if (PlayerPrefs.GetInt("HasBoughtCar", 0) == 0)
        {
            createItemButton(Items.ItemType.car, Items.GetSprite(Items.ItemType.car), "car", Items.GetCost(Items.ItemType.car), 0);

        }
        else
        {
            Debug.Log("Car already bought. Car button not created.");

            Debug.Log("Script One disabled: ");
        }
        var currentSpeed = PlayerPrefs.GetInt("CarSpeed", 0);
        var text = currentSpeed.ToString();
        var currentMulti = PlayerPrefs.GetFloat("Multi", 1f);
        var text2 = currentMulti.ToString();
        var stock = PlayerPrefs.GetInt("Stock", 0);
        var text3 = stock.ToString();
        createItemButton(Items.ItemType.SpeedUpgrade, Items.GetSprite(Items.ItemType.SpeedUpgrade), "Car Speed "+text+ "% ",Items.GetCost(Items.ItemType.SpeedUpgrade), 1);
        createItemButton(Items.ItemType.multiplier, Items.GetSprite(Items.ItemType.multiplier), "Multipler " + text2 + "X", Items.GetCost(Items.ItemType.multiplier), 2);
        createItemButton(Items.ItemType.invest, Items.GetSprite(Items.ItemType.invest), "PizzaStock: "+ text3, Items.GetCost(Items.ItemType.invest), 3);
        createItemButton(Items.ItemType.gamble, Items.GetSprite(Items.ItemType.gamble), "gamble", Items.GetCost(Items.ItemType.gamble), 4);

    }
    private void createItemButton(Items.ItemType itemType, Sprite itemsprite, string itemname, int itemcost, int positionIndex)
    {
        Transform shopItemTransform = Instantiate(shopItemTemplate, container);
        shopItemTransform.gameObject.SetActive(true);
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();

        float shopItemHeight = 70f;
        shopItemRectTransform.anchoredPosition = new Vector2(0, -shopItemHeight * positionIndex);

        shopItemTransform.Find("nameText").GetComponent<TextMeshProUGUI>().SetText(itemname);
        shopItemTransform.Find("costText").GetComponent<TextMeshProUGUI>().SetText(itemcost.ToString());

        shopItemTransform.Find("itemImage").GetComponent<Image>().sprite = itemsprite;

        shopItemTransform.GetComponent<Button>().onClick.AddListener(() =>
        {
            TryBuyItem(itemType, shopItemTransform.gameObject);
        });
       

    }

    private void TryBuyItem(Items.ItemType itemType, GameObject clickedButtonGameObject)
    {
        int itemCost = Items.GetCost(itemType);
        int currentGold = PlayerPrefs.GetInt("CountDollar", 0); 

        if (currentGold >= itemCost)
        {
            PlayerPrefs.SetInt("CountDollar", currentGold - itemCost);
            PlayerPrefs.Save(); 
            shopCustomer.BoughtItem(itemType);

            Debug.Log($"Successfully bought {itemType}. Remaining gold: {PlayerPrefs.GetInt("CountDollar")}");
            int currentDollars = PlayerPrefs.GetInt("CountDollar", 0);
            string currentDollarsText = currentDollars.ToString();
            counter.text = currentDollarsText;
            switch (itemType)
            {
                case Items.ItemType.car:  buyCar(); break;
                case Items.ItemType.SpeedUpgrade: speed(clickedButtonGameObject); break;
                case Items.ItemType.multiplier: multipler(clickedButtonGameObject); break;
                case Items.ItemType.invest: stock(clickedButtonGameObject); break;
                case Items.ItemType.gamble: gameble_D(); break;
            }
            if (itemType == Items.ItemType.car)
            {
                buyCar();
                if (clickedButtonGameObject != null)
                {
                    Destroy(clickedButtonGameObject);
                }
            }
        }
        else
        {
            Debug.Log($"Not enough gold to buy {itemType}. Cost: {itemCost}, Current gold: {currentGold}");
        }

    }
    public Roulette rouletteScript;
    public GameObject routle;
    public void gameble_D()
    {
        if (rouletteScript != null)
        {
            Debug.Log("Calling Rotete() from gameble_D()");
            routle.SetActive(false);
            rouletteScript.bruh(); 
        }
    }
    public void stock(GameObject targetShopItemTransform)
    {
        Transform nameTextTransform = targetShopItemTransform.transform.Find("nameText");
        var stock = PlayerPrefs.GetInt("Stock", 0);
        if (nameTextTransform != null)
        {
            TextMeshProUGUI nameTextComponent = nameTextTransform.GetComponent<TextMeshProUGUI>();

            if (nameTextComponent != null && stock < 100)
            {
                stock += 1;
                PlayerPrefs.SetInt("Stock", stock);
                PlayerPrefs.Save();
                var text = stock.ToString();
                nameTextComponent.text = "PizzaStock: " + text;
                Debug.Log("Changed nameText to: bob");
            }
            else
            {
                int currentGold = PlayerPrefs.GetInt("CountDollar", 0);
                PlayerPrefs.SetInt("CountDollar", currentGold + 50);
                PlayerPrefs.Save();

                int z = PlayerPrefs.GetInt("CountDollar", 0);
                string b = z.ToString();
                counter.text = b;

            }
        }
    }
    public void speed(GameObject targetShopItemTransform)
    {

        Transform nameTextTransform = targetShopItemTransform.transform.Find("nameText");
        var currentSpeed= PlayerPrefs.GetInt("CarSpeed", 0);
        if (nameTextTransform != null)
        {
            TextMeshProUGUI nameTextComponent = nameTextTransform.GetComponent<TextMeshProUGUI>();

            if (nameTextComponent != null && currentSpeed<100)
            {
                currentSpeed += 25;
                PlayerPrefs.SetInt("CarSpeed", currentSpeed);
                PlayerPrefs.Save();
                var text= currentSpeed.ToString();
                nameTextComponent.text = "Car Speed " + text + "%";
                Debug.Log("Changed nameText to: bob");
            }
            else
            {
                int currentGold = PlayerPrefs.GetInt("CountDollar", 0);
                PlayerPrefs.SetInt("CountDollar", currentGold + 20);
                PlayerPrefs.Save();

                int z = PlayerPrefs.GetInt("CountDollar", 0);
                string b = z.ToString();
                counter.text = b;

            }
        }
        
    }
    public void multipler(GameObject targetShopItemTransform)
    {
        Transform nameTextTransform = targetShopItemTransform.transform.Find("nameText");
        var currentMulti = PlayerPrefs.GetFloat("Multi", 1f);
        if (nameTextTransform != null)
        {
            TextMeshProUGUI nameTextComponent = nameTextTransform.GetComponent<TextMeshProUGUI>();

            if (nameTextComponent != null && currentMulti < 2)
            {
                currentMulti += 0.1f;
                PlayerPrefs.SetFloat("Multi", currentMulti);
                PlayerPrefs.Save();
                var text = currentMulti.ToString();
                nameTextComponent.text = "Multipler " + text + "X";
                Debug.Log("Changed nameText to: bob");
            }
            else
            {
                int currentGold = PlayerPrefs.GetInt("CountDollar", 0);
                PlayerPrefs.SetInt("CountDollar", currentGold + 30);
                PlayerPrefs.Save();

                int z = PlayerPrefs.GetInt("CountDollar", 0);
                string b = z.ToString();
                counter.text = b;

            }
        }
    }
    public CarBoard carboard;
    public void buyCar()
    {
        carboard.carbought();
        PlayerPrefs.SetInt("HasBoughtCar", 1); 
        PlayerPrefs.Save();

    }
    private float _speedUpgradePercentage = 0.0f;
  

    public void Show(IShopCustomer customer) 
    { 
        this.shopCustomer = customer;
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}