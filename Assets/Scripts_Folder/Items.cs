using UnityEngine;

public class Items : MonoBehaviour
{
    public enum ItemType
    {
        car,
        SpeedUpgrade,
        multiplier,
        invest,
        gamble
    }
    public static int GetCost(ItemType itemtype)
    {
        switch (itemtype)
        {

            default:
            case ItemType.car: return 10;
            case ItemType.SpeedUpgrade: return 20;
            case ItemType.multiplier: return 30;
            case ItemType.invest: return 50;
            case ItemType.gamble: return 10;
        }
    }
    public static Sprite GetSprite(ItemType itemType)
    {
        switch (itemType)
        {
            default:
            case ItemType.car: return GameAssets.i.car;
            case ItemType.SpeedUpgrade: return GameAssets.i.SpeedUpgrade;
            case ItemType.multiplier: return GameAssets.i.multiplier;
            case ItemType.invest: return GameAssets.i.invest;
            case ItemType.gamble: return GameAssets.i.gamble;
        }

    }
}